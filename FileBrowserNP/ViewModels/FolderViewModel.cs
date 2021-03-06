using FileBrowserNP.Commands;
using FileBrowserNP.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using FileBrowserNP.Helpers;
using FileBrowserNP.Models.MyEventArgs;
using System.Linq;
using System.Collections.Generic;

namespace FileBrowserNP.ViewModels
{
    public class FolderViewModel : BindableBase
    {
        /// <summary>
        /// 
        /// </summary>
        public FolderViewModel(string path, bool isLeftPanelView, int selectedIndex, bool isBack)
        {
            _isLeftPanelView = isLeftPanelView;
            SetFoldersAndFiles(path, !isBack); // путь для создания подпапок и флаг: нужно ли создавать кнопку назад

            int index = (selectedIndex > -1 && selectedIndex < Files.Count) ? selectedIndex : 0;
            if (isLeftPanelView && Files.Count > 0)
                SelectedFile = Files[selectedIndex];
        }

        #region СВОЙСТВА
        private ObservableCollection<Base> _files = new();  
        public ObservableCollection<Base> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }

        private Base _selectedFile;
        public Base SelectedFile
        {
            get { return _selectedFile; }
            set { SetProperty(ref _selectedFile, value); }
        }

        private List<Base> _selectedFiles;
        public List<Base> SelectedFiles
        {
            get { return _selectedFiles; }
            set { SetProperty(ref _selectedFiles, value); }
        }
        

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty(ref _selectedIndex, value); }
        }

        private bool _isError = false; // для своевременного удаления сообщения об ошибке

        private bool _isLeftPanelView;
        public bool IsLeftPanelView
        {
            get { return _isLeftPanelView; }
            set { SetProperty(ref _isLeftPanelView, value); }
        }
        #endregion

        #region КОМАНДЫ
        private DelegateCommand _doubleClickedCommand;
        public DelegateCommand DoubleClickedCommand =>  _doubleClickedCommand ?? (_doubleClickedCommand = new DelegateCommand(OnDoubleClickedCommand));

        private DelegateCommand _selectedCommand;
        public DelegateCommand SelectedCommand =>  _selectedCommand ?? (_selectedCommand = new DelegateCommand(OnItemSelected));

        #endregion

        #region СОБЫТИЯ
        public event EventHandler<SelectedItemEventArgs> ItemDoubleClicked;     // событие двойного клика

        public event EventHandler<SelectedItemEventArgs> ItemSelected;             // событие выбранного элемента

        #endregion
                                                                                                
        #region ОБРАБОТЧИКИ И МЕТОДЫ
        private void OnItemSelected()  // выбрали элемент. главной вью-модели передается путь к файлу и выбранный индекс
        {
            ItemSelected?.Invoke(this, new SelectedItemEventArgs(SelectedFile, SelectedIndex));
        }
        private void OnDoubleClickedCommand()  // двойной щелчок. главной вьюмодели передается путь к файлу и выбранный индекс
        {
            ItemDoubleClicked?.Invoke(this, new SelectedItemEventArgs(SelectedFile, SelectedIndex)); // индекс передается для записи предыдущих выбранных индексов
        }

        public void SetSelectedItem(int index)
        {
            if(Files.Count > 0)
                SelectedFile = Files[index];
        }

        private void SetFoldersAndFiles(string path, bool isForwardToSubFolders) // добавление папок и файлов
        {
            Files.Clear();
            DirectoryInfo dir = new DirectoryInfo($"{path}");
            try
            {
                DirectoryInfo[] directories = dir.GetDirectories();

                if(_isLeftPanelView)
                    Files.Add(new Back{ Path = path });

                foreach (var item in directories)
                {
                    _isError = false;
                    if (item.Attributes == FileAttributes.Hidden || item.Attributes == FileAttributes.System)
                        continue;
                    Files.Add(new Folder { Path = item.FullName, Name = $"[{item.Name}]", Size = "<Папка>", TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }

                FileInfo[] files = dir.GetFiles();
                foreach (var item in files)
                {
                    _isError = false;

                    string name = item.Name; 
                    string fullPath = item.FullName; 
                    string size = Bytes.SizeSuffix(item.Length); 
                    string time = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm");

                    if (item.Extension == ".png" || item.Extension == ".bmp" || item.Extension == ".jpg" || item.Extension == ".gif")
                        Files.Add(new ImageFile { Name = name, Path = fullPath, Size = size, TimeCreated = time });
                   
                    else if (item.Extension == ".txt" || item.Extension == ".cfg" || item.Extension == ".ini" || item.Extension == ".log" || item.Extension == ".csv" || item.Extension == ".xml")
                        Files.Add(new TextFile { Name = name, Path = fullPath, Size = size, TimeCreated = time });
                   
                    else
                        Files.Add(new HexFile { Name = name, Path = fullPath, Size = size, TimeCreated = time });
                }
            }

            catch (Exception ex) // некоторые системные папки и файлы недоступны, но если запустить программу с админскими привилегиями то все ОК.
            {
                if(IsLeftPanelView)
                    Files.Add(new Back());
                throw(ex);
            }
        }
        #endregion
    }
}
