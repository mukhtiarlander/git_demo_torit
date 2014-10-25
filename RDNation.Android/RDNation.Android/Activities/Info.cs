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
using RDN.Portable.Config;
using RDN.Portable.Config.Enums;


namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class InfoActivity : LegacyBarActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);

                SetContentView(Resource.Layout.Info);
                LoggerMobile.Instance.logMessage("Opening Info", LoggerEnum.message);


                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SeparatorColor = Color.Purple;
                AddHomeAction(typeof(Main), Resource.Drawable.icon);

                TextView statusfeedback = FindViewById<TextView>(Resource.Id.statusfeedback);
                statusfeedback.Click += statusfeedback_Click;

                TextView statusGames = FindViewById<TextView>(Resource.Id.statusGames);
                statusGames.Click += statusGames_Click;

                TextView statusEvents = FindViewById<TextView>(Resource.Id.statusEvents);
                statusEvents.Click += statusEvents_Click;

                TextView statusSkaters = FindViewById<TextView>(Resource.Id.statusSkaters);
                statusSkaters.Click += statusSkaters_Click;

                TextView statusLeagues = FindViewById<TextView>(Resource.Id.statusLeagues);
                statusLeagues.Click += statusLeagues_Click;

                TextView statusShops = FindViewById<TextView>(Resource.Id.statusShops);
                statusShops.Click += statusShops_Click;


            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }
        }

        void statusSkaters_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse(ServerConfig.WEBSITE_SIGNUP_URL);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }
        void statusLeagues_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse(ServerConfig.WEBSITE_SETUP_LEAGUE_URL);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }
        void statusShops_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse(ServerConfig.WEBSITE_SHOPS_SELL_URL);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }
        void statusEvents_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse(ServerConfig.WEBSITE_EVENTS_URL);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        void statusGames_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse(ServerConfig.WEBSITE_SCOREBOARD_URL);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        void statusfeedback_Click(object sender, EventArgs e)
        {
            try
            {
                var email = new Intent(Android.Content.Intent.ActionSend);
                email.PutExtra(Android.Content.Intent.ExtraEmail, new string[] { "info@rdnation.com" });
                email.PutExtra(Android.Content.Intent.ExtraSubject, "Android " + Resource.String.version + ":");
                email.SetType("message/rfc822");
                email.PutExtra(Android.Content.Intent.ExtraText,
    "");
                StartActivity(email);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }
        }


    }
}

