using FileBrowserNP.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileBrowserNP.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            ICommand LoadHexView = new DelegateCommand(o => OnLoadHexView());
            ICommand LoadTextView = new DelegateCommand(o => OnLoadTextView());
            ICommand LoadImageView = new DelegateCommand(o => OnLoadImageView());
        }
        private void OnLoadImageView()
        {

        }


        private void OnLoadHexView()
        {

        }


        private void OnLoadTextView()
        {

        }


        public ICommand LoadHexView { get; private set; }
        public ICommand LoadTextView { get; private set; }
        public ICommand LoadImageView { get; private set; }


        private BindableBase _currentViewModel;

        public BindableBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }


    }
}
