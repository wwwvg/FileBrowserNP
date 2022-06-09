using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBrowserNP.Models
{
    public class Drive : Base
    {
        public int FreeSpace { get; set; }
        public int TotalSpace { get; set; }
    }
}
