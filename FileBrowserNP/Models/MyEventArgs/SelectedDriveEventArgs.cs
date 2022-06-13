using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowserNP.Models.MyEventArgs
{
    public class SelectedDriveEventArgs : EventArgs
    {
        public SelectedDriveEventArgs(Base selectedItem, int selectedIndex)
        {
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
        }

        public readonly Base SelectedItem;
        public readonly int SelectedIndex;
    }
}
