using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowserNP.Helpers
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
