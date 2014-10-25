using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using RDN.Utilities.Error;

using RDNation.Droid.Classes;
using Android.Net;
using System.Threading.Tasks;
using RDN.Utilities.Util;
using LegacyBar.Library.BarBase;
using Android.Graphics;
using LegacyBar.Library.BarActions;
using Android.Content.PM;
using RDN.Mobile.Account;
using RDNation.Droid.Classes.Account;
using System.Threading;
using RDNation.Droid.Adapters;
using System.Collections.Generic;
using SlidingMenuSharp;
using SlidingMenuSharp.App;
using Android.Text.Method;
using Android.Text;
using RDNation.Droid.Classes.Public;
using RDNation.Droid.Classes.Helpers;
using RDNation.Droid.Classes.Images;
using Android.Graphics.Drawables;
using RDN.Mobile.Database;
using RDN.Mobile.Classes.Public;
using RDN.Mobile.Classes.Utilities;
using RDNation.Droid.UI.ActionBarAction;
using RDNation.Droid.Adapters.UI;
using RDN.Portable.Models.Json.Shop;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class ShopActivity : LegacyBarActivity
    {

        ShopItemJson ev;
        View m_AdView;
        TextView shopItemPrice;
        TextView shopItemSoldBy;
        TextView shopItemDescription;
        TextView eventOrganizerInfo;
        TextView eventDescription;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening ShopActvitiy", LoggerEnum.message);
                var eventString = Intent.GetStringExtra("shopItem");
                ev = Json.DeserializeObject<ShopItemJson>(eventString);

                SetContentView(Resource.Layout.PublicShopItem);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SeparatorColor = Color.Purple;
                LegacyBar.SetHomeAction(new BackAction(this, null, Resource.Drawable.shop, this));
                LegacyBar.Title = ev.Name;
                // Get our button from the layout resource,
                // and attach an event to it

                try
                {
                    Gallery gallery = (Gallery)FindViewById<Gallery>(Resource.Id.shopItemImage);
                    if (ev.PhotoUrlsThumbs != null && ev.PhotoUrlsThumbs[0] != null)
                        gallery.Adapter = new ImageAdapter(this, ev.PhotoUrlsThumbs);
                    else
                        gallery.Adapter = new ImageAdapter(this, ev.PhotoUrls);
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                }
                TextView shopItemName = FindViewById<TextView>(Resource.Id.shopItemName);
                shopItemName.Text = ev.Name;

                shopItemSoldBy = FindViewById<TextView>(Resource.Id.shopItemSoldBy);
                shopItemSoldBy.Text = ev.SoldBy;
                shopItemSoldBy.PaintFlags = PaintFlags.UnderlineText;
                shopItemSoldBy.Clickable = true;
                //shopItemSoldBy.Click += eventLocation_Click;
                shopItemPrice = FindViewById<TextView>(Resource.Id.shopItemPrice);
                shopItemPrice.Text = ev.Price.ToString();


                shopItemDescription = FindViewById<TextView>(Resource.Id.shopItemDescription);
                if (!String.IsNullOrEmpty(ev.Notes))
                    shopItemDescription.Text = Html.FromHtml(ev.Notes).ToString();


                Button shopItemBuyNow = FindViewById<Button>(Resource.Id.shopItemBuyNow);
                shopItemBuyNow.Click += shopItemBuyNow_Click;


                m_AdView = FindViewById(Resource.Id.adView);
                if (SettingsMobile.Instance.User != null && SettingsMobile.Instance.User.IsValidSub)
                {
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }

        void shopItemBuyNow_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse(ev.RDNUrl);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        protected override void OnDestroy()
        {
            try
            {
                //if (m_AdView != null)
                //    AdMobHelper.Destroy(m_AdView);
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, this);
            }
        }


        private class BackAction : LegacyBarAction
        {
            Activity Activity;
            public BackAction(Context context, Intent intent, int drawable, Activity activity)
            {
                Drawable = drawable;
                Context = context;
                Intent = intent;
                Activity = activity;
            }

            public override int GetDrawable()
            {
                return Drawable;
            }

            public override void PerformAction(View view)
            {
                try
                {
                    Activity.Finish();
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

                }
            }
        }

    }

}

