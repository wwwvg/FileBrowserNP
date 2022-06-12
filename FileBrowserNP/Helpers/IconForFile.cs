using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FileBrowserNP.Helpers
{
    public enum FileType
    {
        Back,
        Folder,
        Image,
        Text,
        Bin
    }
    public static class IconForFile
    {
        static Dictionary<FileType, BitmapImage> _icons = new Dictionary<FileType, BitmapImage>();

        static IconForFile()
        {
            _icons.Add(FileType.Back, new BitmapImage(new Uri("..\\Icons\\Back.png", UriKind.Relative)));
            _icons.Add(FileType.Folder, new BitmapImage(new Uri("..\\Icons\\Folder.png", UriKind.Relative)));
            _icons.Add(FileType.Image, new BitmapImage(new Uri("..\\Icons\\Image.png", UriKind.Relative)));
            _icons.Add(FileType.Text, new BitmapImage(new Uri("..\\Icons\\Text.png", UriKind.Relative)));
            _icons.Add(FileType.Bin, new BitmapImage(new Uri("..\\Icons\\Bin.png", UriKind.Relative)));
        }

        public static BitmapImage GetIconForFile(FileType fileType) => _icons[fileType];
    }
}
