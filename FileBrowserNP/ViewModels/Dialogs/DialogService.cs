using FileBrowserNP.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FileBrowserNP.ViewModels.Dialogs
{
    public interface IDialogService
    {
        void ShowDialog<ViewModel>(BindableBase viewModel, Action<bool?> callback);
    }


    public class DialogService : IDialogService
    {
        static Dictionary<Type, Type> _mapping = new Dictionary<Type, Type>();
        public static string _output = string.Empty;

        public static void RegisterDialog<TView, TViewModel>()
        {
            _mapping.Add(typeof(TViewModel), typeof(TView));   // прописать на уровне приложения -> DialogService.RegisterDialog<Имя View, Имя ViewModel>();
        }

        public void ShowDialog<TViewModel>(BindableBase viewModel, Action<bool?> callback)
        {
            var type = _mapping[typeof(TViewModel)];
            ShowDialogInternal(type, viewModel, callback);
        }

        private static void ShowDialogInternal(Type type, BindableBase viewModel, Action<bool?> callback)
        {
            var dialog = Activator.CreateInstance(type);

            EventHandler closeEventHandler = null;
            closeEventHandler = (s, e) =>
            {
                callback((dialog as Window).DialogResult);
                (dialog as Window).Closed -= closeEventHandler;  // устранение утечек памяти 
            };
            (dialog as Window).Closed += closeEventHandler;

            (dialog as Window).DataContext = viewModel;
            (dialog as Window).ShowDialog();
        }
    }
}
