using System;
using System.Collections.Generic;

namespace FileBrowserNP.Models.MyEventArgs
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
