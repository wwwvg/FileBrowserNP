using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileBrowserNP.Helpers
{
    public class ProhibitedSymbolsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            string[] prohibitedChars = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            foreach (var symbol in prohibitedChars)
                if (((string)value).Contains(symbol))
                    return false;

            return true;   
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
