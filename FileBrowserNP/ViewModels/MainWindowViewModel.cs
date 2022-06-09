using FileBrowserNP.Commands;
using FileBrowserNP.Models;
using MainModule.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileBrowserNP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            ICommand LoadHexView = new DelegateCommand(o => OnLoadHexView());
            ICommand LoadTextView = new DelegateCommand(o => OnLoadTextView());
            ICommand LoadImageView = new DelegateCommand(o => OnLoadImageView());
            SetDrives();
        }

        private ObservableCollection<Base> _files = new(); // список дисков, файлов и каталогов
        public ObservableCollection<Base> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        private void SetDrives()
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives(); //получаем список дисков
                if (drives.Length == 0)
                    return;

                _files.Clear();

                foreach (var drive in drives) //добавляем их в ComboBox
                {

                    Files.Add(new Drive()
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


        public ICommand LoadHexView { get; private set; }
        public ICommand LoadTextView { get; private set; }
        public ICommand LoadImageView { get; private set; }


        private BindableBase _currentViewModel;

        public BindableBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }


    }
}
