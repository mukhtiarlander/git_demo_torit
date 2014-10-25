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
using RDN.Portable.Classes.Account.Enums;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.Account;

namespace RDN.WP.View.Account
{
    public partial class Signup : PhoneApplicationPage
    {

        ProgressIndicator progressIndicator;
        public Signup()
        {
            InitializeComponent();
            try
            {
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();

                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Signing Up...";

                SqlFactory fact = new SqlFactory();
                var user = fact.GetProfile();

                GenderSelect.ItemsSource = GenderHelper.GenderTypes;
                PositionSelect.ItemsSource = PositionHelper.PositionTypes;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void RegisterBtn_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                UserMobile m = new UserMobile();

                m.UserName = Email.Text;
                m.Password = Password.Text;
                m.FirstName = FirstName.Text;
                if (GenderSelect != null && GenderSelect.SelectedItem != null)
                    m.Gender = Convert.ToInt32((GenderEnum)Enum.Parse(typeof(GenderEnum), GenderSelect.SelectedItem.ToString().Replace(" ", "_")));

                if (PositionSelect != null && PositionSelect.SelectedItem != null)
                    m.Position = Convert.ToInt32((DefaultPositionEnum)Enum.Parse(typeof(DefaultPositionEnum), PositionSelect.SelectedItem.ToString().Replace(" ", "_")));

                m.IsConnectedToDerby = YesRadio.IsChecked.GetValueOrDefault();
                RegisterBtn.IsEnabled = false;

                progressIndicator.IsVisible = true;
                Task.Run(new Action(() =>
                    {
                        try
                        {
                            var user = UserMobileWP.SignUp(m);
                            if (user.DidSignUp)
                            {
                                SqlFactory fact = new SqlFactory();
                                fact.InsertProfile(new SqlAccount(user));
                                var json = fact.GetNotificationSettings();
                                json.MemberId = user.MemberId.ToString();
                                NotificationMobileJsonWP.SendNotificationId(json);
                                Dispatcher.BeginInvoke(delegate
                                {
                                    progressIndicator.IsVisible = false;
                                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                                });
                            }
                            else
                            {
                                Dispatcher.BeginInvoke(delegate
                                {
                                    progressIndicator.IsVisible = false;
                                    warning.Text = user.Error;
                                    RegisterBtn.IsEnabled = true;
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
                            RegisterBtn.IsEnabled = true;

                        });
                    }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


        private void YesRadio_Click(object sender, RoutedEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn.IsChecked.GetValueOrDefault())
            {
                DerbyName.Visibility = System.Windows.Visibility.Visible;
                DerbyNameText.Visibility = System.Windows.Visibility.Visible;
                FirstName.Visibility = System.Windows.Visibility.Visible;
                FirstNameText.Visibility = System.Windows.Visibility.Visible;
                GenderSelect.Visibility = System.Windows.Visibility.Visible;
                GenderText.Visibility = System.Windows.Visibility.Visible;
                PositionSelect.Visibility = System.Windows.Visibility.Visible;
                PositionText.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void NoRadio_Click(object sender, RoutedEventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn.IsChecked.GetValueOrDefault())
            {
                DerbyName.Visibility = System.Windows.Visibility.Collapsed;
                DerbyNameText.Visibility = System.Windows.Visibility.Collapsed;
                FirstName.Visibility = System.Windows.Visibility.Collapsed;
                FirstNameText.Visibility = System.Windows.Visibility.Collapsed;
                GenderSelect.Visibility = System.Windows.Visibility.Collapsed;
                GenderText.Visibility = System.Windows.Visibility.Collapsed;
                PositionSelect.Visibility = System.Windows.Visibility.Collapsed;
                PositionText.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

    }
}