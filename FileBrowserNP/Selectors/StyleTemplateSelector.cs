using FileBrowserNP.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FileBrowserNP.Selectors
{
    public class StyleTemplateSelector : StyleSelector
    {
        public Style DriveStyle { get; set; }
        public Style FolderStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {

            Window window = Application.Current.MainWindow;
            Base myType = (Base)item;

            Type type = myType.GetType();
            if (type == typeof(Drive))
                return DriveStyle;

            return FolderStyle;
        }
    }
}
