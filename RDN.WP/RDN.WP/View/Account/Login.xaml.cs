using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Library.Account;
using RDN.Portable.Account;
using System.Threading.Tasks;
using System.Windows.Data;
using RDN.WP.Library.Database;
using RDN.WP.Library.Database.Account;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.Account;

namespace RDN.WP.View.Account
{
    public partial class Login : PhoneApplicationPage
    {

        ProgressIndicator progressIndicator;
        public Login()
        {
            InitializeComponent();
            try
            {

                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Logging In...";

                SqlFactory fact = new SqlFactory();
                var user = fact.GetProfile();
                if (user != null)
                {
                    EmailAddress.Text = user.UserName;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserMobile m = new UserMobile();
                m.UserName = EmailAddress.Text;
                m.Password = Password.Text;
                progressIndicator.IsVisible = true;
                Task.Run(new Action(() =>
                    {
                        try
                        {
                            var user = UserMobileWP.Login(m);
                            if (user.IsLoggedIn)
                            {
                                user.LastMobileLoginDate = DateTime.UtcNow;
                                SqlFactory fact = new SqlFactory();
                                fact.DeleteProfile();
                                fact.InsertProfile(new SqlAccount(user));
                                var json = fact.GetNotificationSettings();
                                if (json != null)
                                {
                                    json.MemberId = user.MemberId.ToString();
                                    NotificationMobileJsonWP.SendNotificationId(json);
                                }
                                Dispatcher.BeginInvoke(delegate
                                {
                                    progressIndicator.IsVisible = false;
                                    NavigationService.Navigate(new Uri("/View/MyLeague/MyLeague.xaml", UriKind.Relative));

                                });
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(delegate
                                {
                                    progressIndicator.IsVisible = false;
                                    warning.Text = "There was an error logging in, Please try again.";
                                });

                            }
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;

                        });
                    }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Account/Signup.xaml", UriKind.Relative));
        }
    }
}