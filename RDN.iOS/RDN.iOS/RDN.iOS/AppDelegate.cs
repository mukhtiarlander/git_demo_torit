using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using RDN.iOS.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Mobile.Database;
using RDN.Portable.Settings;

namespace RDN.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        UIWindow window;
        MainViewController viewController;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            try
            {
                window = new UIWindow(UIScreen.MainScreen.Bounds);

                SettingsMobile.Instance.User = new SqlFactory().CreateTables().GetProfile();
                if (SettingsMobile.Instance.User == null)
                    SettingsMobile.Instance.User = new Portable.Account.UserMobile();

                if (SettingsMobile.Instance.User.LastMobileLoginDate.AddDays(30) > DateTime.UtcNow)
                    SettingsMobile.Instance.User.IsLoggedIn = false;

                viewController = new MainViewController();
                var rootNavigationController = new UINavigationController();
                rootNavigationController.PushViewController(viewController, false);
                window.RootViewController = rootNavigationController;

                window.MakeKeyAndVisible();
               
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.iPhone);
            }
            return true;
        }
    }
}

