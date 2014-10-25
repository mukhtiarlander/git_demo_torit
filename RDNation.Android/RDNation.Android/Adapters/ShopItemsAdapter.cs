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
using RDN.Portable.Models.Json.Shop;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Adapters
{
    public class ShopItemsAdapter : BaseAdapter<ShopItemJson>
    {

        private class ViewHolder : Java.Lang.Object
        {
            public TextView publicShopRowName;
            public TextView publicShopRowMadeBy;
            public TextView publicShopRowPrice;
            public ImageView publicShopRowImage;
        }
        List<ShopItemJson> items;
        Activity context;
        List<Image> n_bitmapCache;
        Action MoreCallback;
        public ShopItemsAdapter(Activity context, List<ShopItemJson> items, Action getMoreCallback)
            : base()
        {
            this.context = context;
            this.items = items;
            this.MoreCallback = getMoreCallback;
            n_bitmapCache = new List<Image>();
            LoggerMobile.Instance.logMessage("Opening ShopItemAdapter", LoggerEnum.message);
        }


        public void AddItems(List<ShopItemJson> items)
        {
            this.items.AddRange(items);
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override ShopItemJson this[int position]
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
            if (position == (Count - 5) && MoreCallback != null)
                this.MoreCallback();

            var item = items[position];

            ViewHolder holder = null;
            try
            {
                if (convertView == null) // no view to re-use, create new
                {
                    convertView = context.LayoutInflater.Inflate(Resource.Layout.PublicShopRow, null);
                    holder = new ViewHolder();
                    holder.publicShopRowName = convertView.FindViewById<TextView>(Resource.Id.publicShopRowName);
                    holder.publicShopRowMadeBy = convertView.FindViewById<TextView>(Resource.Id.publicShopRowMadeBy);
                    holder.publicShopRowPrice = convertView.FindViewById<TextView>(Resource.Id.publicShopRowPrice);
                    holder.publicShopRowImage = convertView.FindViewById<ImageView>(Resource.Id.publicShopRowImage);
                    convertView.Tag = holder;
                }
                if (holder == null)
                    holder = (ViewHolder)convertView.Tag;


                holder.publicShopRowName.Text = item.Name;
                holder.publicShopRowMadeBy.Text = item.SoldBy;
                holder.publicShopRowPrice.Text = item.Price.ToString();
                holder.publicShopRowImage.SetImageBitmap(null);

                if (item.PhotoUrlsThumbs.FirstOrDefault() != null)
                {
                    if (!String.IsNullOrEmpty(item.PhotoUrlsThumbs.FirstOrDefault()))
                    {
                        var pic = item.PhotoUrlsThumbs.FirstOrDefault();
                        var bit = n_bitmapCache.Where(x => x.Id == pic).FirstOrDefault();
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
                                                       n_bitmapCache.Add(img);
                                                       context.RunOnUiThread(() =>
                                                       {
                                                           holder.publicShopRowImage.SetImageBitmap(i);
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
                            holder.publicShopRowImage.SetImageBitmap(bit.Img);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(item.PhotoUrls.FirstOrDefault()))
                    {
                        var pic = item.PhotoUrls.FirstOrDefault();
                        var bit = n_bitmapCache.Where(x => x.Id == pic).FirstOrDefault();
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
                                                       n_bitmapCache.Add(img);
                                                       context.RunOnUiThread(() =>
                                                       {
                                                           holder.publicShopRowImage.SetImageBitmap(i);
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
                            holder.publicShopRowImage.SetImageBitmap(bit.Img);
                    }
                }



            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
            }
            return convertView;
        }
    }
}