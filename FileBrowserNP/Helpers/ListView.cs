using System.Windows;

namespace FileBrowserNP.Helpers
{
    public class ListView : System.Windows.Controls.ListView  // переопределяется ListView для корректного выставления фокуса
    {
        // используется в FileView.xaml, как:
        // xmlns:local="clr-namespace:FileBrowserNP.Helpers"
        // ...
        //<local:ListView ItemsSource="{Binding Files}" Grid.Row="1" Margin="5"
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListViewItem();  // ListViewItem - также переопределяется (собственно там вся логика). Файл ListView.cs
        }
    }
}
