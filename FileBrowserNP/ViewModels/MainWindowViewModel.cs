using FileBrowserNP.Commands;
using FileBrowserNP.Models;
using FileBrowserNP.Helpers;
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

namespace FileBrowserNP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            LoadDriveView();    // при запуске программы отображаем список дисков                                        
            ((DriveViewModel)CurrentViewModel).DriveDoubleClicked += OnDriveDoubleClicked;   // подключен обработчик двойного клика по диску
            ((DriveViewModel)CurrentViewModel).ItemSelected += OnDriveSelected;    // подключен обработчик выбоа диска
            ((DriveViewModel)CurrentViewModel).Error += OnError;
        }

        #region СВОЙСТВА
       
        private BindableBase _currentViewModel;
        public BindableBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
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

        #region ОБРАБОТЧИКИ И МЕТОДЫ
        private void LoadDriveView()   // текущая вью-модель переключается на диски
        {
            CurrentViewModel = new DriveViewModel();      
        }
        public void OnError(object sender, MessageEventArgs e)  // ошибка открытия/чтения дисков, папок и файлов
        {
            ImageStatusBar = new BitmapImage(new Uri("..\\Icons\\Warning.png", UriKind.Relative));
            MessageStatusBar = e.Message;
        }
        public void OnDriveDoubleClicked(object sender, SelectedItemEventArgs e)  // по диску щелкнули два раза
        {

        }

        private void OnDriveSelected(object sender, SelectedItemEventArgs e)   // выбрали диск
        {
            if (e.SelectedItem != null && e.SelectedItem is Drive)
            {
                Drive drive = (Drive)e.SelectedItem;
                ImageStatusBar = null;
                MessageStatusBar = $"[{drive.Label}]  свободно {drive.FreeSpace} из {drive.TotalSpace}";
            }
        }

        private void OnLoadImageView()
        {

        }

        private void OnLoadHexView()
        {

        }

        private void OnLoadTextView()
        {

        }
        #endregion
    }
}
