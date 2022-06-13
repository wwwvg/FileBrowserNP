using System.Windows;
using System.Windows.Controls;

namespace Views.Dialogs
{
    public partial class AddFolderDialog : Window
    {
        public AddFolderDialog()
        {
            InitializeComponent();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
