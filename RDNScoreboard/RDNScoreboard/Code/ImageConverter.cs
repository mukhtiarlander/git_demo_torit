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
                    bi.BeginInit();
                    bi.UriSource = new Uri(value.ToString());
                    //bi.DownloadFailed +=new EventHandler<ExceptionEventArgs>(bi_DownloadFailed); 
                    bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
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

    /// <summary>
    /// had to use a diffrent image converter for the logos menu because it would only load team 1 and not team two
    /// with the bi.createoptions set to igonore cache
    /// </summary>
    public class ImageConverterForLogos : IValueConverter
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
                    BitmapImage _internetImage = new BitmapImage();
                    _internetImage.BeginInit();
                    _internetImage.DecodePixelWidth = 200;
                    _internetImage.UriSource = new Uri(value.ToString());
                    //bi.DownloadFailed +=new EventHandler<ExceptionEventArgs>(bi_DownloadFailed); 
                    // _internetImage.DownloadCompleted += new EventHandler(bi_DownloadCompleted);
                    _internetImage.CacheOption = BitmapCacheOption.OnLoad;
                    _internetImage.EndInit();

                    return _internetImage;
                }
            }
            catch
            {

            }
            return null;
        }

        void bi_DownloadCompleted(object sender, EventArgs e)
        {
            //tempLoadingImage.UriSource = new Uri( sender.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
