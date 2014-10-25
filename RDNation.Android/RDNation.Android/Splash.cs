using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RDN.Mobile.Database;
using RDN.Utilities.Util;
using RDNation.Droid.Classes;
using RDNation.Droid.Classes.Configuration;
using RDNation.Droid.Classes.Services;
using RDN.Portable.Config.Enums;


namespace RDNation.Droid
{
    [Activity(Theme = "@style/Theme.Splash", ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true, NoHistory = true)]
    public class Splash : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            try
            {
                var user = Settings.GetUserPreferences((Context)this);
                LoggerMobile.Instance.logMessage(PackageManager.GetPackageInfo(PackageName, 0).VersionName, LoggerEnum.message);
                Task<bool>.Factory.StartNew(
                                      () =>
                                      {
                                          var f = new SqlFactory().CreateTables();
                                          LoggerMobile.Instance.logMessage("Opening Splash", LoggerEnum.message);
                                          if (!user.IsRegisteredForNotifications)
                                          {
                                              PushClient.CheckDevice(this);
                                              PushClient.CheckManifest(this);
                                              PushClient.Register(this, PushHandlerBroadcastReceiver.SENDER_IDS);
                                          }
                                          return true;
                                      });
                //ErrorManagerMobile.checkForOldErrors();
              
                StartActivity(typeof(Main));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.Android, (Context)this);

            }

        }
    }
}