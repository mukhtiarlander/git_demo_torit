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
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.Activities.Calendar
{
    [Activity(Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class EventsActivity : LegacyBarActivity, ILocationListener
    {
        //static MyCharacterPickerDialog _dialog;
        //static String options = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        EventsAdapter ListAdapter;
        EventsJson initialArray;
        EditText search_events;
        ListView eventsList;
        public int PAGE_COUNT = 20;
        public string lastLetterPulled = "";
        public int lastPagePulled = 0;
        View m_AdView;
        private Location _currentLocation;
        private LocationManager _locationManager;
        private string _locationProvider;
        private bool FoundLocation;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                RequestWindowFeature(WindowFeatures.NoTitle);

                base.OnCreate(bundle);
                LoggerMobile.Instance.logMessage("Opening EventsActivity", LoggerEnum.message);
                SetContentView(Resource.Layout.PublicEvents);
                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                LegacyBar.SetHomeLogo(Resource.Drawable.icon);
                LegacyBar.SeparatorColor = Color.Purple;

                LegacyBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
                AddHomeAction(typeof(Main), Resource.Drawable.icon);
                // Get our button from the layout resource,
                // and attach an event to it

                Action<EventsJson> evs = new Action<EventsJson>(UpdateAdapter);
                LegacyBar.ProgressBarVisibility = ViewStates.Visible;

                eventsList = FindViewById<ListView>(Resource.Id.eventsList);
                initialArray = new EventsJson();
                Action pullMore = new Action(PullMore);
                RDNation.Droid.Classes.Public.Calendar.PullEvents(lastPagePulled, PAGE_COUNT, (Context)this, evs);
                ListAdapter = new EventsAdapter(this, initialArray.Events, pullMore);
                eventsList.Adapter = ListAdapter;
                eventsList.FastScrollEnabled = true;


                eventsList.ItemClick += skaterList_ItemClick;
                var myString = new SpannableStringBuilder("lol");
                Selection.SelectAll(myString); // needs selection or Index Out of bounds

                search_events = FindViewById<EditText>(Resource.Id.search_events);
                search_events.TextChanged += search_skaters_TextChanged;
                var searchMenuItemAction = new SearchAction(this, null, Resource.Drawable.ic_action_search, search_events);
                LegacyBar.AddAction(searchMenuItemAction);


                InitializeLocationManager();

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
        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            //adds the action after the first find.
            if (FoundLocation == false)
            {
                FoundLocation = true;
                RunOnUiThread(() =>
                {
                    var locationMenuAction = new LocationAction(this, null, Resource.Drawable.location_place, this);
                    LegacyBar.AddAction(locationMenuAction);
                });
            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }
        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
        }
        private void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            var criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            var acceptableLocationProviders = _locationManager.GetBestProvider(criteriaForLocationService, true);
            _locationProvider = acceptableLocationProviders;
        }
        void search_skaters_TextChanged(object sender, TextChangedEventArgs e)
        {
            lastPagePulled = 0;
            LegacyBar.ProgressBarVisibility = ViewStates.Visible;
            Action<EventsJson> evs = new Action<EventsJson>(UpdateAdapter);
            var t = (EditText)sender;
            if (t.Text.Length == 0)
            {
                RDNation.Droid.Classes.Public.Calendar.PullEvents(lastPagePulled, PAGE_COUNT, (Context)this, evs);
            }
            else if (t.Text.Length > 2)
            {
                RDNation.Droid.Classes.Public.Calendar.SearchEvents(lastPagePulled, PAGE_COUNT, t.Text, (Context)this, evs);
            }
            initialArray.Events.Clear();
        }
        void UpdateAdapter(EventsJson events)
        {
            initialArray.Events.AddRange(events.Events);
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
            Action<EventsJson> evs = new Action<EventsJson>(UpdateAdapter);
            LegacyBar.ProgressBarVisibility = ViewStates.Visible;
            RDNation.Droid.Classes.Public.Calendar.PullEvents(lastPagePulled, PAGE_COUNT, (Context)this, evs);
        }

        void skaterList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                var listView = sender as ListView;
                var t = initialArray.Events[e.Position];

                var intent = new Intent(this, typeof(EventActivity));
                var data = Json.ConvertToString<EventJson>(t);
                intent.PutExtra("event", data);
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
        private class LocationAction : LegacyBarAction
        {
            EventsActivity Activity;
            public LocationAction(Context context, Intent intent, int drawable, EventsActivity activity)
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

                    Activity.lastPagePulled += 1;
                    Action<EventsJson> evs = new Action<EventsJson>(Activity.UpdateAdapter);
                    Activity.LegacyBar.ProgressBarVisibility = ViewStates.Visible;
                    RDNation.Droid.Classes.Public.Calendar.PullEventsByLocation(Activity.lastPagePulled, Activity.PAGE_COUNT, Activity._currentLocation.Longitude, Activity._currentLocation.Latitude, Context, evs);
                    Activity.initialArray.Events.Clear();
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

                }
            }
        }

    }

}

