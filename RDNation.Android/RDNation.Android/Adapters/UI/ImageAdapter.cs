using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RDN.Utilities.Util;
using RDNation.Droid.Classes;
using RDNation.Droid.Classes.Images;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Adapters.UI
{
    public class ImageAdapter : BaseAdapter
    {
        Activity context;
        List<Image> bitmaps = new List<Image>();
        List<string> imageUrls = new List<string>();

        public ImageAdapter(Activity c, List<string> urls)
        {
            context = c;
            imageUrls = urls;
            LoggerMobile.Instance.logMessage("Opening ImageAdapter", LoggerEnum.message);
        }

        public override int Count { get { return imageUrls.Count; } }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        // create a new ImageView for each item referenced by the Adapter
        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            ImageView iHold = new ImageView(context);
            //make sure we don't get an index out of bounds exception
            if (imageUrls.Count > position)
            {
                string pic = imageUrls[position];
                var bit = bitmaps.Where(x => x.Id == pic).FirstOrDefault();
                if (!String.IsNullOrEmpty(pic))
                {
                    if (bit == null)
                    {
                        Task<bool>.Factory.StartNew(
                                       () =>
                                       {
                                           try
                                           {
                                               LoggerMobile.Instance.logMessage("downloading:" + pic, LoggerEnum.message);
                                               var i = Image.GetImageBitmapFromUrl(pic);
                                               Image img = new Image();
                                               img.Id = pic;
                                               img.Img = i;
                                               if (i != null)
                                               {
                                                   bitmaps.Add(img);
                                                   context.RunOnUiThread(() =>
                                                   {
                                                       iHold.SetImageBitmap(i);

                                                       iHold.LayoutParameters = new Gallery.LayoutParams(300, 300);
                                                       iHold.SetScaleType(ImageView.ScaleType.FitXy);
                                                   });
                                               }
                                           }
                                           catch (Exception exception)
                                           {
                                               ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
                                           }
                                           return true;
                                       });
                    }
                    else
                        iHold.SetImageBitmap(bit.Img);
                }
            }

            return iHold;
        }

    }
}