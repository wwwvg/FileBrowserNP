using FileBrowserNP.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FileBrowserNP.Selectors
{
    public class FileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DriveTemplate { get; set; }
        public DataTemplate FileTemplate { get; set; }
        public DataTemplate HexFileTemplate { get; set; }
        public DataTemplate TextFileTemplate { get; set; }
        public DataTemplate ImageFileTemplate { get; set; }
        public DataTemplate BackTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item,
        DependencyObject container)
        {
            Base product = (Base)item;

            Type type = product.GetType();
            if (type == typeof(Drive))
                return DriveTemplate;

            if (type == typeof(Folder))
                return FileTemplate;

            if (type == typeof(HexFile))
                return HexFileTemplate;

            if (type == typeof(TextFile))
                return TextFileTemplate;

            if (type == typeof(ImageFile))
                return ImageFileTemplate;

            return BackTemplate;

        }

        //public string PropertyToEvaluate
        //{
        //    get; set;
        //}
        //public string PropertyValueToHighlight
        //{
        //    get; set;
        //}
    }
}
