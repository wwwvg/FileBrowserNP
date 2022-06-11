using FileBrowserNP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowserNP.Helpers
{
    public class SelectedItemEventArgs : EventArgs
    {
        public SelectedItemEventArgs(Base selectedItem, int selectedIndex)
        {
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
        }

        public readonly Base SelectedItem;
        public readonly int SelectedIndex;
    }
}
