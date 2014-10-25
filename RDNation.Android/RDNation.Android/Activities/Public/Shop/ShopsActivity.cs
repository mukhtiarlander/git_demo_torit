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
using RDNation.Droid.Activities.UI;
using RDN.Mobile.Classes.Utilities;
using Android.Locations;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Models.Json.Shop;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Activities.Calendar
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class ShopsActivity : LegacyBarActivity
    {
        //static MyCharacterPickerDialog _dialog;
        //static String options = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        ShopItemsAdapter ListAdapter;
        ShopsJson initialArray;
        EditText search_events;
        ListView eventsList;
        public int PAGE_COUNT = 20;
        public string lastLetterPulled = "";
        public int lastPagePulled = 0;
        View m_AdView;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening ShopsAdapter", LoggerEnum.message);
                SetContentView(Resource.Layout.PublicEvents);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SetHomeLogo(Resource.Drawable.icon);
                LegacyBar.SeparatorColor = Color.Purple;
                LegacyBar.Title = "Shop Derby";
                

                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                AddHomeAction(typeof(Main), Resource.Drawable.icon);
                // Get our button from the layout resource,
                // and attach an event to it

                Action<ShopsJson> shops = new Action<ShopsJson>(UpdateAdapter);
                LegacyBar.ProgressBarVisibility = ViewStates.Visible;

                eventsList = FindViewById<ListView>(Resource.Id.eventsList);
                initialArray = new ShopsJson();
                Action pullMore = new Action(PullMore);
                Shop.PullShopItems(lastPagePulled, PAGE_COUNT, (Context)this, shops);
                ListAdapter = new ShopItemsAdapter(this, initialArray.Items, pullMore);
                eventsList.Adapter = ListAdapter;
                eventsList.FastScrollEnabled = true;


                eventsList.ItemClick += skaterList_ItemClick;
                var myString = new SpannableStringBuilder("lol");
                Selection.SelectAll(myString); // needs selection or Index Out of bounds

                search_events = FindViewById<EditText>(Resource.Id.search_events);
                search_events.TextChanged += search_skaters_TextChanged;
                var searchMenuItemAction = new SearchAction(this, null, Resource.Drawable.ic_action_search, search_events);
                LegacyBar.AddAction(searchMenuItemAction);

                LegacyBarAction infoAction = new DefaultLegacyBarAction(this, CreateInfoIntent(), Resource.Drawable.action_about);
                LegacyBar.AddAction(infoAction);
                

                m_AdView = FindViewById(Resource.Id.adView);
                m_AdView.Visibility = ViewStates.Gone;
                if (SettingsMobile.Instance.User != null && SettingsMobile.Instance.User.IsValidSub)
                {
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }
        private Intent CreateInfoIntent()
        {
            var intent = new Intent(this, typeof(InfoActivity));
            return intent;
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
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }


        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }



        void search_skaters_TextChanged(object sender, TextChangedEventArgs e)
        {
            lastPagePulled = 0;
            LegacyBar.ProgressBarVisibility = ViewStates.Visible;
            Action<ShopsJson> evs = new Action<ShopsJson>(UpdateAdapter);
            var t = (EditText)sender;
            if (t.Text.Length == 0)
            {
                Shop.PullShopItems(lastPagePulled, PAGE_COUNT, (Context)this, evs);
            }
            else if (t.Text.Length > 2)
            {
                Shop.SearchShopItems(lastPagePulled, PAGE_COUNT, t.Text, (Context)this, evs);
            }
            initialArray.Items.Clear();
        }
        void UpdateAdapter(ShopsJson events)
        {
            initialArray.Items.AddRange(events.Items);
            RunOnUiThread(() =>
            {
                try
                {
                    int firstPosition = eventsList.ScrollY;
                    ListAdapter.NotifyDataSetChanged();
                    LegacyBar.ProgressBarVisibility = ViewStates.Gone;
                    eventsList.ScrollTo(0, firstPosition);
                    //_dialog.Dismiss();
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                }
            });
        }
        void PullMore()
        {
            lastPagePulled += 1;
            Action<ShopsJson> evs = new Action<ShopsJson>(UpdateAdapter);
            LegacyBar.ProgressBarVisibility = ViewStates.Visible;
        Shop.PullShopItems(lastPagePulled, PAGE_COUNT, (Context)this, evs);
        }

        void skaterList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                var listView = sender as ListView;
                var t = initialArray.Items[e.Position];

                var intent = new Intent(this, typeof(ShopActivity));
                var data = Json.ConvertToString<ShopItemJson>(t);
                intent.PutExtra("shopItem", data);
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }





        private Intent CreateSignUpIntent()
        {
            var intent = new Intent(this, typeof(SignUpActivity));
            return intent;
        }

        private class MenuAction : LegacyBarAction
        {
            public MenuAction(Context context, Intent intent, int drawable)
            {
                Drawable = drawable;
                Context = context;
                Intent = intent;
            }

            public override int GetDrawable()
            {
                return Drawable;
            }

            public override void PerformAction(View view)
            {
                try
                {
                    //_dialog.Show();
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

                }
            }
        }
        private class SearchAction : LegacyBarAction
        {
            EditText text;
            public SearchAction(Context context, Intent intent, int drawable, EditText field)
            {
                Drawable = drawable;
                Context = context;
                Intent = intent;
                text = field;
            }

            public override int GetDrawable()
            {
                return Drawable;
            }

            public override void PerformAction(View view)
            {
                try
                {
                    if (text.Visibility == ViewStates.Gone)
                    {
                        text.Visibility = ViewStates.Visible;
                        text.RequestFocus();
                    }
                    else
                        text.Visibility = ViewStates.Gone;
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

                }
            }
        }

    }

}

