using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RDN.WP.Helpers
{
    public class ImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is string)
                {
                    value = new Uri((string)value);
                }

                if (value is Uri)
                {
                    BitmapImage bi = new BitmapImage();
                    bi.UriSource = new Uri(value.ToString());
                    //bi.DownloadFailed +=new EventHandler<ExceptionEventArgs>(bi_DownloadFailed); 
                    bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    return bi;
                }
            }
            catch
            {

            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

}
