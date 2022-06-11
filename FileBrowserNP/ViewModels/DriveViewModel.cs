using FileBrowserNP.Models;
using MainModule.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowserNP.ViewModels
{
    public class DriveViewModel: BindableBase
    {
        public DriveViewModel(ObservableCollection<Base> files)
        {
            Files = files;
            SetDrives();
        }
        private ObservableCollection<Base> _files; // список дисков, файлов и каталогов
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

                foreach (var drive in drives) //добавляем их в список
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
    }
}
