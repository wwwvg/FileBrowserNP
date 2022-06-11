using System;

namespace FileBrowserNP.Models
{
    public class Folder : Base
    {
        public string Path { get; set; }
        public string Size { get; set; }
        public string TimeCreated { get; set; }
    }
}
