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
using Android.Graphics.Drawables;
using RDNation.Droid.Classes.Services;
using RDNation.Droid.Classes.Account;
using RDNation.Droid.Classes.Configuration;
using RDNation.Droid.Activities.Calendar;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;


namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class Main : LegacyBarActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);

                SetContentView(Resource.Layout.Main);
                LoggerMobile.Instance.logMessage("Opening Main", LoggerEnum.message);


                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SetHomeLogo(Resource.Drawable.icon);
                LegacyBar.SeparatorColor = Color.Purple;

                if (SettingsMobile.Instance.User != null && SettingsMobile.Instance.User.IsLoggedIn)
                {
                    LegacyBarAction loginAction = new DefaultLegacyBarAction(this, CreateSettingsIntent(), Resource.Drawable.action_settings);
                    LegacyBar.AddAction(loginAction);
                }
                else
                {
                    LegacyBarAction loginAction = new DefaultLegacyBarAction(this, CreateLoginIntent(), Resource.Drawable.social_person);
                    LegacyBar.AddAction(loginAction);
                }
                var skatersBtn = FindViewById<Button>(Resource.Id.skatersBtn);
                skatersBtn.Click += skatersBtn_Click;

                var leaguesBtn = FindViewById<Button>(Resource.Id.leaguesBtn);
                leaguesBtn.Click += leaguesBtn_Click;

                var gamesBtn = FindViewById<Button>(Resource.Id.gamesBtn);
                gamesBtn.Click += gamesBtn_Click;

                var calendarBtn = FindViewById<Button>(Resource.Id.calendarBtn);
                calendarBtn.Click += calendarBtn_Click;

                var shopsBtn = FindViewById<Button>(Resource.Id.shopsBtn);
                shopsBtn.Click += shopsBtn_Click;

                LegacyBarAction infoAction = new DefaultLegacyBarAction(this, CreateInfoIntent(), Resource.Drawable.action_about);
                LegacyBar.AddAction(infoAction);
                //throw new Exception("BoomAndroid");

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }
        }

        void skatersBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(SkatersActivity));
        }
        void gamesBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(GamesActivity));
        }
        void leaguesBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LeaguesActivity));
        }
        void calendarBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(EventsActivity));
        }
        void shopsBtn_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(ShopsActivity));
        }
        private Intent CreateLoginIntent()
        {
            var intent = new Intent(this, typeof(LoginActivity));
            return intent;
        }
        private Intent CreateInfoIntent()
        {
            var intent = new Intent(this, typeof(InfoActivity));
            return intent;
        }

        private Intent CreateSettingsIntent()
        {
            var intent = new Intent(this, typeof(SettingsActivity));
            return intent;
        }
    }
}

