using FileBrowserNP.ViewModels.Dialogs;
using FileBrowserNP.ViewModels;
using System.Windows;
using FileBrowserNP.Views.Dialogs;

namespace FileBrowserNP
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RegisterDialogs();          
            var window = new MainWindow() { DataContext = new MainWindowViewModel() };
            window.Show();
        }

        private void RegisterDialogs()
        {
            DialogService.RegisterDialog<AddFolderDialog, AddFolderDialogViewModel>();
            DialogService.RegisterDialog<DeleteDialog, DeleteDialogViewModel>();
        }
    }
}
