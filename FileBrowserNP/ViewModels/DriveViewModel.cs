using FileBrowserNP.Commands;
using FileBrowserNP.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using MainModule.Helpers;
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
        public DriveViewModel()
        {
            SetDrives();
            SelectedDrive = (Drive)Drives[0];
        }

        #region СВОЙСТВА
        public ObservableCollection<Base> Drives { get; set; } = new();
        //public Drive SelectedDrive { get; set; }

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
            _doubleClickedCommand ?? (_doubleClickedCommand = new DelegateCommand(o => OnDoubleClickedCommand()));

        private DelegateCommand _selectedCommand;
        public DelegateCommand SelectedCommand =>
            _selectedCommand ?? (_selectedCommand = new DelegateCommand(o => OnItemSelected()));

        #endregion

        #region СОБЫТИЯ
        public event EventHandler<SelectedItemEventArgs> DriveDoubleClicked;     // событие двойного клика

        public event EventHandler<SelectedItemEventArgs> ItemSelected;             // событие выбранного элемента

        public event EventHandler<MessageEventArgs> Error;

        #endregion
                                                                                        #warning зачем передаешь индекс?
        #region ОБРАБОТЧИКИ И МЕТОДЫ
        private void OnItemSelected()  // выбрали элемент. главной вью-модели передается путь к файлу и выбранный индекс
        {
            ItemSelected?.Invoke(this, new SelectedItemEventArgs(SelectedDrive, SelectedIndex));
        }
        private void OnDoubleClickedCommand()  // двойной щелчок. главной вьюмодели передается путь к файлу и выбранный индекс
        {
            DriveDoubleClicked?.Invoke(this, new SelectedItemEventArgs(SelectedDrive, SelectedIndex));
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
                Error?.Invoke(this, new MessageEventArgs(ex.Message, SelectedIndex));
            }
        }
        #endregion
    }
}
