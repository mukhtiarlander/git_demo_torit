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
using Coding4Fun.Toolkit.Controls;
using RDN.Portable.Classes.Account.Classes;
using System.Collections.ObjectModel;

namespace RDN.WP.View.MyLeague.Dues
{
    public partial class DuesItemEdit : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        DuesPortableModel dues;
        DuesItem item;
        ObservableCollection<MemberDisplayDues> Members;
        long duesItemId = 0;
        Guid duesId = new Guid();
        public DuesItemEdit()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    dues = new DuesPortableModel();
                    //(App.Current as App).SecondPageObject = null;
                    progressIndicator = SystemTray.ProgressIndicator;
                    progressIndicator = new ProgressIndicator();
                    SystemTray.SetProgressIndicator(this, progressIndicator);
                    progressIndicator.IsIndeterminate = true;
                    progressIndicator.Text = "Pulling Dues Item...";
                    if (SettingsMobile.Instance.User == null)
                    {
                        SqlFactory fact = new SqlFactory();
                        SettingsMobile.Instance.User = fact.GetProfile();
                    }
                    duesItemId = Convert.ToInt64(this.NavigationContext.QueryString["did"]);
                    duesId = new Guid(this.NavigationContext.QueryString["dmid"]);
                    PullTopic();
                }
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
                progressIndicator.Text = "Pulling Dues Item...";
            });

            Task.Run(new Action(() =>
            {
                try
                {
                    dues = DuesMobile.GetDuesItem(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId, duesItemId);
                    item = dues.DuesFees.FirstOrDefault();
                    Members = new ObservableCollection<MemberDisplayDues>(dues.Members);
                    Dispatcher.BeginInvoke(delegate
                    {
                        Title.Title = item.PayBy.ToLongDateString();
                        DueDate.Value = item.PayBy;
                        AmountDue.Text = item.CostOfDues.ToString("N2");
                        RosterList.ItemsSource = Members;

                        progressIndicator.IsVisible = false;
                    });
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                }
            }));


        }



        private void SendEmailReminders_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Sending Reminders...";
            });

            Task.Run(new Action(() =>
            {
                try
                {
                    var duesParams = DuesMobile.SendEmailReminderAll(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId, duesItemId);
                    Dispatcher.BeginInvoke(delegate
                    {
                        progressIndicator.IsVisible = false;
                        if (duesParams.IsSuccessful)
                        {
                            ToastPrompt t = new ToastPrompt
                            {
                                Title = "Successfully Sent...",
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


        private void PaidButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mem = ((sender as Button).DataContext as MemberDisplayDues);

                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Paying Dues...";

                int index = Members.IndexOf(mem);
                Members.Remove(mem);

                mem.collected += mem.tempDue;
                mem.due -= mem.tempDue;
                if (mem.due <= 0.0)
                    mem.isPaidFull = true;

                RosterList.UpdateLayout();
                Members.Insert(index, mem);
                RosterList.UpdateLayout();

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var duesParams = DuesMobile.PayDuesAmount(SettingsMobile.Instance.User.MemberId,
                            SettingsMobile.Instance.User.LoginId, duesId, duesItemId, mem.tempDue, "", mem.MemberId);
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (duesParams.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = mem.DerbyName + " Paid...",
                                    MillisecondsUntilHidden = 1500,
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

        private void WaiveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mem = ((sender as Button).DataContext as MemberDisplayDues);

                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Waiving Dues...";

                int index = Members.IndexOf(mem);
                Members.Remove(mem);

                mem.isWaived = true;
                mem.isPaidFull = true;

                RosterList.UpdateLayout();
                Members.Insert(index, mem);
                RosterList.UpdateLayout();

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var duesParams = DuesMobile.WaiveDuesAmount(SettingsMobile.Instance.User.MemberId,
                            SettingsMobile.Instance.User.LoginId, duesId, duesItemId, mem.MemberId, "");
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (duesParams.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = mem.DerbyName + " Waived...",
                                    MillisecondsUntilHidden = 1500,
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

        private void SendReminder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mem = ((sender as Button).DataContext as MemberDisplayDues);

                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Sending Reminder...";

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var duesParams = DuesMobile.SendEmailReminder(SettingsMobile.Instance.User.MemberId,
                            SettingsMobile.Instance.User.LoginId, duesId, duesItemId, mem.MemberId);
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (duesParams.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = mem.DerbyName + " Reminded...",
                                    MillisecondsUntilHidden = 1500,
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

        private void UnWaiveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mem = ((sender as Button).DataContext as MemberDisplayDues);

                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Clearing Waived Dues...";

                int index = Members.IndexOf(mem);
                Members.Remove(mem);

                mem.isWaived = false;
                mem.isPaidFull = false;

                RosterList.UpdateLayout();
                Members.Insert(index, mem);
                RosterList.UpdateLayout();

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var duesParams = DuesMobile.WaiveRemoveDuesAmount(SettingsMobile.Instance.User.MemberId,
                            SettingsMobile.Instance.User.LoginId, duesId, duesItemId, mem.MemberId);
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (duesParams.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = mem.DerbyName + " Cleared...",
                                    MillisecondsUntilHidden = 1500,
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


        private void DueAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var box = (sender as TextBox);
                double amount = Convert.ToDouble(box.Text);
                var mem = (box.DataContext as MemberDisplayDues);
                mem.tempDue = amount;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }



        private void EditDuesForMember_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as MemberDisplayDues;
            NavigationService.Navigate(new Uri("/View/MyLeague/Dues/DuesMemberEdit.xaml?did=" + duesItemId + "&dmid=" + duesId + "&mid=" + blah.MemberId, UriKind.Relative));

        }

        private void SaveDues_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
                progressIndicator.Text = "Saving Dues Item...";
            });

            try
            {
                double am = Convert.ToDouble(AmountDue.Text);
                DateTime payBy = DueDate.Value.GetValueOrDefault();
                Task.Run(new Action(() =>
                {
                    try
                    {
                        var re = DuesMobile.EditDuesItem(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId, duesItemId, am, payBy);
                        Dispatcher.BeginInvoke(delegate
                        {
                            progressIndicator.IsVisible = false;
                            if (re.IsSuccessful)
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Saved...",
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

        private void DeleteDuesItem_Click(object sender, EventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Delete Dues Item?",
                Message = "Are you sure you want to delete this dues item?",
                LeftButtonContent = "yes",
                RightButtonContent = "no"
            };

            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        Dispatcher.BeginInvoke(delegate
           {
               progressIndicator.IsVisible = true;
               progressIndicator.Text = "Deleting Dues Item...";
           });

                        try
                        {
                            Task.Run(new Action(() =>
                            {
                                try
                                {
                                    var re = DuesMobile.DeleteDuesItem(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId, duesItemId);
                                    Dispatcher.BeginInvoke(delegate
                                    {
                                        progressIndicator.IsVisible = false;
                                        if (re.IsSuccessful)
                                        {
                                            ToastPrompt t = new ToastPrompt
                                            {
                                                Title = "Deleted...",
                                                TextOrientation = System.Windows.Controls.Orientation.Vertical
                                            };
                                            t.Show();
                                            NavigationService.Navigate(new Uri("/View/MyLeague/Dues/Dues.xaml", UriKind.Relative));
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
                        break;
                    case CustomMessageBoxResult.RightButton:
                        // Do something.
                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            messageBox.Show();


        }

    }
}