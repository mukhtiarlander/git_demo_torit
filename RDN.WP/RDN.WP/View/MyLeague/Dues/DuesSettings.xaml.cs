using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.WP.Library.Classes.League;
using System.Threading.Tasks;
using RDN.Portable.Settings;
using RDN.Portable.Classes.Controls.Dues;
using RDN.WP.Library.Database;
using Coding4Fun.Toolkit.Controls;
using RDN.Portable.Classes.Payment.Classes;

namespace RDN.WP.View.MyLeague.Dues
{
    public partial class DuesSettings : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        DuesPortableModel dues;
        Guid duesId = new Guid();
        public DuesSettings()
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
                    progressIndicator.Text = "Pulling Dues Settings...";
                    duesId = new Guid(this.NavigationContext.QueryString["dmid"]);
                    if (SettingsMobile.Instance.User == null)
                    {
                        SqlFactory fact = new SqlFactory();
                        SettingsMobile.Instance.User = fact.GetProfile();
                    }

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
                progressIndicator.Text = "Pulling Dues Settings...";
            });

            Task.Run(new Action(() =>
            {
                try
                {
                    dues = DuesMobile.GetDuesSettings(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, duesId);
                    Dispatcher.BeginInvoke(delegate
                    {
                        try
                        {
                            DayOfMonthToCollect.Text = dues.DayOfMonthToCollectDefault.ToString();
                            DaysBeforeToNotify.Text = dues.DaysBeforeDeadlineToNotifyDefault.ToString();
                            AmountToCollect.Text = dues.DuesCostDisplay;
                            DuesForManagementOnly.IsChecked = dues.LockDownManagementToManagersOnly;
                            PayPalPaymentsAllowed.IsChecked = dues.AcceptPaymentsOnline;
                            PaypalEmailAddress.Text = dues.PayPalEmailAddress;
                            CurrencySelect.ItemsSource = dues.Currencies;
                            CurrencySelect.SelectedItem = dues.Currencies.Where(x => x.CurrencyAbbrName == dues.Currency).FirstOrDefault();
                            progressIndicator.IsVisible = false;
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }
                    });
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                }
            }));
        }

        private void Save_Click(object sender, EventArgs e)
        {
            try
            {

                progressIndicator.Text = "Saving Settings...";
                progressIndicator.IsVisible = true;
                dues.CurrentMemberId = SettingsMobile.Instance.User.MemberId;
                dues.CurrentUserId = SettingsMobile.Instance.User.LoginId;
                if (CurrencySelect != null && CurrencySelect.SelectedItem != null)
                {
                    dues.Currency = ((CurrencyExchange)CurrencySelect.SelectedItem).CurrencyAbbrName;
                }
                dues.DayOfMonthToCollectDefault = Convert.ToInt32(DayOfMonthToCollect.Text);
                dues.DaysBeforeDeadlineToNotifyDefault = Convert.ToInt32(DaysBeforeToNotify.Text);
                dues.DuesCostDisplay = AmountToCollect.Text;
                dues.LockDownManagementToManagersOnly = DuesForManagementOnly.IsChecked.GetValueOrDefault();
                dues.AcceptPaymentsOnline = PayPalPaymentsAllowed.IsChecked.GetValueOrDefault();
                dues.PayPalEmailAddress = PaypalEmailAddress.Text;

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var m = DuesMobile.SaveDuesSettings(dues);
                        if (m.IsSuccessful)
                        {
                            Dispatcher.BeginInvoke(delegate
                            {
                                progressIndicator.IsVisible = false;
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Successfully Saved...",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                };
                                t.Show();
                            });
                        }
                        else
                        {
                            MessageBox.Show("Something went wrong, please check values and try again.");
                        }
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