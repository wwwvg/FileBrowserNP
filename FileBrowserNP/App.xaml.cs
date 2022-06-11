﻿using FileBrowserNP.ViewModels;
using System.Windows;

namespace FileBrowserNP
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow() { DataContext = new MainWindowViewModel() };
            window.Show();
        }
    }
}
