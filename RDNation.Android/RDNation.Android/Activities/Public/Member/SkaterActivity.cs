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
using RDN.Portable.Models.Json;
using RDN.Portable.Config.Enums;
using RDN.Portable.Models.Json.Public;
using RDN.Portable.Settings;

namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class SkaterActivity : LegacyBarActivity
    {

        SkaterJson skater;
        View m_AdView;
        TextView gamesCount;
        Button teamNameBtn;
        TextView winsLoses;
        TextView bio;
        TextView profileHeight;
        TextView profileWeight;
        TextView profileName;
        LeagueJsonDataTable league;
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening SkaterActivty", LoggerEnum.message);
                var skaterString = Intent.GetStringExtra("skater");
                skater = Json.DeserializeObject<SkaterJson>(skaterString);
                LoggerMobile.Instance.logMessage("On Skater: " + skater.DerbyName + " " + skater.MemberId, LoggerEnum.message);
                SetContentView(Resource.Layout.SkaterProfile);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SeparatorColor = Color.Purple;
                LegacyBar.SetHomeAction(new BackAction(this, null, Resource.Drawable.skater, this));
                LegacyBar.Title = skater.DerbyName;
                // Get our button from the layout resource,
                // and attach an event to it
                if (!skater.GotExtendedContent)
                {
                    LegacyBar.ProgressBarVisibility = ViewStates.Visible;
                    Action<SkaterJson> skaters = new Action<SkaterJson>(UpdateAdapter);
                    Skater.PullSkater(skater.MemberId, (Context)this, skaters);
                }

                TextView ProfileNameNumber = FindViewById<TextView>(Resource.Id.ProfileNameNumber);
                ProfileNameNumber.Text = skater.DerbyName + " " + skater.DerbyNumber;

                gamesCount = FindViewById<TextView>(Resource.Id.gamesCount);
                gamesCount.Text = skater.GamesCount.ToString();

                winsLoses = FindViewById<TextView>(Resource.Id.winsLoses);
                winsLoses.Text = skater.Wins + "-" + skater.Losses;
                bio = FindViewById<TextView>(Resource.Id.bio);
                if (!String.IsNullOrEmpty(skater.Bio))
                    bio.Text = Html.FromHtml(skater.Bio).ToString();

                profileHeight = FindViewById<TextView>(Resource.Id.profileHeight);
                profileHeight.Text = skater.HeightFeet + "-" + skater.HeightInches;

                profileWeight = FindViewById<TextView>(Resource.Id.profileWeight);
                if (!String.IsNullOrEmpty(skater.Weight))
                    profileWeight.Text = skater.Weight;
                else
                    profileWeight.Text = "0";

                profileName = FindViewById<TextView>(Resource.Id.profileName);
                if (!String.IsNullOrEmpty(skater.FirstName))
                    profileName.Text = skater.FirstName;
                else
                    profileName.Text = " ";


                var profileAge = FindViewById<TextView>(Resource.Id.profileAge);
                var profileBorn = FindViewById<TextView>(Resource.Id.profileBorn);
                if (skater.DOB.ToShortDateString() != "1/1/0001")
                {
                    DateTime today = DateTime.Today;
                    int age = today.Year - skater.DOB.Year;
                    if (skater.DOB > today.AddYears(-age)) age--;
                    profileAge.Text = age.ToString();
                    profileBorn.Text = skater.DOB.ToShortDateString();
                }
                else
                {
                    var ageRow = FindViewById<TableRow>(Resource.Id.ageRow);
                    ageRow.Visibility = ViewStates.Gone;
                    var bornRow = FindViewById<TableRow>(Resource.Id.bornRow);
                    bornRow.Visibility = ViewStates.Gone;
                }



                SetProfileImage(skater);

                teamNameBtn = FindViewById<Button>(Resource.Id.teamNameBtn);
                if (!String.IsNullOrEmpty(skater.LeagueName))
                {
                    teamNameBtn.Text = skater.LeagueName;
                    SetLeagueImage(skater);
                    //just pulls the league so we can insert it into the DB..
                    var callback = new Action<LeagueJsonDataTable>(leagueCallback);
                    League.PullLeague(skater.LeagueId, this, callback);
                    teamNameBtn.Click += teamNameBtn_Click;
                }

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
        private void leagueCallback(LeagueJsonDataTable leag)
        {
            league = leag;
        }
        void teamNameBtn_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(LeagueActivity));
            //var league = new SqlFactory().GetLeagueProfile(skater.LeagueId);
            intent.PutExtra("league", Json.ConvertToString<LeagueJsonDataTable>(league));
            StartActivity(intent);
        }

        private void SetProfileImage(SkaterJson skater)
        {
            ImageView profileImage = FindViewById<ImageView>(Resource.Id.profileImage);
            if (!String.IsNullOrEmpty(skater.ThumbUrl) && !skater.ThumbUrl.Contains("roller-girl.jpg"))
            {

                Task<bool>.Factory.StartNew(
                               () =>
                               {
                                   try
                                   {
                                       var i = Image.GetImageBitmapFromUrl(skater.ThumbUrl);
                                       this.RunOnUiThread(() =>
                                       {
                                           profileImage.SetImageBitmap(i);
                                       });
                                   }
                                   catch (Exception exception)
                                   {
                                       ErrorHandler.Save(exception, MobileTypeEnum.Android, this, additionalInformation: skater.ThumbUrl);
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
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }

        void UpdateAdapter(SkaterJson skater)
        {
            SetLeagueImage(skater);
            RunOnUiThread(() =>
            {
                try
                {
                    if (!String.IsNullOrEmpty(skater.Bio))
                        bio.Text = Html.FromHtml(skater.Bio).ToString();
                    gamesCount.Text = skater.GamesCount.ToString();
                    winsLoses.Text = skater.Wins + "-" + skater.Losses;
                    profileHeight.Text = skater.HeightFeet + "-" + skater.HeightInches;
                    if (!String.IsNullOrEmpty(skater.Weight))
                        profileWeight.Text = skater.Weight;
                    else
                        profileWeight.Text = "0";
                    if (!String.IsNullOrEmpty(skater.FirstName))
                        profileName.Text = skater.FirstName;
                    else
                        profileName.Text = " ";
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

