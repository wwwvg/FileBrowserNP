﻿using System;
using System.Windows.Media.Imaging;

namespace FileBrowserNP.Models
{
    public class ImageFile : Base
    {
        public string Path { get; set; }
        public string Size { get; set; }
        public string TimeCreated { get; set; }
    }
}
