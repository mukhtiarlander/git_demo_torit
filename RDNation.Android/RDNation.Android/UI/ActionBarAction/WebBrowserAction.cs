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
using RDNation.Droid.Classes;
using RDN.Portable.Config.Enums;

namespace RDNation.Droid.UI.ActionBarAction
{
    public  class WebBrowserAction : LegacyBarAction
    {
        string Url;
        public WebBrowserAction(Context context, Intent intent, int drawable, string url)
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

                var uri = Android.Net.Uri.Parse(Url);
                var intent = new Intent(Intent.ActionView, uri);
                Context.StartActivity(intent);
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, Context);

            }
        }
    }
}