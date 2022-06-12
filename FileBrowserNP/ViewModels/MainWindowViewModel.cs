using FileBrowserNP.Commands;
using FileBrowserNP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FileBrowserNP.Models.MyEventArgs;

namespace FileBrowserNP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            LoadDriveView(0);    // при запуске программы отображаем список дисков                                        
        }

        #region СВОЙСТВА

        private string _currentDirectory = "";
        private Stack<int> _previousSelectedIndexes = new();
        
        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty(ref _selectedIndex, value); }
        }

        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        private BindableBase _contentViewModel;
        public BindableBase ContentViewModel
        {
            get => _contentViewModel;
            set => SetProperty(ref _contentViewModel, value);
        }

        private ObservableCollection<Base> _files = new(); // список дисков, файлов и каталогов
        public ObservableCollection<Base> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        private Base _selectedFile = new(); // выбранный элемент
        public Base SelectedFile
        {
            get { return _selectedFile; }
            set { SetProperty(ref _selectedFile, value); }
        }

        private ImageSource _imageStatusBar;  // значок предупреждения в статусбаре
        public ImageSource ImageStatusBar
        {
            get { return _imageStatusBar; }
            set { SetProperty(ref _imageStatusBar, value); }
        }

        private string _messageStatusBar;  // информация о дисках, папках, файлов и ошибок в статусбаре
        public string MessageStatusBar
        {
            get { return _messageStatusBar; }
            set { SetProperty(ref _messageStatusBar, value); }
        }
        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ ДИСКОВ
        private void LoadDriveView(int selectedIndex)   // текущая вью-модель переключается на диски
        {  
            CurrentViewModel = new DriveViewModel();
            ((DriveViewModel)CurrentViewModel).DriveDoubleClicked += OnDriveDoubleClicked;   // подключен обработчик двойного клика по диску
            ((DriveViewModel)CurrentViewModel).ItemSelected += OnDriveSelected;    // подключен обработчик выбора диска
            ((DriveViewModel)CurrentViewModel).Error += OnError;
            SetSelection(0, false);
        }

        void SetSelection(int index, bool isBack)
        {
            if (index > -1 && index < Files.Count)
                SelectedIndex = index;
        }
        public void OnError(object sender, MessageEventArgs e)  // ошибка открытия/чтения дисков, папок и файлов
        {
            ImageStatusBar = new BitmapImage(new Uri("..\\Icons\\Warning.png", UriKind.Relative));
            MessageStatusBar = e.Message;
#warning по-ходу обработчик отцепляется при ошибке
        }

        private void OnDriveSelected(object sender, SelectedItemEventArgs e)   // выбрали диск
        {
            if (e.SelectedItem != null && e.SelectedItem is Drive)
            {
                Drive drive = (Drive)e.SelectedItem;
                SelectedIndex = (int)e.SelectedIndex;
                ImageStatusBar = null;
                MessageStatusBar = $"[{drive.Label}]  свободно {drive.FreeSpace} из {drive.TotalSpace}";
                _currentDirectory = drive.Name;

                ContentViewModel = new FolderViewModel(_currentDirectory, false);
                //((FolderViewModel)ContentViewModel).FileSelected += OnFileSelected;    // подключен обработчик выбора диска во втором окне
            }
        }
        public void OnDriveDoubleClicked(object sender, SelectedItemEventArgs e)  // по диску щелкнули два раза -> создаем вместо дисков каталоги, справа дочерние каталоги
        {
            if (e.SelectedItem != null && e.SelectedItem is Drive drive)
            {
                CreateNewFolderView(drive.Name, true);
                _previousSelectedIndexes.Push(e.SelectedIndex);
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

                if (type == typeof(Drive))
                {
                    CreateNewFolderView(_currentDirectory, true);
                    _previousSelectedIndexes.Push(_selectedIndex);
                    return;
                }

                if (type == typeof(Folder))
                {
                    CreateNewFolderView(((Folder)e.SelectedItem).Path, true);
                    _previousSelectedIndexes.Push(_selectedIndex);
                    return;
                }

                if (type == typeof(HexFile))
                    return;

                if (type == typeof(TextFile))
                    return;

                if (type == typeof(ImageFile))
                    return;

                if (type == typeof(Back))
                {
                    //SelectedIndex = _previousSelectedIndexes.Pop();
                    if (Directory.GetParent(_currentDirectory) != null)
                    {
                        string currentDirectoryRoot = Directory.GetParent(_currentDirectory).FullName;
                        CreateNewFolderView(currentDirectoryRoot, true);
                        return;
                    }
                    else
                    {
                        if (_previousSelectedIndexes.TryPop(out int selectedIndex) && selectedIndex < Files.Count)
                            LoadDriveView(selectedIndex);
                        else
                            LoadDriveView(0);
                    }
                }
                    
            }
        }

        private void CreateNewFolderView(string path, bool isFirstView)
        {
                                                                        #warning не очищается статусбар
            MessageStatusBar = "";
            CurrentViewModel = new FolderViewModel(path, isFirstView);   // текущая вью-модель переключается на папки
            ((FolderViewModel)CurrentViewModel).FileDoubleClicked += OnFileDoubleClicked;   // подключен обработчик двойного клика по диску
            ((FolderViewModel)CurrentViewModel).FileSelected += OnFileSelected;    // подключен обработчик выбора диска
            ((FolderViewModel)CurrentViewModel).Error += OnError;
        }

        public void OnFileSelected(object sender, SelectedItemEventArgs e)  // выбрали папку/файл
        {
            if (e.SelectedItem != null && e.SelectedItem is Folder folder)
            {
                folder = ((Folder)e.SelectedItem);
                //string dir = folder.Path;
                _currentDirectory = Directory.GetParent(folder.Path).FullName;
                MessageStatusBar = $"Путь: {folder.Path}         Размер: {folder.Size}         Дата и время изменения: {folder.TimeCreated}";
                ContentViewModel = new FolderViewModel(folder.Path, false);
               // ((FolderViewModel)ContentViewModel).IsFirstView = true;
            }
        }

        #endregion

    }
}
