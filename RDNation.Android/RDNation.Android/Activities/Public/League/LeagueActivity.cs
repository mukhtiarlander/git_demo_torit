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
using Droid = Android.Net;
using RDN.Mobile.Classes.Utilities;
using RDN.Portable.Models.Json;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class LeagueActivity : LegacyBarActivity
    {

        LeaguesJson initialArray;
        View m_AdView;
        TextView gamesCount;
        Button teamNameBtn;
        TextView winsLoses;
        TextView leagueCityState;
        TextView leagueName;
        bool HasSetLeagueImage = false;
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening LeagueActivity", LoggerEnum.message);
                SetContentView(Resource.Layout.PublicLeagueProfile);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SeparatorColor = Color.Purple;
                LegacyBar.SetHomeAction(new BackAction(this, null, Resource.Drawable.icon, this));
                leagueName = FindViewById<TextView>(Resource.Id.leagueName);
                leagueCityState = FindViewById<TextView>(Resource.Id.leagueCityState);
                var leagueIdString = Intent.GetStringExtra("leagueId");

                if (String.IsNullOrEmpty(leagueIdString))
                {
                    var leagueString = Intent.GetStringExtra("league");
                    var league = Json.DeserializeObject<LeagueJsonDataTable>(leagueString);
                    leagueIdString = league.LeagueId;
                    LegacyBar.Title = league.LeagueName;
                    leagueName.Text = league.LeagueName;
                    leagueCityState.Text = league.City + ", " + league.State + " " + league.Country;
                    LoggerMobile.Instance.logMessage("On League: " +league.LeagueName +" " + leagueIdString, LoggerEnum.message);
                    SetProfileImage(league);
                    HasSetLeagueImage = true;
                }
                LegacyBar.ProgressBarVisibility = ViewStates.Visible;

                Action<LeagueJsonDataTable> leagues = new Action<LeagueJsonDataTable>(UpdateAdapter);
                League.PullLeague(leagueIdString, (Context)this, leagues);
                Action<EventsJson> leagueEvents = new Action<EventsJson>(UpdateEventsAdapter);
                League.PullLeagueEvents(leagueIdString, (Context)this, leagueEvents);
                Action<SkatersJson> skaters = new Action<SkatersJson>(UpdateSkatersAdapter);
                Skater.PullSkaters(leagueIdString, (Context)this, skaters);

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

        private void SetProfileImage(LeagueJsonDataTable league)
        {
            ImageView profileImage = FindViewById<ImageView>(Resource.Id.leagueImage);
            if (!String.IsNullOrEmpty(league.LogoUrlThumb))
            {

                Task<bool>.Factory.StartNew(
                               () =>
                               {
                                   try
                                   {
                                       LoggerMobile.Instance.logMessage("LeagueLogo: " + league.LogoUrlThumb, LoggerEnum.message);
                                       var i = Image.GetImageBitmapFromUrl(league.LogoUrlThumb);
                                       this.RunOnUiThread(() =>
                                       {
                                           profileImage.SetImageBitmap(i);
                                       });
                                   }
                                   catch (Exception exception)
                                   {
                                       ErrorHandler.Save(exception, MobileTypeEnum.Android, this, additionalInformation: league.LogoUrlThumb);
                                   }
                                   return true;
                               });


            }
        }
        private void SetProfileImage(string url, ImageView image)
        {

            if (!String.IsNullOrEmpty(url))
            {

                Task<bool>.Factory.StartNew(
                               () =>
                               {
                                   try
                                   {
                                       LoggerMobile.Instance.logMessage("LeagueProfileImage: " +url, LoggerEnum.message);
                                       var i = Image.GetImageBitmapFromUrl(url);
                                       this.RunOnUiThread(() =>
                                       {
                                           image.SetImageBitmap(i);
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
        private void SetLeagueImage(SkaterJson skater)
        {

            if (!String.IsNullOrEmpty(skater.LeagueLogo))
            {

                Task<bool>.Factory.StartNew(
                               () =>
                               {
                                   try
                                   {
                                       LoggerMobile.Instance.logMessage("league Logo:" + skater.LeagueLogo, LoggerEnum.message);
                                       var i = Image.GetImageBitmapFromUrl(skater.LeagueLogo);
                                       this.RunOnUiThread(() =>
                                       {
                                           teamNameBtn.SetCompoundDrawablesWithIntrinsicBounds(new BitmapDrawable(i), null, null, null);
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
        void UpdateEventsAdapter(EventsJson events)
        {
            TableLayout leagueScheduleTable = FindViewById<TableLayout>(Resource.Id.leagueScheduleTable);
            RunOnUiThread(() =>
                        {
                            try
                            {
                                LegacyBar.ProgressBarVisibility = ViewStates.Gone;
                                // Go through each item in the array
                                for (int current = 0; current < events.Events.Count; current++)
                                {
                                    // Create a TableRow and give it an ID
                                    View scheduleTableRow = LayoutInflater.Inflate(Resource.Layout.PublicLeagueEventRow, null, false);

                                    DateTime iKnowThisIsUtc = events.Events[current].StartDate;
                                    DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(iKnowThisIsUtc, DateTimeKind.Utc);
                                    DateTime localVersion = runtimeKnowsThisIsUtc.ToLocalTime();
                                    var publicLeagueRowDate = scheduleTableRow.FindViewById<TextView>(Resource.Id.publicLeagueRowDate);
                                    publicLeagueRowDate.Text = localVersion.ToShortDateString();
                                    var publicLeagueRowTime = scheduleTableRow.FindViewById<TextView>(Resource.Id.publicLeagueRowTime);
                                    publicLeagueRowTime.Text = localVersion.ToShortTimeString();
                                    var publicLeagueRowEventName = scheduleTableRow.FindViewById<TextView>(Resource.Id.publicLeagueRowEventName);
                                    publicLeagueRowEventName.Text = events.Events[current].Name;
                                    var publicLeagueRowId = scheduleTableRow.FindViewById<TextView>(Resource.Id.publicLeagueRowId);
                                    publicLeagueRowId.Text = events.Events[current].CalendarItemId;
                                    publicLeagueRowDate.PaintFlags = PaintFlags.UnderlineText;
                                    publicLeagueRowEventName.PaintFlags = PaintFlags.UnderlineText;
                                    publicLeagueRowTime.PaintFlags = PaintFlags.UnderlineText;

                                    if (current % 2 == 0)
                                    {
                                        scheduleTableRow.SetBackgroundResource(Resource.Color.gray);
                                    }

                                    scheduleTableRow.Clickable = true;
                                    scheduleTableRow.Click += scheduleTableRow_Click;
                                    leagueScheduleTable.AddView(scheduleTableRow); //not working, obviously im missing something

                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                            }
                        });
        }
        void UpdateSkatersAdapter(SkatersJson skaters)
        {
            TableLayout leagueMemberTable = FindViewById<TableLayout>(Resource.Id.leagueMemberTable);
            RunOnUiThread(() =>
            {
                try
                {
                    LegacyBar.ProgressBarVisibility = ViewStates.Gone;
                    // Go through each item in the array
                    for (int current = 0; current < skaters.Skaters.Count; current++)
                    {
                        // Create a TableRow and give it an ID
                        View memberTableRow = LayoutInflater.Inflate(Resource.Layout.PublicLeagueMemberRow, null, false);


                        var publicLeagueRowMemberNumber = memberTableRow.FindViewById<TextView>(Resource.Id.publicLeagueRowMemberNumber);
                        publicLeagueRowMemberNumber.Text = skaters.Skaters[current].DerbyNumber;
                        var publicLeagueRowMemberName = memberTableRow.FindViewById<TextView>(Resource.Id.publicLeagueRowMemberName);
                        publicLeagueRowMemberName.Text = skaters.Skaters[current].DerbyName;
                        var publicLeagueRowId = memberTableRow.FindViewById<TextView>(Resource.Id.publicLeagueRowId);
                        publicLeagueRowId.Text = skaters.Skaters[current].MemberId;
                        publicLeagueRowMemberNumber.PaintFlags = PaintFlags.UnderlineText;
                        publicLeagueRowMemberName.PaintFlags = PaintFlags.UnderlineText;
                        var publicLeagueRowMemberImage = memberTableRow.FindViewById<ImageView>(Resource.Id.publicLeagueRowMemberImage);
                        SetProfileImage(skaters.Skaters[current].ThumbUrl, publicLeagueRowMemberImage);

                        if (current % 2 == 0)
                        {
                            memberTableRow.SetBackgroundResource(Resource.Color.gray);
                        }

                        memberTableRow.Clickable = true;
                        memberTableRow.Click += memberTableRow_Click;
                        leagueMemberTable.AddView(memberTableRow); //not working, obviously im missing something

                    }
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                }
            });
        }

        void memberTableRow_Click(object sender, EventArgs e)
        {
            var id = (TextView)((TableRow)sender).GetChildAt(0);
            var ev = new SqlFactory().GetSkaterProfile(id.Text);
            var intent = new Intent(this, typeof(SkaterActivity));
            var data = Json.ConvertToString<SkaterJson>(ev);
            intent.PutExtra("skater", data);
            StartActivity(intent);
        }
        void scheduleTableRow_Click(object sender, EventArgs e)
        {
            try
            {
                var id = (TextView)((TableRow)sender).GetChildAt(0);
                var ev = new SqlFactory().GetCalendarEvent(id.Text);
                var intent = new Intent(this, typeof(EventActivity));
                var data = Json.ConvertToString<EventJson>(ev);
                intent.PutExtra("event", data);
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }

        void UpdateAdapter(LeagueJsonDataTable league)
        {
            if (!HasSetLeagueImage)
                SetProfileImage(league);
            RunOnUiThread(() =>
            {
                try
                {
                    LegacyBar.Title = league.LeagueName;
                    leagueName.Text = league.LeagueName;
                    leagueCityState.Text = league.City + ", " + league.State + " " + league.Country;
                    LegacyBar.ProgressBarVisibility = ViewStates.Gone;
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                }
            });
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

