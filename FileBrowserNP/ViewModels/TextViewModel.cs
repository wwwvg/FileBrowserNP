using FileBrowserNP.Commands;
using FileBrowserNP.Helpers;
using FileBrowserNP.Models.MyEventArgs;
using FileBrowserNP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileBrowserNP.ViewModels
{
    public class TextViewModel : BindableBase
    {
        public TextViewModel(string fullPath)
        {
            _fullPath = fullPath;
            SetFirstLines();
        }

        #region СВОЙСТВА
        private string _fullPath;


        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
        #endregion

        #region КОМАНДА MouseWheel
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
        public void SetFirstLines()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose(); // освободили предыдущий enumerator
            }
            if (_fullPath != null && File.Exists(_fullPath))
            {
                GetYieldText.CloseStreamReader(); // освободили предыдущий ресурс
                try
                {
                    StringBuilder sb = new StringBuilder();
                    _enumerator = GetYieldText.GetLinesFromFile(_fullPath); // получаем первые 100 линий
                    int count = 0;
                    while (_enumerator.MoveNext())
                    {
                        sb.AppendLine(_enumerator.Current);  // добавляется очередная линия
                        if (count++ > 100)
                            break;
                    }
                    Text = sb.ToString();  // устанавливаются первые 100 строк
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
