﻿using System;
using System.Collections.Generic;

namespace FileBrowserNP.Models.MyEventArgs
{
    public class SelectedItemEventArgs : EventArgs
    {
        public SelectedItemEventArgs(Base selectedItem, int selectedIndex, List<string> files = null)
        {
            SelectedItem = selectedItem;
            SelectedIndex = selectedIndex;
            Files = files;
        }

        public readonly Base SelectedItem;
        public readonly int SelectedIndex;
        public List<string> Files;
    }
}
