using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using RDN.Portable.Models.Json;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Adapters
{
    public class SkaterAdapter : BaseAdapter<SkaterJson>
    {
        List<SkaterJson> items;
        Activity context;
        List<Image> n_bitmapCache;
        Action MoreSkatersCallback;
        public SkaterAdapter(Activity context, List<SkaterJson> items, Action getMoreSkatersCallBack)
            : base()
        {
            this.context = context;
            this.items = items;
            this.MoreSkatersCallback = getMoreSkatersCallBack;
            n_bitmapCache = new List<Image>();
            LoggerMobile.Instance.logMessage("Opening SkaterAdapter", LoggerEnum.message);
        }


        public void AddItems(List<SkaterJson> items)
        {
            this.items.AddRange(items);
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override SkaterJson this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count(); }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //loads more skaters when 10 are left to view.
            if (position == (Count - 10))
                this.MoreSkatersCallback();

            var item = items[position];
            View view = convertView;
            try
            {
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.PublicSkaterRow, null);
                view.FindViewById<TextView>(Resource.Id.skaterName).Text = item.DerbyName;
                view.FindViewById<TextView>(Resource.Id.skaterNumber).Text = item.DerbyNumber;
                view.FindViewById<TextView>(Resource.Id.skaterGender).Text = item.Gender;
                var image = view.FindViewById<ImageView>(Resource.Id.skaterImageRow);
                image.SetImageResource(Resource.Drawable.skater);

                if (!String.IsNullOrEmpty(item.ThumbUrl) && !item.photoUrl.Contains("roller-girl.jpg") && !item.photoUrl.Contains("roller-person.gif"))
                {
                    var bit = n_bitmapCache.Where(x => x.Id == item.MemberId).FirstOrDefault();
                    if (bit == null)
                    {
                        Task<bool>.Factory.StartNew(
                                       () =>
                                       {
                                           try
                                           {
                                               var i = Image.GetImageBitmapFromUrl(item.ThumbUrl);
                                               LoggerMobile.Instance.logMessage("downloadingSkater " + item.ThumbUrl, LoggerEnum.message);
                                               Image img = new Image();
                                               img.Id = item.MemberId;
                                               img.Img = i;
                                               if (i != null && item.MemberId != null)
                                               {
                                                   context.RunOnUiThread(() =>
                                                   {
                                                       image.SetImageBitmap(i);
                                                   });
                                                   n_bitmapCache.Add(img);
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
                        image.SetImageBitmap(bit.Img);
                }
                else if (!String.IsNullOrEmpty(item.photoUrl) && !item.photoUrl.Contains("roller-girl.jpg") && !item.photoUrl.Contains("roller-person.gif"))
                {
                    var bit = n_bitmapCache.Where(x => x.Id == item.MemberId).FirstOrDefault();
                    if (bit == null)
                    {
                        Task<bool>.Factory.StartNew(
                                       () =>
                                       {
                                           try
                                           {
                                               var i = Image.GetImageBitmapFromUrl(item.photoUrl);
                                               LoggerMobile.Instance.logMessage("downloadingSkater " + item.photoUrl, LoggerEnum.message);
                                               Image img = new Image();
                                               img.Id = item.MemberId;
                                               img.Img = i;
                                               if (i != null && item.MemberId != null)
                                               {
                                                   context.RunOnUiThread(() =>
                                                   {
                                                       image.SetImageBitmap(i);
                                                   });
                                                   n_bitmapCache.Add(img);
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
                        image.SetImageBitmap(bit.Img);
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
            }
            return view;
        }
    }
}