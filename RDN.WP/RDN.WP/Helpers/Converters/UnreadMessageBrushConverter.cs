using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace RDN.WP.Helpers.Converters
{
    public class UnreadMessageBrushConverter : IValueConverter
    {
        public string UnreadMessageColorHex { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // hard-coded colours example,  you may want to look at 
            // using predefined resources for this, though.
            if (!(bool)value)
            {
                return RDN.WP.Library.Classes.Media.Colors.GetColorFromHexa(UnreadMessageColorHex);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
