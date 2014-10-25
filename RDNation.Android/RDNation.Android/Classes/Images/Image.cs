using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RDNation.Droid.Classes.Images
{
    public class Image
    {
        public Bitmap Img { get; set; }
        public string Id { get; set; }
        public DateTime Added { get; set; }
        public int position { get; set; }
        public static Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    if (imageBitmap != null)
                        imageBitmap = Bitmap.CreateScaledBitmap(imageBitmap, 120, 120, false);
                }
            }

            return imageBitmap;
        }
    }
}