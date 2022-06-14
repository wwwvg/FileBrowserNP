using System.Collections.Generic;

namespace FileBrowserNP.Models.MyEventArgs
{
    /// <summary>
    /// Когда список дисков или файлов/каталогов заполняется, этот список передается в главную вью-модель
    /// </summary>

    public class ItemsAddedEventArgs
    {
        public ItemsAddedEventArgs(IEnumerable<Base> items)
        {
            Items = items;
        }
        public readonly IEnumerable<Base> Items;
    }
}
