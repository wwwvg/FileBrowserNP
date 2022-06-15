using FileBrowserNP.Commands;
using FileBrowserNP.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using FileBrowserNP.Helpers;
using FileBrowserNP.Models.MyEventArgs;

namespace FileBrowserNP.ViewModels
{
    public class DriveViewModel: BindableBase
    {
        /// <summary>
        /// При запуске программы включается данная вью-модель отображающая диски. При выборе диска или ошибке в главную вью-модель
        /// передается соответствующее сообщение устанавлиываемое главной вью-моделью в статус бар. При двойном щелчке управление 
        /// передается обратно в главную вью-модель, которая далее отображает список каталогов и файлов.
        /// </summary>
        public DriveViewModel(int selectedIndex)
        {
            SetDrives();
            SelectedDrive = (Drive)Drives[(selectedIndex > -1 && selectedIndex < Drives.Count) ? selectedIndex : 0]; // выделяем диск
            SelectedIndex = selectedIndex;
            OnItemSelected();
        }

        #region СВОЙСТВА
        public ObservableCollection<Base> Drives { get; set; } = new();

        private Drive _selectedDrive;
        public Drive SelectedDrive
        {
            get { return _selectedDrive; }
            set { SetProperty(ref _selectedDrive, value); }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty(ref _selectedIndex, value); }
        }
        #endregion

        #region КОМАНДЫ
        private DelegateCommand _doubleClickedCommand;
        public DelegateCommand DoubleClickedCommand =>
            _doubleClickedCommand ?? (_doubleClickedCommand = new DelegateCommand(OnDoubleClickedCommand));

        private DelegateCommand _selectedCommand;
        public DelegateCommand SelectedCommand =>
            _selectedCommand ?? (_selectedCommand = new DelegateCommand(OnItemSelected));

        #endregion

        #region СОБЫТИЯ
        public event EventHandler<SelectedDriveEventArgs> DriveDoubleClicked;     // событие двойного клика

        public event EventHandler<SelectedDriveEventArgs> DriveSelected;             // событие выбранного элемента

        #endregion
                                                                                        #warning зачем передаешь индекс?
        #region ОБРАБОТЧИКИ И МЕТОДЫ
        private void OnItemSelected()  // выбрали элемент. главной вью-модели передается путь к файлу и выбранный индекс
        {
            DriveSelected?.Invoke(this, new SelectedDriveEventArgs(SelectedDrive, SelectedIndex));
        }
        private void OnDoubleClickedCommand()  // двойной щелчок. главной вьюмодели передается путь к файлу и выбранный индекс
        {
            DriveDoubleClicked?.Invoke(this, new SelectedDriveEventArgs(SelectedDrive, SelectedIndex));
        }
        private void SetDrives()
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives(); //получаем список дисков
                if (drives.Length == 0)
                    return;

                Drives.Clear();

                foreach (var drive in drives) //добавляем их в список
                {
                    Drives.Add(new Drive()
                    {
                        Name = drive.Name,
                        Label = drive.VolumeLabel,
                        FreeSpace = Bytes.SizeSuffix(drive.TotalFreeSpace),
                        TotalSpace = Bytes.SizeSuffix(drive.TotalSize)
                    });
                }
                
            }
            catch (Exception ex) // не все диски м.б. доступны (например - сетевой)
            {
                throw (ex);
            }
        }
        #endregion
    }
}
