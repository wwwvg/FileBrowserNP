using FileBrowserNP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FileBrowserNP.Models.MyEventArgs;
using FileBrowserNP.Commands;
using FileBrowserNP.Views.Dialogs;
using FileBrowserNP.ViewModels.Dialogs;
using Microsoft.VisualBasic.FileIO;

namespace FileBrowserNP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            LoadDriveView();    // при запуске программы отображаем список дисков
            DeleteItemImage = new BitmapImage(new Uri("..\\Icons\\DeleteItem.png", UriKind.Relative));
            AddFolderImage = new BitmapImage(new Uri("..\\Icons\\AddFolder.png", UriKind.Relative));
            RefreshImage = new BitmapImage(new Uri("..\\Icons\\Refresh.png", UriKind.Relative));
        }

        #region СВОЙСТВА
        IDialogService _dialogService = new DialogService();

        private List<string> _listOfFiles = new();

        private Stack<int> _previousSelectedIndexes = new();

        public ImageSource DeleteItemImage { get; set; }
        public ImageSource AddFolderImage { get; set; }
        public ImageSource RefreshImage { get; set; }

        private BindableBase _LeftViewModel;
        public BindableBase LeftViewModel { get => _LeftViewModel; set => SetProperty(ref _LeftViewModel, value); }

        
        private BindableBase _RightViewModel;
        public BindableBase RightViewModel { get => _RightViewModel; set => SetProperty(ref _RightViewModel, value); }

       
        private ObservableCollection<Base> _files = new(); // список дисков, файлов и каталогов
        public ObservableCollection<Base> Files { get => _files; set => SetProperty(ref _files, value); }

       
        private Base _selectedFile = new();
        public Base SelectedFile { get => _selectedFile; set => SetProperty(ref _selectedFile, value); }

        
        private ImageSource _imageStatusBar;  // значок предупреждения в статусбаре
        public ImageSource ImageStatusBar { get => _imageStatusBar; set => SetProperty(ref _imageStatusBar, value); }

       
        private string _messageStatusBar;  // информация о дисках, папках, файлов и ошибок в статусбаре
        public string MessageStatusBar { get => _messageStatusBar; set => SetProperty(ref _messageStatusBar, value); }

       
        private bool _canAddFolder; // доступность кнопки Добавить папку
        public bool CanAddFolder { get => _canAddFolder; set => SetProperty(ref _canAddFolder, value); }

        
        private bool _CanDeleteItem;// доступность кнопки Удалить папку/файл
        public bool CanDeleteItem { get => _CanDeleteItem; set => SetProperty(ref _CanDeleteItem, value); }
        #endregion

        #region КОМАНДЫ И ОБРАБОТЧИКИ
        #region ДОБАВИТЬ
        private DelegateCommand _addFolder;
        public DelegateCommand AddFolder => _addFolder ?? (_addFolder = new DelegateCommand(ExecuteAddFolder, CanExecuteAddFolder).
                                                                                                            ObservesProperty(() => CanAddFolder));

        void ExecuteAddFolder()
        {
            ShowAddFolderDialog();
        }

        bool CanExecuteAddFolder()
        {
            return CanAddFolder;
        }

        void ShowAddFolderDialog()
        {
            var vmAddFolder = new AddFolderDialogViewModel(GetCurrentDirectoryFileNames());
            bool createFolder = false;

            _dialogService.ShowDialog<AddFolderDialogViewModel>(vmAddFolder, result =>
            {
                createFolder = (bool)result;
            });

            if(createFolder)
            {
                string newFolderName = vmAddFolder.NewFolderName;
                newFolderName = Path.Combine(GetSelectedItemParentForAddFolder(), newFolderName);
                try
                {
                    FileSystem.CreateDirectory(newFolderName);
                    ExecuteRefresh();
                }
                catch(Exception ex)
                {
                    MessageStatusBar = ex.Message;
                }
            }
        }
        #endregion

        #region УДАЛИТЬ
        private DelegateCommand _deleteItem;
        public DelegateCommand DeleteItem => _deleteItem ?? (_deleteItem = new DelegateCommand( ExecuteDeleteItem, CanExecuteDeleteItem).ObservesProperty(() => CanDeleteItem));

        void ExecuteDeleteItem()
        {
            ShowDeleteDialog();
        }

        bool CanExecuteDeleteItem()
        {
            return CanDeleteItem;
        }

        private void ShowDeleteDialog()
        {
            bool deleteFile = false;
            bool isFolder = ((FolderViewModel)_LeftViewModel).SelectedFile is Folder;
            var vmDeleteFile = new DeleteDialogViewModel( GetSelectedItemPath());
            _dialogService.ShowDialog<DeleteDialogViewModel>(vmDeleteFile, result =>
            {
                deleteFile = (bool)result;
            });

            if (deleteFile)
            {
                try
                {
                    bool moveToRecycleBin = vmDeleteFile.MoveToRecycleBin;
                    if (moveToRecycleBin) // переместить в корзину или удалить навсегда
                    {
                        if (isFolder)
                            FileSystem.DeleteDirectory( GetSelectedItemPath(), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        else
                            FileSystem.DeleteFile( GetSelectedItemPath(), UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else
                    {
                        if (isFolder)
                            Directory.Delete( GetSelectedItemPath(), true);
                        else
                            File.Delete( GetSelectedItemPath());
                    }
                    ExecuteRefresh();
                }
                catch (Exception ex)
                {
                    MessageStatusBar = ex.Message;
                }
            }
        }

        #endregion

        #region ОБНОВИТЬ
        private DelegateCommand _refresh;
        public DelegateCommand Refresh => _refresh ?? (_refresh = new DelegateCommand(ExecuteRefresh));

        void ExecuteRefresh()
        {
            if(_LeftViewModel is FolderViewModel)
                LoadLeftFolderView(GetSelectedItemParentForAddFolder(), false, true);
        }
        #endregion
        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ ДИСКОВ

        //private void GetSelectedIndex()  
        //{
        //    if (LeftViewModel is FolderViewModel file)
        //    {

        //    }
        //}
        

        private void LoadDriveView()   // текущая вью-модель переключается на диски
        {
            CanAddFolder = false;
            CanDeleteItem = false;

            if(_previousSelectedIndexes.Count == 0)
                LeftViewModel = new DriveViewModel(0);
            else
            {
                LeftViewModel = new DriveViewModel(_previousSelectedIndexes.Pop());                                                     // <======================== POP
            }
            var driveVM = (DriveViewModel)LeftViewModel;
            driveVM.DriveDoubleClicked += OnDriveDoubleClicked;   // подключен обработчик двойного клика по диску
            driveVM.DriveSelected += OnDriveSelected;    // подключен обработчик выбора диска
            LoadRightFolderView(driveVM.SelectedDrive.Name);
            ShowDriveInfoMessage(driveVM.SelectedDrive.Label, driveVM.SelectedDrive.FreeSpace, driveVM.SelectedDrive.TotalSpace);
        }     
        private void OnDriveSelected(object sender, SelectedDriveEventArgs e)   // выбрали диск
        {
            if (e.SelectedItem != null && e.SelectedItem is Drive)
            {
                Drive drive = (Drive)e.SelectedItem;
                ImageStatusBar = null;
                ShowDriveInfoMessage(drive.Label, drive.FreeSpace, drive.TotalSpace);    
                if(!_isDriveDoubleClicked)
                    LoadRightFolderView(drive.Name);
                _isDriveDoubleClicked = false;
            }
        }
        bool _isDriveDoubleClicked = false;
        public void OnDriveDoubleClicked(object sender, SelectedDriveEventArgs e)  // по диску щелкнули два раза -> создаем вместо дисков каталоги, справа дочерние каталоги
        {
            if (e.SelectedItem != null && e.SelectedItem is Drive drive)
            {
                LoadLeftFolderView(drive.Name, false, false);
    //            RightViewModel = new BackViewModel();
                CanAddFolder = true;
                _previousSelectedIndexes.Push(e.SelectedIndex);                                                                                   // <======================== PUSH
                RightViewModel = new BackViewModel();
                _isDriveDoubleClicked = true;
            }
        }
        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ ПАПОК/ФАЙЛОВ

        //*******************************    ФАЙЛЫ СЛЕВА     ***********************************************************************************                           
        private void LoadLeftFolderView(string path, bool isBack, bool isRefreshRequested = false)    
        {  
            try
            {
                if (isBack)
                {
                    int previuosIndex = _previousSelectedIndexes.Pop();    // взял из стека предыдущий индекс                                   // <======================== POP   
                    LeftViewModel = new FolderViewModel(path, true, previuosIndex, true);   // текущая вью-модель переключается на папки
  
                    if (previuosIndex == 0)
                        CanDeleteItem = false;
                    else
                        CanDeleteItem = true;
                }
                else
                {
                    if(isRefreshRequested)
                        LeftViewModel = new FolderViewModel(path, true, 0, true);
                    else
                        LeftViewModel = new FolderViewModel(path, true, 0, false);
                    
                    CanDeleteItem = false;
                }
                ((FolderViewModel)LeftViewModel).ItemDoubleClicked += OnFileDoubleClicked;   // подключен обработчик двойного клика по папкам и файлам
                ((FolderViewModel)LeftViewModel).ItemSelected += OnLeftFileSelected;    // подключен обработчик выбора папок/файлов
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        //*******************************    ФАЙЛЫ СПРАВА     ***********************************************************************************
        private void LoadRightFolderView(string path)                                                   
        {
            RightViewModel = new FolderViewModel(path, false, 0, false);
            ((FolderViewModel)RightViewModel).ItemSelected += OnRightFileSelected;    // подключен обработчик выбора файлов справа
        }

        //*******************************    ДВОЙНОЙ ЩЕЛЧОК ПО ФАЙЛУ     ***********************************************************************************
        bool _isBackDoubleClicked = false;
        public void OnFileDoubleClicked(object sender, SelectedItemEventArgs e)                     
        {
            try
            {
                if (e.SelectedItem != null)
                {
                    Base myType = (Base)e.SelectedItem;

                    Type type = myType.GetType();

                    if (type == typeof(Folder))
                    {
                        LoadLeftFolderView(GetSelectedItemPath(), false);
                        RightViewModel = new BackViewModel();
                        _previousSelectedIndexes.Push(e.SelectedIndex);                                              // <======================== PUSH
                        return;
                    }

                    if (type == typeof(Back))
                    {
                        _isBackDoubleClicked = true;

                        if (GetSelectedItemParent() != null)
                        {
                            string s = GetSelectedItemPath();
                            string s1 = GetSelectedItemParent();
                            LoadLeftFolderView(GetSelectedItemParent(), true);
                            return;
                        }
                        else
                        {
                            LoadDriveView();
                        }
                    }

                    if (/*type == typeof(HexFile) || */ type == typeof(TextFile) || type == typeof(ImageFile))
                        OpenFile();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        //*******************************    ВЫБОР ЛЕВОГО ФАЙЛА     ************************************************************

        public void OnLeftFileSelected(object sender, SelectedItemEventArgs e)  // выбрали папку/файл
        {
            if (e.SelectedItem is Back)
            {
                if (!_isBackDoubleClicked)
                {
                    RightViewModel = new BackViewModel();            //  отображение пустой правой панели
                    
                }
                CanDeleteItem = false;
                return;
            }
            else
                CanDeleteItem = true;

            _isBackDoubleClicked = false;

            try
            {
                if (e.SelectedItem != null && e.SelectedItem is Folder folder)
                {
                    folder = ((Folder)e.SelectedItem);                                              // показ файлов справа   
                    ShowFileInfoMessage(folder.Path, folder.Size, folder.TimeCreated);
                    LoadRightFolderView(folder.Path);                                               
                }

                if (e.SelectedItem != null && e.SelectedItem is HexFile hexFile)
                {
                    RightViewModel = new HexViewModel(((HexFile)e.SelectedItem).Path);            //  вывод НЕХ файла
                    ShowFileInfoMessage(hexFile.Path, hexFile.Size, hexFile.TimeCreated);
                }

                if (e.SelectedItem != null && e.SelectedItem is ImageFile imageFile)
                {
                    RightViewModel = new ImageViewModel(((ImageFile)e.SelectedItem).Path);            //  вывод картинки
                    ShowFileInfoMessage(imageFile.Path, imageFile.Size, imageFile.TimeCreated);
                }

                if (e.SelectedItem != null && e.SelectedItem is TextFile textFile)
                {
                    RightViewModel = new TextViewModel(((TextFile)e.SelectedItem).Path);            //  вывод текста
                    ShowFileInfoMessage(textFile.Path, textFile.Size, textFile.TimeCreated);
                }
            }
            catch(Exception ex)
            {
                ShowErrorMessage(ex.Message);
                RightViewModel = new BackViewModel();
            }
        }

        //*******************************    ВЫБОР ПРАВОГО ФАЙЛА     ************************************************************
        public void OnRightFileSelected(object sender, SelectedItemEventArgs e)  // выбрали папку/файл
        {
            if (e.SelectedItem is Back)
                RightViewModel = new BackViewModel();            //  отображение пустой правой панели

            try
            {
                if (e.SelectedItem != null && e.SelectedItem is Folder folder)
                {
                    folder = ((Folder)e.SelectedItem);
                    ShowFileInfoMessage(folder.Path, folder.Size, folder.TimeCreated);
                }

                if (e.SelectedItem != null && e.SelectedItem is HexFile hexFile)
                {                
                    ShowFileInfoMessage(hexFile.Path, hexFile.Size, hexFile.TimeCreated); //  вывод НЕХ файла
                }

                if (e.SelectedItem != null && e.SelectedItem is ImageFile imageFile)
                {      
                    ShowFileInfoMessage(imageFile.Path, imageFile.Size, imageFile.TimeCreated); //  вывод картинки
                }

                if (e.SelectedItem != null && e.SelectedItem is TextFile textFile)
                {   
                    ShowFileInfoMessage(textFile.Path, textFile.Size, textFile.TimeCreated); //  вывод текста
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }
        #endregion
        //*******************************************************************************************************************************************************
        private List<string> GetCurrentDirectoryFileNames() // файлы в каталоге
        {
            if (LeftViewModel is FolderViewModel file)
            {
                List<string> list = new List<string>();
                foreach (var item in file.Files)
                    if (item is Folder)
                        list.Add(item.Name.Replace("[", "").Replace("]", "").ToUpper());
                return list;
            }
            return new List<string>();
        }
        private string GetSelectedItemPath()        // путь к выбранному файлу
        {
            if (LeftViewModel is FolderViewModel file)
                return file.SelectedFile.Path;

            return string.Empty;
        }

        private string GetSelectedItemParent()    // путь к родителю выбранного файла
        {
            if (LeftViewModel is FolderViewModel file)
                return Directory.GetParent(GetSelectedItemPath())?.FullName;

            return string.Empty;
        }

        private string GetSelectedItemParentForAddFolder()    // путь к родителю выбранного файла
        {
            if (LeftViewModel is FolderViewModel file)
                if (Directory.GetParent(GetSelectedItemPath()) == null)
                    return file.SelectedFile.Path;
                else
                    return Directory.GetParent(GetSelectedItemPath()).FullName;

            return string.Empty;
        }

        #region ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        //*******************************************************************************************************************************************************
        private void OpenFile()
        {
            try
            {
                string Location_ToOpen = GetSelectedItemPath();
                if (!File.Exists(Location_ToOpen))
                {
                    return;
                }

                string argument = "/open, \"" + Location_ToOpen + "\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
                return;
            }
            catch (Exception ex)
            {
                MessageStatusBar = ex.Message;
            }
        }

        public void ShowFileInfoMessage(string path, string size, string timeCreated)  // ошибка открытия/чтения дисков, папок и файлов
        {
            ImageStatusBar = new BitmapImage();
            MessageStatusBar = $"Путь: {path}         Размер: {size}         Дата и время изменения: {timeCreated}"; ;
        }

        public void ShowErrorMessage(string errorMessage)  // ошибка открытия/чтения дисков, папок и файлов
        {
            ImageStatusBar = new BitmapImage(new Uri("..\\Icons\\Warning.png", UriKind.Relative));
            MessageStatusBar = errorMessage;
        }

        public void ShowDriveInfoMessage(string label, string freeSpace, string totalSpace)  // ошибка открытия/чтения дисков, папок и файлов
        {
            ImageStatusBar = new BitmapImage();
            MessageStatusBar = $"[{label}]  свободно {freeSpace} из {totalSpace}";
        }
    }
    #endregion
}
