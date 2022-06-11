using FileBrowserNP.Commands;
using FileBrowserNP.Models;
using FileBrowserNP.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using MainModule.Helpers;

namespace FileBrowserNP.ViewModels
{
    public class FolderViewModel : BindableBase
    {
        /// <summary>
        /// 
        /// </summary>
        public FolderViewModel(string path)
        {
            SetFoldersAndFiles(path);
        }

        #region СВОЙСТВА
        private ObservableCollection<Base> _files = new();  
        public ObservableCollection<Base> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }

        public Base SelectedFile { get; set; }
        public int SelectedIndex { get; set; }

        private bool _isError = false; // для своевременного удаления сообщения об ошибке
        #endregion

        #region КОМАНДЫ
        private DelegateCommand _doubleClickedCommand;
        public DelegateCommand DoubleClickedCommand =>
            _doubleClickedCommand ?? (_doubleClickedCommand = new DelegateCommand(o => OnDoubleClickedCommand()));

        private DelegateCommand _selectedCommand;
        public DelegateCommand SelectedCommand =>
            _selectedCommand ?? (_selectedCommand = new DelegateCommand(o => OnItemSelected()));

        #endregion

        #region СОБЫТИЯ
        public event EventHandler<SelectedItemEventArgs> FileDoubleClicked;     // событие двойного клика

        public event EventHandler<SelectedItemEventArgs> FileSelected;             // событие выбранного элемента

        public event EventHandler<MessageEventArgs> Error;

  //      public event EventHandler<MessageEventArgs> BackClicked;        // событие перехода на верхний уровень

        #endregion

        #region ОБРАБОТЧИКИ И МЕТОДЫ
        private void OnItemSelected()  // выбрали элемент. главной вьюмодели передается путь к файлу и выбранный индекс
        {
            FileSelected?.Invoke(this, new SelectedItemEventArgs(SelectedFile, SelectedIndex));
        }
        private void OnDoubleClickedCommand()  // двойной щелчок. главной вьюмодели передается путь к файлу и выбранный индекс
        {
            FileDoubleClicked?.Invoke(this, new SelectedItemEventArgs(SelectedFile, SelectedIndex));
        }


        private void SetFoldersAndFiles(string path) // добавление папок и файлов
        {
            Files.Clear();
            DirectoryInfo dir = new DirectoryInfo($"{path}");
            try
            {
                DirectoryInfo[] directories = dir.GetDirectories();

                Files.Add(new Back());

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
                SelectedIndex = 0;
            }

            catch (Exception ex) // некоторые системные папки и файлы недоступны, но если запустить программу с админскими привилегиями то все ОК.
            {
                _isError = true;

            }
        }
        #endregion
    }
}
