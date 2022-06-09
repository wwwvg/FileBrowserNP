using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowserNP.Models
{
    public class Drive : Base
    {
        public string Label { get; set; }
        public string FreeSpace { get; set; }
        public string TotalSpace { get; set; }
    }
}
