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
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Adapters
{
    public class EventsAdapter : BaseAdapter<EventJson>
    {

        private class ViewHolder : Java.Lang.Object
        {
            public TextView publicEventRowName;
            public TextView publicEventRowDetails;
            public ImageView publicEventRowImage;
        }
        List<EventJson> items;
        Activity context;
        List<Image> n_bitmapCache;
        Action MoreCallback;
        public EventsAdapter(Activity context, List<EventJson> items, Action getMoreCallback)
            : base()
        {
            this.context = context;
            this.items = items;
            this.MoreCallback = getMoreCallback;
            n_bitmapCache = new List<Image>();
            LoggerMobile.Instance.logMessage("Opening EventsAdapter", LoggerEnum.message);
        }


        public void AddItems(List<EventJson> items)
        {
            this.items.AddRange(items);
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override EventJson this[int position]
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
                    convertView = context.LayoutInflater.Inflate(Resource.Layout.PublicEventRow, null);
                    holder = new ViewHolder();
                    holder.publicEventRowName = convertView.FindViewById<TextView>(Resource.Id.publicEventRowName);
                    holder.publicEventRowDetails = convertView.FindViewById<TextView>(Resource.Id.publicEventRowDetails);
                    holder.publicEventRowImage = convertView.FindViewById<ImageView>(Resource.Id.publicEventRowImage);
                    convertView.Tag = holder;
                }
                if (holder == null)
                    holder = (ViewHolder)convertView.Tag;

                DateTime iKnowThisIsUtc = item.StartDate;
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(iKnowThisIsUtc, DateTimeKind.Utc);
                DateTime localVersion = runtimeKnowsThisIsUtc.ToLocalTime();
                holder.publicEventRowName.Text = item.Name;
                holder.publicEventRowDetails.Text = localVersion.Day + "/" + localVersion.Month + ", " + item.StartDate.ToShortTimeString() + " @ " + item.Location;
                holder.publicEventRowImage.SetImageBitmap(null);

                if (!String.IsNullOrEmpty(item.LogoUrl))
                {
                    var bit = n_bitmapCache.Where(x => x.Id == item.LogoUrl).FirstOrDefault();
                    if (bit == null)
                    {
                        Task<bool>.Factory.StartNew(
                                       () =>
                                       {
                                           try
                                           {
                                               LoggerMobile.Instance.logMessage("GettingLogo" + item.LogoUrl, LoggerEnum.message);
                                               var i = Image.GetImageBitmapFromUrl(item.LogoUrl);
                                               Image img = new Image();
                                               img.Id = item.LogoUrl;
                                               img.Img = i;
                                               n_bitmapCache.Add(img);
                                               context.RunOnUiThread(() =>
                                               {
                                                   holder.publicEventRowImage.SetImageBitmap(i);
                                               });
                                           }
                                           catch (Exception exception)
                                           {
                                               ErrorHandler.Save(exception, MobileTypeEnum.Android, context);
                                           }
                                           return true;
                                       });
                    }
                    else
                        holder.publicEventRowImage.SetImageBitmap(bit.Img);
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