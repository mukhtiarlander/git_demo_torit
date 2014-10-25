using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Drawing;

namespace RDNScoreboard.Code
{
    public class EnumStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is Enum)
                {
                    return RDN.Utilities.Enums.EnumExt.ToFreindlyName((Enum)value);
                }
            }
            catch
            { }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }



}
