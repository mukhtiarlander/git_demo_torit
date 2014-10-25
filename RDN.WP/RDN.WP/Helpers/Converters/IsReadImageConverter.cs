using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RDN.WP.Helpers.Converters
{
    public class IsReadImageConverter : IValueConverter
    {
        public Image ReadImage { get; set; }
        public string ReadImageSource { get; set; }
        public Image UnreadImage { get; set; }
        public string UnreadImageSource { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (ReadImage == null)
            {
                ReadImage = new Image();
                ReadImage.Source = new BitmapImage(new Uri(ReadImageSource, UriKind.RelativeOrAbsolute));
            }
            if (UnreadImage == null)
            {
                UnreadImage = new Image();
                UnreadImage.Source = new BitmapImage(new Uri(UnreadImageSource, UriKind.RelativeOrAbsolute));
            }
            if (!(value is bool))
            {
                return null;
            }
            bool b = (bool)value;
            if (b)
            {
                return this.ReadImage;
            }
            else
            {
                return this.UnreadImage;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
