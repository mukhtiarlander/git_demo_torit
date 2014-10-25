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
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class EventActivity : LegacyBarActivity
    {

        EventJson ev;
        View m_AdView;
        TextView eventAddress;
        TextView eventLocation;
        TextView eventDateTime;
        TextView eventOrganizerInfo;
        TextView eventDescription;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening EventActivity", LoggerEnum.message);
                var eventString = Intent.GetStringExtra("event");
                ev = Json.DeserializeObject<EventJson>(eventString);

                SetContentView(Resource.Layout.PublicEvent);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SeparatorColor = Color.Purple;
                LegacyBar.SetHomeAction(new BackAction(this, null, Resource.Drawable.calendar, this));
                LegacyBar.Title = ev.Name;
                // Get our button from the layout resource,
                // and attach an event to it

                if (!String.IsNullOrEmpty(ev.TicketUrl))
                {
                    LegacyBarAction azAction = new WebBrowserAction(this, null, Resource.Drawable.ic_action_data_ticket_icon, ev.TicketUrl);
                    LegacyBar.AddAction(azAction);
                }
                if (!String.IsNullOrEmpty(ev.RDNUrl))
                {
                    LegacyBarAction azAction = new WebBrowserAction(this, null, Resource.Drawable.icon, ev.RDNUrl);
                    LegacyBar.AddAction(azAction);
                }
                if (!String.IsNullOrEmpty(ev.EventUrl))
                {
                    LegacyBarAction azAction = new WebBrowserAction(this, null, Resource.Drawable.action_about, ev.EventUrl);
                    LegacyBar.AddAction(azAction);
                }

                TextView eventName = FindViewById<TextView>(Resource.Id.eventName);
                eventName.Text = ev.Name;

                eventLocation = FindViewById<TextView>(Resource.Id.eventLocation);
                eventLocation.Text = ev.Location;
                eventLocation.PaintFlags = PaintFlags.UnderlineText;
                eventLocation.Clickable = true;
                eventLocation.Click += eventLocation_Click;
                eventAddress = FindViewById<TextView>(Resource.Id.eventAddress);
                eventAddress.Text = "@ " + ev.Address;
                eventAddress.PaintFlags = PaintFlags.UnderlineText;
                eventAddress.Clickable = true;
                eventAddress.Click += eventLocation_Click;

                eventDateTime = FindViewById<TextView>(Resource.Id.eventDateTime);
                DateTime iKnowThisIsUtc = ev.StartDate;
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(iKnowThisIsUtc, DateTimeKind.Utc);
                DateTime startDate = runtimeKnowsThisIsUtc.ToLocalTime();

                DateTime iKnowThisIsUtcEnd = ev.EndDate;
                DateTime runtimeKnowsThisIsUtcEnd = DateTime.SpecifyKind(iKnowThisIsUtcEnd, DateTimeKind.Utc);
                DateTime endDate = runtimeKnowsThisIsUtcEnd.ToLocalTime();

                eventDateTime.Text = startDate.ToShortDateString() + " " + startDate.ToShortTimeString() + " - " + endDate.ToShortTimeString();
                eventOrganizerInfo = FindViewById<TextView>(Resource.Id.eventOrganizerInfo);
                eventOrganizerInfo.Text = ev.OrganizersName;
                eventOrganizerInfo.PaintFlags = PaintFlags.UnderlineText;

                eventOrganizerInfo.Clickable = true;
                eventOrganizerInfo.Click += eventOrganizerInfo_Click;
                eventDescription = FindViewById<TextView>(Resource.Id.eventDescription);
                if (!String.IsNullOrEmpty(ev.Description))
                    eventDescription.Text = Html.FromHtml(ev.Description).ToString();





                SetProfileImage(ev);



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



        void eventLocation_Click(object sender, EventArgs e)
        {
            var geoUri = Android.Net.Uri.Parse("geo:" + ev.Latitude + "," + ev.Longitude + "?q=" + ev.Location + ", " + ev.Address);
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            StartActivity(mapIntent);
        }

        void eventOrganizerInfo_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(LeagueActivity));
            intent.PutExtra("leagueId", ev.LeagueId);
            StartActivity(intent);
        }
        

        private void SetProfileImage(EventJson ev)
        {
            ImageView profileImage = FindViewById<ImageView>(Resource.Id.eventImage);
            if (!String.IsNullOrEmpty(ev.LogoUrl))
            {

                Task<bool>.Factory.StartNew(
                               () =>
                               {
                                   try
                                   {
                                       var i = Image.GetImageBitmapFromUrl(ev.LogoUrl);
                                       this.RunOnUiThread(() =>
                                       {
                                           profileImage.SetImageBitmap(i);
                                       });
                                   }
                                   catch (Exception exception)
                                   {
                                       ErrorHandler.Save(exception, MobileTypeEnum.Android, this);
                                   }
                                   return true;
                               });


            }
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

