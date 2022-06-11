using FileBrowserNP.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FileBrowserNP.Selectors
{
    public class FileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DriveTemplate { get; set; }
        public DataTemplate FolderTemplate { get; set; }
        public DataTemplate HexTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate BackTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item,
        DependencyObject container)
        {
            Base product = (Base)item;

            Type type = product.GetType();
            if (type == typeof(Drive))
                return DriveTemplate;

            if (type == typeof(Folder))
                return FolderTemplate;

            if (type == typeof(HexFile))
                return HexTemplate;

            if (type == typeof(TextFile))
                return TextTemplate;

            if (type == typeof(ImageFile))
                return ImageTemplate;

            return BackTemplate;

        }
    }
}
