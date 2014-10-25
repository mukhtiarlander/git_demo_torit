using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Account.Classes;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Settings;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using RDN.WP.Classes.UI.Roster;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using RDN.Portable.Classes.Controls.Dues;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;
using RDN.Portable.Classes.Payment.Enums;

namespace RDN.WP.View.MyLeague.Dues
{
    public partial class Dues : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        DuesPortableModel duesModel;
        public Dues()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                duesModel = new DuesPortableModel();

                //(App.Current as App).SecondPageObject = null;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Dues...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
                if (SettingsMobile.Instance.AccountSettings.IsTreasurer)
                {
                    ApplicationBar.IsVisible = true;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }


            PullDues();
        }
        void PullDues()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
            });
            try
            {
                Task.Run(new Action(() =>
                {
                    duesModel = DuesMobile.GetDuesManagement(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId);

                    Dispatcher.BeginInvoke(delegate
                    {
                       
                        DuesList.ItemsSource = duesModel.DuesFees;
                        progressIndicator.IsVisible = false;
                    });

                }));

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void ViewDues_Click(object sender, RoutedEventArgs e)
        {

            var blah = ((MenuItem)sender).DataContext as DuesItem;
            NavigationService.Navigate(new Uri("/View/MyLeague/Dues/DuesItemView.xaml?did=" + blah.DuesItemId + "&dmid=" + duesModel.DuesId, UriKind.Relative));

        }

        private void EditDues_Click(object sender, RoutedEventArgs e)
        {

            var blah = ((MenuItem)sender).DataContext as DuesItem;
            NavigationService.Navigate(new Uri("/View/MyLeague/Dues/DuesItemEdit.xaml?did=" + blah.DuesItemId + "&dmid=" + duesModel.DuesId, UriKind.Relative));

        }

        private void DuesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    if (e.AddedItems.Count > 0)
            //    {
            //        var topic = (DuesItem)e.AddedItems[0];
            //        NavigationService.Navigate(new Uri("/View/MyLeague/Dues/DuesItemView.xaml?did=" + topic.DuesItemId + "&dmid=" +duesModel.DuesId, UriKind.Relative));

            //    }
            //}
            //catch (Exception exception)
            //{
            //    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            //}
        }

        private void PayBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mem = ((sender as Button).DataContext as DuesItem);

                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Redirecting To Paypal...";


                Task.Run(new Action(() =>
                {
                    try
                    {
                        var duesParams = DuesMobile.PayDuesPersonally(SettingsMobile.Instance.User.MemberId,
                            SettingsMobile.Instance.User.LoginId, duesModel.DuesId, mem.DuesItemId);
                        Dispatcher.BeginInvoke(delegate
                        {

                            progressIndicator.IsVisible = false;

                            if (duesParams.Status == InvoiceStatus.Paypal_Email_Not_Confirmed)
                            {
                                CustomMessageBox messageBox = new CustomMessageBox()
                                {
                                    Caption = "Paypal Email Not Confirmed",
                                    Message = "Your leagues paypal email address is not confirmed.  Please make sure its correct and try again.",
                                    LeftButtonContent = "ok"
                                };
                                messageBox.Show();
                            }
                            else if (duesParams.Status == InvoiceStatus.Pending_Payment_From_Paypal)
                            {
                                WebBrowserTask wb = new WebBrowserTask();
                                wb.Uri = new Uri(duesParams.RedirectLink, UriKind.Absolute);
                                wb.Show();
                            }
                            else
                            {
                                CustomMessageBox messageBox = new CustomMessageBox()
                                {
                                    Caption = "Something Failed",
                                    Message = "Payment Failed.  Problem Sent to Developers.",
                                    LeftButtonContent = "ok"
                                };
                                messageBox.Show();
                            }
                        });
                    }
                    catch (Exception exception)
                    {
                        ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                    }
                }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MyLeague/Dues/DuesSettings.xaml?dmid=" + duesModel.DuesId, UriKind.Relative));

        }

        //private void RosterButton_Click(object sender, RoutedEventArgs e)
        //{

        //    LeagueRoster.SelectedIndex = 0;
        //    RosterList.ItemTemplate = (DataTemplate)Resources["RosterTemplate"];
        //}

        //private void DatesButton_Click(object sender, RoutedEventArgs e)
        //{
        //    LeagueRoster.SelectedIndex = 0;
        //    RosterList.ItemTemplate = (DataTemplate)Resources["DatesTemplate"];

        //}

        //private void JobsButton_Click(object sender, RoutedEventArgs e)
        //{

        //    LeagueRoster.SelectedIndex = 0;
        //    RosterList.ItemTemplate = (DataTemplate)Resources["JobsTemplate"];
        //}

        //private void InsuranceButton_Click(object sender, RoutedEventArgs e)
        //{

        //    LeagueRoster.SelectedIndex = 0;
        //    RosterList.ItemTemplate = (DataTemplate)Resources["InsuranceTemplate"];
        //}

        //private void SkatingLevelButton_Click(object sender, RoutedEventArgs e)
        //{

        //    LeagueRoster.SelectedIndex = 0;
        //    RosterList.ItemTemplate = (DataTemplate)Resources["SkatingLevelTemplate"];
        //}



    }
}