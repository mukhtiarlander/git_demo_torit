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
using Droid = Android.Net;
using RDN.Portable.Models.Json.Games;
using RDN.Portable.Config.Enums;
using RDN.Portable.Settings;
namespace RDNation.Droid
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class GamesActivity : LegacyBarActivity
    {
        GamesAdapter PastGamesAdapter;
        //GamesAdapter CurrentGamesAdapter;
        GamesJson initialPastArray;
        //GamesJson initialCurrentArray;
        //ListView currentGamesList;
        ListView pastGamesList;
        public static int PAGE_COUNT = 20;
        public static int lastPagePulled = 0;
        View m_AdView;
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening GamesActivity", LoggerEnum.message);
                SetContentView(Resource.Layout.PublicGames);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SeparatorColor = Color.Purple;

                AddHomeAction(typeof(Main), Resource.Drawable.icon);
                // Get our button from the layout resource,
                // and attach an event to it

                LegacyBar.ProgressBarVisibility = ViewStates.Visible;
                //currentGamesList = FindViewById<ListView>(Resource.Id.currentGamesList);
                pastGamesList = FindViewById<ListView>(Resource.Id.pastGamesList);
                LegacyBarAction loginAction = new RefreshAction(this, null, Resource.Drawable.ic_action_refresh, this);
                LegacyBar.AddAction(loginAction);

                initialPastArray = new GamesJson();
                //initialCurrentArray = new GamesJson();
                Action pullMorePast = new Action(PullMorePast);
                //Action pullCurrent = new Action(PullMorePast);

                PastGamesAdapter = new GamesAdapter(this, initialPastArray.Games, pullMorePast);
                //CurrentGamesAdapter = new GamesAdapter(this, initialCurrentArray.Games, null);

                //currentGamesList.Adapter = CurrentGamesAdapter;
                pastGamesList.Adapter = PastGamesAdapter;
                Game.PullCurrentGames(this, UpdateCurrentAdapter);
                Game.PullPastGames(PAGE_COUNT, lastPagePulled, this, UpdatePastAdapter);

                //currentGamesList.ItemClick += currentGamesList_ItemClick;
                pastGamesList.ItemClick += pastGamesList_ItemClick;
                var myString = new SpannableStringBuilder("lol");
                Selection.SelectAll(myString); // needs selection or Index Out of bounds

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
     

        void pastGamesList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {

                var listView = sender as ListView;
                var t = initialPastArray.Games[e.Position];

                var uri = Android.Net.Uri.Parse(t.GameUrl);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }

        protected override void OnDestroy()
        {
            try{
            //if (m_AdView != null)
            //    AdMobHelper.Destroy(m_AdView);
            base.OnDestroy();
              }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
            }
        }
        void UpdatePastAdapter(GamesJson games)
        {
            initialPastArray.Games.AddRange(games.Games);
            RunOnUiThread(() =>
            {
                try
                {
                    int firstPosition = pastGamesList.ScrollY;
                    PastGamesAdapter.NotifyDataSetChanged();
                    LegacyBar.ProgressBarVisibility = ViewStates.Gone;
                    pastGamesList.ScrollTo(0, firstPosition);
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                }
            });
        }
        void UpdateCurrentAdapter(GamesJson games)
        {
            initialPastArray.Games.AddRange(games.Games);

            RunOnUiThread(() =>
            {
                try
                {
                    if (games.Games.Count > 0)
                    {
                        PastGamesAdapter.NotifyDataSetChanged();
                    }
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
                }
            });
        }

        void PullMorePast()
        {
            lastPagePulled += 1;
            Action<GamesJson> games = new Action<GamesJson>(UpdatePastAdapter);
            LegacyBar.ProgressBarVisibility = ViewStates.Visible;
            Game.PullPastGames(PAGE_COUNT, lastPagePulled, (Context)this, games);
        }


        //void skaterList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    try
        //    {
        //        var listView = sender as ListView;
        //        var t = initialArray.Leagues[e.Position];

        //        var intent = new Intent(this, typeof(LeagueActivity));
        //        var data = LeagueJsonDataTable.ConvertToString(t);
        //        intent.PutExtra("league", data);
        //        StartActivity(intent);
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);
        //    }
        //}





        private Intent CreateSignUpIntent()
        {
            var intent = new Intent(this, typeof(SignUpActivity));
            return intent;
        }

        private class RefreshAction : LegacyBarAction
        {
            GamesActivity Activity;
            public RefreshAction(Context context, Intent intent, int drawable, GamesActivity activity)
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
                    Activity.initialPastArray.Games.Clear();
                    Activity.LegacyBar.ProgressBarVisibility = ViewStates.Visible;
                    Game.PullCurrentGames(Context, Activity.UpdateCurrentAdapter);
                    Game.PullPastGames(PAGE_COUNT, lastPagePulled, Context, Activity.UpdatePastAdapter);
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

                }
            }
        }

    }

}

