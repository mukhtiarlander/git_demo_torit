using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Controls.Dues;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using System.Threading.Tasks;
using RDN.WP.Library.Classes.League;
using System.Collections.ObjectModel;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyLeague.Dues
{
    public partial class DuesMemberEdit : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        DuesMemberItem item;
        ObservableCollection<DuesCollected> Collected;
        long duesItemId = 0;
        Guid duesId = new Guid();
        Guid memId = new Guid();
        public DuesMemberEdit()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                item = new DuesMemberItem();
                //(App.Current as App).SecondPageObject = null;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Member Dues...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
                duesItemId = Convert.ToInt64(this.NavigationContext.QueryString["did"]);
                duesId = new Guid(this.NavigationContext.QueryString["dmid"]);
                memId = new Guid(this.NavigationContext.QueryString["mid"]);
                PullTopic();


            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }
        void PullTopic()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Pulling Member Dues...";
            });

            Task.Run(new Action(() =>
            {
                try
                {
                    item = DuesMobile.EditMemberDues(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId, duesItemId, memId);
                    Dispatcher.BeginInvoke(delegate
                    {
                        Title.Title = item.Member.DerbyName + ", Modify Dues";
                        DerbyName.Text = item.Member.DerbyName;
                        DueDate.Text = item.DuesItem.PayBy.ToLongDateString();
                        AmountDue.Text = item.DuesItem.CostOfDues.ToString("N2");
                        Collected = new ObservableCollection<DuesCollected>(item.DuesItem.DuesCollected.ToArray());
                        PaymentsList.ItemsSource = Collected;
                        progressIndicator.IsVisible = false;
                    });
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                }
            }));


        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Updating Member Dues...";
            });

            try
            {
                double am = Convert.ToDouble(AmountDue.Text);
                Task.Run(new Action(() =>
                {
                    try
                    {
                        var re = DuesMobile.SetDuesAmount(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId, duesItemId, am, memId);
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (re.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Updated...",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                };
                                t.Show();
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

        private void EmailReminder_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Sending Reminder...";
            });

            try
            {
                Task.Run(new Action(() =>
                {
                    try
                    {
                        var re = DuesMobile.SendEmailReminder(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId, duesItemId, memId);
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (re.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Reminded...",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                };
                                t.Show();
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

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mem = ((sender as Button).DataContext as DuesCollected);

                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Removing Dues Payment...";

                int index = Collected.IndexOf(mem);
                Collected.Remove(mem);
                PaymentsList.UpdateLayout();

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var duesParams = DuesMobile.RemoveDuesPayment(SettingsMobile.Instance.User.MemberId,
                            SettingsMobile.Instance.User.LoginId, duesId, duesItemId, mem.DuesCollectedId, memId);
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (duesParams.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Removed...",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                };
                                t.Show();
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

    }
}