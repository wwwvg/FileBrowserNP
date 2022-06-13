using FileBrowserNP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FileBrowserNP.Models.MyEventArgs;
using FileBrowserNP.Commands;

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
        private bool _isError;
        private string _currentDirectory = "";
        private Stack<int> _previousSelectedIndexes = new();

        public ImageSource DeleteItemImage { get; set; }
        public ImageSource AddFolderImage { get; set; }
        public ImageSource RefreshImage { get; set; }


        private int _selectedIndex = 0;
        public int SelectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value);}

        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }

        
        private BindableBase _contentViewModel;
        public BindableBase ContentViewModel { get => _contentViewModel; set => SetProperty(ref _contentViewModel, value); }

       
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

        
        private bool _canDeleteItem;// доступность кнопки Удалить папку/файл
        public bool CanDeleteItem { get => _canDeleteItem; set => SetProperty(ref _canDeleteItem, value); }
        #endregion

        #region КОМАНДЫ И ОБРАБОТЧИКИ
        #region ДОБАВИТЬ
        private DelegateCommand _addFolder;
        public DelegateCommand AddFolder => _addFolder ?? (_addFolder = new DelegateCommand(o => ExecuteAddFolder(), o => CanExecuteAddFolder()));

        void ExecuteAddFolder()
        {
            ShowAddFolderDialog();
        }

        bool CanExecuteAddFolder()
        {
            if(_currentViewModel is FolderViewModel)
                return true;
            return false;
        }

        void ShowAddFolderDialog()
        {
            //_dialogService.ShowAddFolderDialog(_currentDirectory, r =>
            //{
            //    if (r.Result == ButtonResult.Yes)
            //    {
            //        try
            //        {
            //            string newFolderName = r.Parameters.GetValue<string>("NewFolderName");
            //            newFolderName = Path.Combine(_currentDirectory, newFolderName);
            //            FileSystem.CreateDirectory(newFolderName);
            //            _eventAggregator.GetEvent<RefreshRequested>().Publish();   //==========> сообщение FileViewModel на обновление списка файлов
            //        }
            //        catch (Exception ex)
            //        {
            //            _eventAggregator.GetEvent<Error>().Publish(ex.Message); //==========> сообщение ErrorViewModel если не удается удалить
            //        }
            //    }
            //});
        }
        #endregion

        #region УДАЛИТЬ
        private DelegateCommand _deleteItem;
        public DelegateCommand DeleteItem => _deleteItem ?? (_deleteItem = new DelegateCommand( o => ExecuteDeleteItem(), o => CanExecuteDeleteItem()));

        void ExecuteDeleteItem()
        {
            ShowDeleteDialog();
        }

        bool CanExecuteDeleteItem()
        {
            // если это файловое представление и выбранный элемент не является Back
            if (_currentViewModel is FolderViewModel && !(((FolderViewModel)_currentViewModel).SelectedFile is Back))         
                return true;

            return false;
        }

        private void ShowDeleteDialog()
        {
            //_dialogService.ShowDeleteDialog(_fileInfoModel?.FullPath, r =>
            //{
            //    if (r.Result == ButtonResult.Yes)
            //    {
            //        try
            //        {
            //            bool moveToRecycleBin = r.Parameters.GetValue<bool>("MoveToRecycleBin");
            //            if (moveToRecycleBin) // переместить в корзину или удалить навсегда
            //            {
            //                if (_fileInfoModel.Type == Helpers.FileType.Folder)
            //                    FileSystem.DeleteDirectory(_fileInfoModel.FullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            //                if (_fileInfoModel.Type == Helpers.FileType.Text || _fileInfoModel.Type == Helpers.FileType.Bin || _fileInfoModel.Type == Helpers.FileType.Image)
            //                    FileSystem.DeleteFile(_fileInfoModel.FullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            //            }
            //            else
            //            {
            //                if (_fileInfoModel.Type == Helpers.FileType.Folder)
            //                    Directory.Delete(_fileInfoModel.FullPath, true);
            //                if (_fileInfoModel.Type == Helpers.FileType.Text || _fileInfoModel.Type == Helpers.FileType.Bin || _fileInfoModel.Type == Helpers.FileType.Image)
            //                    File.Delete(_fileInfoModel.FullPath);
            //            }
            //            _eventAggregator.GetEvent<RefreshRequested>().Publish();   //==========> сообщение FileViewModel на обновление списка файлов

            //        }
            //        catch (Exception ex)
            //        {
            //            _eventAggregator.GetEvent<Error>().Publish(ex.Message); //==========> сообщение ErrorViewModel если не удается удалить
            //        }
            //    }
            //});
        }
        #endregion

        #region ОБНОВИТЬ
        private DelegateCommand _refresh;
        public DelegateCommand Refresh => _refresh ?? (_refresh = new DelegateCommand( o=> ExecuteRefresh()));

        void ExecuteRefresh()
        {
            
        }
        #endregion
        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ ДИСКОВ
        private void LoadDriveView(bool isBack, bool isFirstTime = false)   // текущая вью-модель переключается на диски
        {
            if (isBack)
                CurrentViewModel = new DriveViewModel(_previousSelectedIndexes.Pop());
            else // если вперед
            {
                if(!isFirstTime)
                    _previousSelectedIndexes.Push(SelectedIndex);
                CurrentViewModel = new DriveViewModel(0);
            }

            ((DriveViewModel)CurrentViewModel).DriveDoubleClicked += OnDriveDoubleClicked;   // подключен обработчик двойного клика по диску
            ((DriveViewModel)CurrentViewModel).ItemSelected += OnDriveSelected;    // подключен обработчик выбора диска
            ((DriveViewModel)CurrentViewModel).Error += OnError;

            var drive = ((DriveViewModel)CurrentViewModel);
            ContentViewModel = new FolderViewModel(drive.SelectedDrive.Name, false, 0, false);
            MessageStatusBar = $"[{drive.SelectedDrive.Label}]  свободно {drive.SelectedDrive.FreeSpace} из {drive.SelectedDrive.TotalSpace}";
        }

        private void OnDriveSelected(object sender, SelectedItemEventArgs e)   // выбрали диск
        {
            if (e.SelectedItem != null && e.SelectedItem is Drive)
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
                //((FolderViewModel)ContentViewModel).FileSelected += OnFileSelected;    // подключен обработчик выбора диска во втором окне
            }
        }
        public void OnDriveDoubleClicked(object sender, SelectedItemEventArgs e)  // по диску щелкнули два раза -> создаем вместо дисков каталоги, справа дочерние каталоги
        {
            if (e.SelectedItem != null && e.SelectedItem is Drive drive)
            {
                CreateNewFolderView(drive.Name, true, false);
                _previousSelectedIndexes.Push(SelectedIndex);
                MessageStatusBar = "";
            }
        }
        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ ПАПОК/ФАЙЛОВ
        public void OnFileDoubleClicked(object sender, SelectedItemEventArgs e)  // по папке/файлу щелкнули два раза или назад
        {
            if (e.SelectedItem != null)
            {
                Base myType = (Base)e.SelectedItem;

                Type type = myType.GetType();

                if (type == typeof(Folder))
                {
                    CreateNewFolderView(((Folder)e.SelectedItem).Path, true, false);
                    _selectedIndex = e.SelectedIndex;
                    _previousSelectedIndexes.Push(_selectedIndex);
                    return;
                }

                if (type == typeof(HexFile))
                {
                    
                    return;
                }

                if (type == typeof(TextFile))
                    return;

                if (type == typeof(ImageFile))
                    return;

                if (type == typeof(Back))
                {
                    if (Directory.GetParent(_currentDirectory) != null)
                    {
                        string currentDirectoryRoot = Directory.GetParent(_currentDirectory).FullName;
                        CreateNewFolderView(currentDirectoryRoot, true, true);
                        return;
                    }
                    else
                        LoadDriveView(true);
                }
            }
        }

        private void CreateNewFolderView(string path, bool isLeftPanelView, bool isBack)
        {
                                                                       #warning не очищается статусбар
            if (isBack)
            {
                CurrentViewModel = new FolderViewModel(path, isLeftPanelView, _previousSelectedIndexes.Pop(), true);   // текущая вью-модель переключается на папки
                _currentDirectory = Directory.GetParent(path)?.FullName ?? path;
            }
            else
            {
                CurrentViewModel = new FolderViewModel(path, isLeftPanelView, 0, false);
                if(isLeftPanelView)
                    _previousSelectedIndexes.Push(SelectedIndex);
                _currentDirectory = path;
            }
            ((FolderViewModel)CurrentViewModel).FileDoubleClicked += OnFileDoubleClicked;   // подключен обработчик двойного клика по диску
            ((FolderViewModel)CurrentViewModel).FileSelected += OnFileSelected;    // подключен обработчик выбора диска
            ((FolderViewModel)CurrentViewModel).Error += OnError;
        }

        public void OnFileSelected(object sender, SelectedItemEventArgs e)  // выбрали папку/файл
        {
            if (e.SelectedItem != null && e.SelectedItem is Folder folder)
            {
                folder = ((Folder)e.SelectedItem);
                _currentDirectory = Directory.GetParent(folder.Path).FullName;
  //              MessageStatusBar = $"Путь: {folder.Path}         Размер: {folder.Size}         Дата и время изменения: {folder.TimeCreated}";
                ContentViewModel = new FolderViewModel(folder.Path, false, 0, false);
            }

            if (e.SelectedItem != null && e.SelectedItem is HexFile)
            {
                ContentViewModel = new HexViewModel(((HexFile)e.SelectedItem).Path);            //  вывод НЕХ файла
                ((HexViewModel)ContentViewModel).Error += OnError;
            }

            if (e.SelectedItem != null && e.SelectedItem is ImageFile)
                ContentViewModel = new ImageViewModel(((ImageFile)e.SelectedItem).Path);            //  вывод картинки

            if (e.SelectedItem != null && e.SelectedItem is TextFile)
                ContentViewModel = new TextViewModel(((TextFile)e.SelectedItem).Path);            //  вывод картинки

            if (e.SelectedItem != null && e.SelectedItem is Back)
                ContentViewModel = new BackViewModel();            //  отображение пустой правой панели
        }

        #endregion

        public void OnError(object sender, MessageEventArgs e)  // ошибка открытия/чтения дисков, папок и файлов
        {
            if (e.SelectedIndex == -1)
                ImageStatusBar = new BitmapImage(new Uri("..\\Icons\\Warning.png", UriKind.Relative));
            else
                ImageStatusBar = null;
            MessageStatusBar = e.Message;
            _isError = true;
#warning по-ходу обработчик отцепляется при ошибке
        }
    }
}
