using System;

namespace FileBrowserNP.Models.MyEventArgs
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message, int selectedIndex)
        {
            Message = message;
            SelectedIndex = selectedIndex;
        }

        public readonly string Message;
        public readonly int SelectedIndex;
    }
}
