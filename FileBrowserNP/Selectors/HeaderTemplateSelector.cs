using FileBrowserNP.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FileBrowserNP.Selectors
{
    public class HeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DriveHeaderTemplate { get; set; }
        public DataTemplate FileHeaderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return DriveHeaderTemplate;

            Base product = (Base)item;
  
            Type type = product.GetType();
            if (type == typeof(Drive))
                return DriveHeaderTemplate;

            return FileHeaderTemplate;
        }
    }
}
