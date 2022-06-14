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
using System.Linq;
using FileBrowserNP.ViewModels.Dialogs;
using System.Diagnostics;

namespace FileBrowserNP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            LoadDriveView(false, true);    // при запуске программы отображаем список дисков
            DeleteItemImage = new BitmapImage(new Uri("..\\Icons\\DeleteItem.png", UriKind.Relative));
            AddFolderImage = new BitmapImage(new Uri("..\\Icons\\AddFolder.png", UriKind.Relative));
            RefreshImage = new BitmapImage(new Uri("..\\Icons\\Refresh.png", UriKind.Relative));
        }

        #region СВОЙСТВА
        IDialogService _dialogService = new DialogService();

  //      private List<string> _listOfFiles = new();

        private bool _isError;
        private string _currentDirectory = "";
        private Stack<int> _previousSelectedIndexes = new();

        public ImageSource DeleteItemImage { get; set; }
        public ImageSource AddFolderImage { get; set; }
        public ImageSource RefreshImage { get; set; }


        private int _selectedIndex = 0;
        public int SelectedIndex { get => _selectedIndex;  set => SetProperty(ref _selectedIndex, value); }

        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }

        
        private BindableBase _contentViewModel;
        public BindableBase ContentViewModel { get => _contentViewModel; set => SetProperty(ref _contentViewModel, value); }
      
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
            Directory.GetDirectories(_currentDirectory);
            List<string> folderNames = new();
            foreach (string folderName in Directory.GetDirectories(_currentDirectory).ToList<string>())
                folderNames.Add((string)Path.GetFileName(folderName));

            var vmAddFolder = new AddFolderDialogViewModel(folderNames);
            vmAddFolder.CurrentDirectory = _currentDirectory;
            bool createFolder = false;

            _dialogService.ShowDialog<AddFolderDialogViewModel>(vmAddFolder, result =>
            {
                createFolder = (bool)result;
            });

            if(createFolder)
            {
                string newFolderName = vmAddFolder.NewFolderName;
                newFolderName = Path.Combine(_currentDirectory, newFolderName);
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
            string fileName = GetSelectedFullFileName();
            bool isFolder = ((FolderViewModel)_currentViewModel).SelectedFile is Folder;
            var vmDeleteFile = new DeleteDialogViewModel(fileName);
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
                            FileSystem.DeleteDirectory(fileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        else
                            FileSystem.DeleteFile(fileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else
                    {
                        if (isFolder)
                            Directory.Delete(fileName, true);
                        else
                            File.Delete(fileName);
                    }
                    ExecuteRefresh();
                }
                catch (Exception ex)
                {
                    MessageStatusBar = ex.Message;
                }
            }
        }

        private string GetSelectedFullFileName()
        {

            string fileName = ((FolderViewModel)_currentViewModel).SelectedFile.Name.Replace("[", "").Replace("]", "");
            fileName = Path.Combine(_currentDirectory, fileName);
            return fileName;
        }
        #endregion

        #region ОБНОВИТЬ
        private DelegateCommand _refresh;
        public DelegateCommand Refresh => _refresh ?? (_refresh = new DelegateCommand(ExecuteRefresh));

        void ExecuteRefresh()
        {
            if(_currentViewModel is FolderViewModel)
                CreateNewFolderView(_currentDirectory, true, false, true);
        }
        #endregion
        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ ДИСКОВ
        private void LoadDriveView(bool isBack, bool isFirstTime = false)   // текущая вью-модель переключается на диски
        {
            CanAddFolder = false;
            CanDeleteItem = false;
            if (isBack)
                CurrentViewModel = new DriveViewModel(_previousSelectedIndexes.Pop());
            else // если вперед           
                CurrentViewModel = new DriveViewModel(0);
 

            ((DriveViewModel)CurrentViewModel).DriveDoubleClicked += OnDriveDoubleClicked;   // подключен обработчик двойного клика по диску
            ((DriveViewModel)CurrentViewModel).ItemSelected += OnDriveSelected;    // подключен обработчик выбора диска

            var drive = ((DriveViewModel)CurrentViewModel);
            ContentViewModel = new FolderViewModel(drive.SelectedDrive.Name, false, 0, false);
            MessageStatusBar = $"[{drive.SelectedDrive.Label}]  свободно {drive.SelectedDrive.FreeSpace} из {drive.SelectedDrive.TotalSpace}";
        }

        private void OnDriveSelected(object sender, SelectedDriveEventArgs e)   // выбрали диск
        {        
            if (e.SelectedItem != null && e.SelectedItem is Drive && !_isDriveDoubleClicked)
            {
                Drive drive = (Drive)e.SelectedItem;
                SelectedIndex = (int)e.SelectedIndex;
                ImageStatusBar = null;
                if (!_isError)
                    MessageStatusBar = $"[{drive.Label}]  свободно {drive.FreeSpace} из {drive.TotalSpace}";
                else
                        _isError = false;
                _currentDirectory = drive.Name;

                ContentViewModel = new FolderViewModel(_currentDirectory, false, 0, false);  // дочерние папки
            }
            _isDriveDoubleClicked = false;
        }
        private bool _isDriveDoubleClicked;
        public void OnDriveDoubleClicked(object sender, SelectedDriveEventArgs e)  // по диску щелкнули два раза -> создаем вместо дисков каталоги, справа дочерние каталоги
        {
            _isDriveDoubleClicked = true;
            if (e.SelectedItem != null && e.SelectedItem is Drive drive)
            {
                CreateNewFolderView(drive.Name, true, false);
                _previousSelectedIndexes.Push(SelectedIndex);
                MessageStatusBar = "";
                SelectedFile = ((FolderViewModel)CurrentViewModel).Files[0];
            }
        }
        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ ПАПОК/ФАЙЛОВ
        private void CreateNewFolderView(string path, bool isLeftPanelView, bool isBack, bool isRefreshRequested = false)
        {
            if (isBack)
            {
                int previuosIndex = _previousSelectedIndexes.Pop();
                CurrentViewModel = new FolderViewModel(path, isLeftPanelView, previuosIndex, true);   // текущая вью-модель переключается на папки
                _currentDirectory = Directory.GetParent(path)?.FullName ?? path;
                if (previuosIndex == 0)
                    CanDeleteItem = false;
                else
                    CanDeleteItem = true;
            }
            else
            {
                CurrentViewModel = new FolderViewModel(path, isLeftPanelView, 0, false);
                //if (isLeftPanelView && !isRefreshRequested)
                    //_previousSelectedIndexes.Push(SelectedIndex);
                _currentDirectory = path;
                CanDeleteItem = false;
            }
            ((FolderViewModel)CurrentViewModel).FileDoubleClicked += OnFileDoubleClicked;   // подключен обработчик двойного клика по диску
            ((FolderViewModel)CurrentViewModel).FileSelected += OnFileSelected;    // подключен обработчик выбора папок/файлов
        }

        public void OnFileDoubleClicked(object sender, SelectedItemEventArgs e)  // по папке/файлу щелкнули два раза или назад
        {
            _isFileDoubleClicked = true;
            if (e.SelectedItem != null)
            {
                Base myType = (Base)e.SelectedItem;

                Type type = myType.GetType();

                if (type == typeof(Folder))
                {
                    CreateNewFolderView(((Folder)e.SelectedItem).Path, true, false);
                    SelectedIndex = e.SelectedIndex;
                    _previousSelectedIndexes.Push(_selectedIndex);
                    return;
                }

                if (type == typeof(Back))
                {
                    //string s1 = Directory.GetParent(_currentDirectory).FullName;
                    //string s2 = Directory.GetDirectoryRoot(_currentDirectory);

                    if (_previousSelectedIndexes.Count > 1)
                    {
                        string currentDirectoryRoot = Directory.GetParent(_currentDirectory).FullName;
                        CreateNewFolderView(currentDirectoryRoot, true, true);
                        return;
                    }
                    else
                    {
                        LoadDriveView(true);
                    }
                }

                if (type == typeof(HexFile))
                {
                    
                    return;
                }

                if (type == typeof(TextFile))
                {
                    try
                    {
                        Process.Start("Notepad.exe", GetSelectedFullFileName());
                        return;
                    }
                    catch(Exception ex)
                    {
                        MessageStatusBar = ex.Message;
                        return;
                    }
                }

                if (type == typeof(ImageFile))
                {
                    try
                    {
                        string Location_ToOpen = GetSelectedFullFileName();
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
            }
        }
        private bool _isFileDoubleClicked;
        public void OnFileSelected(object sender, SelectedItemEventArgs e)  // выбрали папку/файл
        {
            if (!_isFileDoubleClicked)
            {
                CanAddFolder = true;
                if (e.SelectedItem is Back)
                {
                    ContentViewModel = new BackViewModel();            //  отображение пустой правой панели
                    CanDeleteItem = false;
                }
                else
                    CanDeleteItem = true;

                try
                {
                    if (e.SelectedItem != null && e.SelectedItem is Folder folder)
                    {
                        folder = ((Folder)e.SelectedItem);                                          // отображение каталогов справа
                        _currentDirectory = Directory.GetParent(folder.Path).FullName;
                        ShowFileInfoMessage(folder.Path, folder.Size, folder.TimeCreated);
                        ContentViewModel = new FolderViewModel(folder.Path, false, 0, false);
                        ((FolderViewModel)ContentViewModel).FileSelected += OnFileSelected;    // подключен обработчик выбора папок/файлов
                    }

                    if (e.SelectedItem != null && e.SelectedItem is HexFile hexFile)
                    {
                        ContentViewModel = new HexViewModel(((HexFile)e.SelectedItem).Path);            //  вывод НЕХ файла
                        ShowFileInfoMessage(hexFile.Path, hexFile.Size, hexFile.TimeCreated);
                    }

                    if (e.SelectedItem != null && e.SelectedItem is ImageFile imageFile)
                    {
                        ContentViewModel = new ImageViewModel(((ImageFile)e.SelectedItem).Path);            //  вывод картинки
                        ShowFileInfoMessage(imageFile.Path, imageFile.Size, imageFile.TimeCreated);
                    }

                    if (e.SelectedItem != null && e.SelectedItem is TextFile textFile)
                    {
                        ContentViewModel = new TextViewModel(((TextFile)e.SelectedItem).Path);            //  вывод текста
                        ShowFileInfoMessage(textFile.Path, textFile.Size, textFile.TimeCreated);
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            _isFileDoubleClicked = false;
        }
        #endregion

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
    }
}
