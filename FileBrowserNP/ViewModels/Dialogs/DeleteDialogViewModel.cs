namespace FileBrowserNP.ViewModels.Dialogs
{
    public class DeleteDialogViewModel : BindableBase
    {
        public DeleteDialogViewModel(string fullPath)
        {
            Message = $"Вы уверены, что хотите удалить выбранный элемент:?";
            FileName = $"{fullPath}";
        }

        #region СВОЙСТВА
        public string Title => "Удалить";

        private string _textRecycleBin = "Переместить в корзину";
        public string TextRecycleBin { get => _textRecycleBin; set => SetProperty(ref _textRecycleBin, value); }


        private bool _moveToRecycleBin = true;
        public bool MoveToRecycleBin { get => _moveToRecycleBin; set => SetProperty(ref _moveToRecycleBin, value); }


        private string _message;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        private string _fileName;
        public string FileName { get => _fileName; set => SetProperty(ref _fileName, value); }
        #endregion

        #region КОМАНДЫ И ОБРАБОТЧИКИ
        //private DelegateCommand _cancelCommand;
        //public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(CloseDialog));

        //private DelegateCommand _deleteCommand;
        //public DelegateCommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new DelegateCommand(CloseDialogAndDelete));


        #endregion
    }
}
