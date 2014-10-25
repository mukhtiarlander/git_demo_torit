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
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class SkatersActivity : LegacyBarActivity
    {
        static MyCharacterPickerDialog _dialog;
        static String options = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        SkaterAdapter ListAdapter;
        SkatersJson initialArray;
        EditText search_skaters;
        ListView skaterList;
        private int PAGE_COUNT = 20;
        private string lastLetterPulled = "a";
        private int lastPagePulled = 0;
        View m_AdView;
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening SkatersActivity", LoggerEnum.message);
                SetContentView(Resource.Layout.PublicSkaters);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SetHomeLogo(Resource.Drawable.icon);
                LegacyBar.SeparatorColor = Color.Purple;

                LegacyBarAction signUpAction = new MenuAction(this, null, Resource.Drawable.a_z);
                LegacyBar.AddAction(signUpAction);

                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                AddHomeAction(typeof(Main), Resource.Drawable.icon);
                // Get our button from the layout resource,
                // and attach an event to it

                Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
                LegacyBar.ProgressBarVisibility = ViewStates.Visible;
                Skater.PullSkaters(lastPagePulled, PAGE_COUNT, lastLetterPulled, (Context)this, skaters);


                skaterList = FindViewById<ListView>(Resource.Id.skaterList);
                initialArray = new SkatersJson();
                Action pullMoreSkaters = new Action(PullMoreSkaters);
                ListAdapter = new SkaterAdapter(this, initialArray.Skaters, pullMoreSkaters);
                skaterList.Adapter = ListAdapter;
                skaterList.FastScrollEnabled = true;


                skaterList.ItemClick += skaterList_ItemClick;
                var myString = new SpannableStringBuilder("lol");
                Selection.SelectAll(myString); // needs selection or Index Out of bounds
                _dialog = new MyCharacterPickerDialog(this, new View(this), myString, options, false);
                _dialog.Clicked += (sender, args) =>
                {
                    lastPagePulled = 0;
                    lastLetterPulled = args.Text;
                    LegacyBar.ProgressBarVisibility = ViewStates.Visible;
                    Skater.PullSkaters(0, PAGE_COUNT, lastLetterPulled, (Context)this, skaters);
                    initialArray.Skaters.Clear();
                };
                search_skaters = FindViewById<EditText>(Resource.Id.search_skaters);
                search_skaters.TextChanged += search_skaters_TextChanged;
                var searchMenuItemAction = new SearchAction(this, null, Resource.Drawable.ic_action_search, search_skaters);
                LegacyBar.AddAction(searchMenuItemAction);

                LegacyBarAction infoAction = new DefaultLegacyBarAction(this, CreateInfoIntent(), Resource.Drawable.action_about);
                LegacyBar.AddAction(infoAction);

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
        private Intent CreateInfoIntent()
        {
            var intent = new Intent(this, typeof(InfoActivity));
            return intent;
        }
        protected override void OnDestroy()
        {
            try{
            //if (m_AdView != null)
                //AdMobHelper.Destroy(m_AdView);
            base.OnDestroy();
               }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }
        void search_skaters_TextChanged(object sender, TextChangedEventArgs e)
        {
            lastPagePulled = 0;
            LegacyBar.ProgressBarVisibility = ViewStates.Visible;
            Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
            var t = (EditText)sender;
            if (t.Text.Length == 0)
            {
                Skater.PullSkaters(lastPagePulled, PAGE_COUNT, lastLetterPulled, (Context)this, skaters);
            }
            else if (t.Text.Length > 2)
            {
                Skater.SearchSkaters(lastPagePulled, PAGE_COUNT, t.Text, (Context)this, skaters);
            }
            initialArray.Skaters.Clear();
        }
        void UpdateAdapter(SkatersJson skaters)
        {
            initialArray.Skaters.AddRange(skaters.Skaters);
            RunOnUiThread(() =>
            {
                try
                {
                    int firstPosition = skaterList.ScrollY;
                    ListAdapter.NotifyDataSetChanged();
                    LegacyBar.ProgressBarVisibility = ViewStates.Gone;
                    skaterList.ScrollTo(0, firstPosition);
                    _dialog.Dismiss();
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                }
            });
        }
        void PullMoreSkaters()
        {
            lastPagePulled += 1;
            Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateAdapter);
            LegacyBar.ProgressBarVisibility = ViewStates.Visible;
            Skater.PullSkaters(lastPagePulled, PAGE_COUNT, lastLetterPulled, (Context)this, skaters);
        }

        void skaterList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                var listView = sender as ListView;
                var t = initialArray.Skaters[e.Position];

                var intent = new Intent(this, typeof(SkaterActivity));
                var data =Json.ConvertToString< SkaterJson>(t);
                intent.PutExtra("skater", data);
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
                    _dialog.Show();
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

