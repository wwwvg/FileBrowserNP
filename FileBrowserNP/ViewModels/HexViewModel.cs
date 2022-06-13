using FileBrowserNP.Commands;
using FileBrowserNP.Helpers;
using FileBrowserNP.Models.MyEventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileBrowserNP.ViewModels
{
    public class HexViewModel : BindableBase
    {
        public HexViewModel(string fullPath)
        {
            _fullPath = fullPath;
            SetHeader();
            SetFirstLines();
        }

        private string _fullPath;

        private string _header;
        public string Header { get => _header; set => SetProperty(ref _header, value); } // 0 1 2 3 4 5 6 7 8 9 a b c d e f


        private string _text;
        public string Text { get => _text; set => SetProperty(ref _text, value); } // текст выводимый на экран

        #region УСТАНОВКА ЗАГОЛОВКА
        private void SetHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"          ");
            foreach (var item in GetYieldText.Digits)
                sb.Append($" 0{item}");

            Header = sb.ToString();
        }
        #endregion

        #region КОМАНДА СКРОЛЛА
        private DelegateCommand _mouseWheelCommand;
        public DelegateCommand MouseWheelCommand => _mouseWheelCommand ?? (_mouseWheelCommand = new DelegateCommand(ExecuteScrollChanged));
      
        void ExecuteScrollChanged()
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            while (_enumerator.MoveNext())
            {
                sb.AppendLine(_enumerator.Current);
                if (count++ > 100)
                    break;
            }
            Text += sb.ToString();
        }
        #endregion

        #region УСТАНОВКА ПЕРВЫХ 100 ЛИНИЙ
        public event EventHandler<MessageEventArgs> Error;
        private IEnumerator<string> _enumerator;
        private void SetFirstLines()
        {
            if (_enumerator != null)
                _enumerator.Dispose(); // освободили предыдущий enumerator

            if (_fullPath != null && File.Exists(_fullPath))
            {
                GetYieldText.CloseBinaryReader(); // освободили предыдущий ресурс
                try
                {
                    StringBuilder sb = new StringBuilder();
                    _enumerator = GetYieldText.GetHexLinesFromFile(_fullPath); // получаем первые 100 линий
                    int count = 0;
                    while (_enumerator.MoveNext())
                    {
                        sb.AppendLine(_enumerator.Current);  // добавляется очередная линия
                        if (count++ > 100)
                            break;
                    }
                    Text = sb.ToString();  // устанавливаюся первые 100 строк
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, new MessageEventArgs(ex.Message, -1));
                }
            }
        }
        #endregion
    }
}
