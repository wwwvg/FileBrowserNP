using FileBrowserNP.Commands;
using FileBrowserNP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Views.Dialogs
{
    public class AddFolderDialogViewModel : BindableBase
    {
        public AddFolderDialogViewModel(List<string> fileNames)
        {
            _listOfFiles = fileNames;
            SetWarningIcon(false);
        }

        #region СВОЙСТВА

        
        private ImageSource _headerImage;
        public ImageSource HeaderImage
        {
            get { return _headerImage; }
            set { SetProperty(ref _headerImage, value); }
        }

        private ImageSource _warningIcon = new BitmapImage(new Uri("Icons/Browser.png", UriKind.Relative));
        public ImageSource WarningIcon
        {
            get { return _warningIcon; }
            set { SetProperty(ref _warningIcon, value); }
        }

        private List<string> _listOfFiles = new();
        public List<string> ListOfFiles
        {
            get { return _listOfFiles; }
            set { SetProperty(ref _listOfFiles, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }

        private string _currentDirectory;
        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => SetProperty(ref _currentDirectory, value);
        }

        private bool _canAddFolder; // доступность кнопки Добавить папку
        public bool CanAddFolder
        {
            get => _canAddFolder;
            set => SetProperty(ref _canAddFolder, value);
        }

        private bool _isNameOfFolderWithoutMistakes = false;
        public bool IsNameOfFolderWithoutMistakes
        {
            get => _isNameOfFolderWithoutMistakes;
            set => SetProperty(ref _isNameOfFolderWithoutMistakes, value);
        }
        public string Title => "File Browser";

        private string _message = "Создать новый каталог (папку):";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _newFolderName;
        public string NewFolderName
        {
            get => _newFolderName;
            set => SetProperty(ref _newFolderName, value);
        }
        #endregion

        #region КОМАНДА ДОБАВЛЕНИЯ КАТАЛОГА И СООТВЕТСВУЮЩИЕ ОБРАБОТЧИКИ

        private DelegateCommand _addFolder;
        public DelegateCommand AddFolder => _addFolder ?? (_addFolder = new DelegateCommand(ExecuteAddFolder, CanExecuteAddFolder).ObservesProperty(() => CanAddFolder));
        public void ExecuteAddFolder()
        {

        }

        public bool CanExecuteAddFolder()
        {
            return CanAddFolder;
        }

        //void ChangeCanExecuteAddFolder(string currentDirectory)
        //{
        //    if (currentDirectory == null || currentDirectory == string.Empty)
        //    {
        //        CanAddFolder = false;
        //        return;
        //    }
        //    _currentDirectory = currentDirectory;
        //    CanAddFolder = true;
        //}

        private DelegateCommand _textChangedCommand;
        public DelegateCommand TextChangedCommand =>
            _textChangedCommand ?? (_textChangedCommand = new DelegateCommand(ExecuteTextChangedCommand));

        void ExecuteTextChangedCommand()
        {
            if (NewFolderName == null || NewFolderName.Trim() == "")
            {
                CanAddFolder = false;
                _addFolder.RaiseCanExecuteChanged();
                return;
            }

            string[] prohibitedChars = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            foreach (var symbol in prohibitedChars)
            if (NewFolderName.Contains(symbol))
                {
                    ErrorMessage = "Символы \\ / : * ? \" < > | не разрешены!";
                    SetWarningIcon(true);
                    CanAddFolder = false;
                    return;
                }



            if (_listOfFiles.Contains(Path.GetFileName(NewFolderName)))
            {
                ErrorMessage = "Такая папка уже существует!";
                SetWarningIcon(true);
                CanAddFolder = false;
                return;
            }

            CanAddFolder = true;
            ErrorMessage = "";
            SetWarningIcon(false);
            return;
        }

        private void SetWarningIcon(bool isError)
        {
            //if (isError)
            //    WarningIcon = new BitmapImage(new Uri("Icons/Warning.png", UriKind.Relative));
            //else
            //    WarningIcon = null;
        }

        private void CloseDialogAndCreateFolder()
        {
            
        }
        #endregion
    }
}
