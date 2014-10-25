using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.BarActions;
using RDN.Mobile.Config.Enums;
using RDNation.Droid.Classes;

namespace RDNation.Droid.UI.ActionBarAction
{
    public  class AddEventToCalAction : LegacyBarAction
    {
        string Url;
        public AddEventToCalAction(Context context, Intent intent, int drawable, string url)
        {
            Drawable = drawable;
            Context = context;
            Intent = intent;
            Url = url;
        }

        public override int GetDrawable()
        {
            return Drawable;
        }

        public override void PerformAction(View view)
        {
            try
            {
                ContentValues eventValues = new ContentValues();

                eventValues.Put( CalendarContract.Events.InterfaceConsts.CalendarId,
                    _calId);
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Title,
                    "Test Event from M4A");
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Description,
                    "This is an event created from Xamarin.Android");
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtstart,
                    GetDateTimeMS(2011, 12, 15, 10, 0));
                eventValues.Put(CalendarContract.Events.InterfaceConsts.Dtend,
                    GetDateTimeMS(2011, 12, 15, 11, 0));

                var uri = ContentResolver.Insert(CalendarContract.Events.ContentUri,
                    eventValues);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

            }
        }
    }
}