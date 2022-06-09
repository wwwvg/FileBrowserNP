﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.Helpers
{
    /// <summary>
    /// Конвертирует байты в килобайты, мегабайты, гигабайты ...
    /// </summary>
    public static class Bytes
    {
        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            string[] SizeSuffixes =
                          { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }
    }
}
