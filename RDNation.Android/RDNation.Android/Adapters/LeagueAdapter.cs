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
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Adapters
{
    public class LeagueAdapter : BaseAdapter<LeagueJsonDataTable>
    {
        List<LeagueJsonDataTable> items;
        Activity context;
        List<Image> n_bitmapCache;
        Action MoreCallback;
        public LeagueAdapter(Activity context, List<LeagueJsonDataTable> items, Action getMoreCallback)
            : base()
        {
            this.context = context;
            this.items = items;
            this.MoreCallback = getMoreCallback;
            n_bitmapCache = new List<Image>();
            LoggerMobile.Instance.logMessage("Opening LeagueAdapter", LoggerEnum.message);
        }


        public void AddItems(List<LeagueJsonDataTable> items)
        {
            this.items.AddRange(items);
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override LeagueJsonDataTable this[int position]
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
            if (position == (Count - 5))
                this.MoreCallback();

            var item = items[position];
            View view = convertView;
            try
            {
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.PublicLeagueRow, null);
                view.FindViewById<TextView>(Resource.Id.leagueName).Text = item.LeagueName;
                view.FindViewById<TextView>(Resource.Id.leagueCityState).Text = item.City + ", " + item.State + " " + item.Country;
                var image = view.FindViewById<ImageView>(Resource.Id.leagueImageRow);
                image.SetImageBitmap(null);

                if (!String.IsNullOrEmpty(item.LogoUrlThumb))
                {
                    var bit = n_bitmapCache.Where(x => x.Id == item.LeagueId).FirstOrDefault();
                    if (bit == null)
                    {
                        Task<bool>.Factory.StartNew(
                                       () =>
                                       {
                                           try
                                           {
                                               LoggerMobile.Instance.logMessage("leagueLogo: " + item.LogoUrlThumb, LoggerEnum.message);
                                               var i = Image.GetImageBitmapFromUrl(item.LogoUrlThumb);
                                                                                        
                                               context.RunOnUiThread(() =>
                                               {
                                                   image.SetImageBitmap(i);
                                               });
                                               Image img = new Image();
                                               img.Id = item.LeagueId;
                                               img.Img = i;
                                               n_bitmapCache.Add(img);

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