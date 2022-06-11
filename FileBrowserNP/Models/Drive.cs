namespace FileBrowserNP.Models
{
    public class Drive : Base
    {
        public string Label { get; set; }
        public string FreeSpace { get; set; }
        public string TotalSpace { get; set; }

        public override string ToString()
        {
            return $"Доступно {FreeSpace} из {TotalSpace}";
        }
    }
}
