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
using RDN.WP.Library.Classes.League;
using System.Threading.Tasks;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;

namespace RDN.WP.View.MyLeague.Dues
{
    public partial class DuesReceipt : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        RDN.Portable.Classes.Controls.Dues.DuesReceipt dues;
        Guid invoiceId = new Guid();
        public DuesReceipt()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                dues = new Portable.Classes.Controls.Dues.DuesReceipt();
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Getting Receipt...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
                invoiceId = new Guid(this.NavigationContext.QueryString["ivId"]);
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
                progressIndicator.Text = "Getting Receipt...";
            });

            Task.Run(new Action(() =>
            {
                try
                {
                    dues = DuesMobile.GetReceipt(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, invoiceId);
                    Dispatcher.BeginInvoke(delegate
                    {
                        InvoiceId.Text = dues.InvoiceId.ToString().Replace("-", "");
                        DueDate.Text = dues.PaidDuesForMonth.ToShortDateString();
                        DuesPaid.Text = dues.BasePrice.ToString("N2");
                        FeesPaid.Text = dues.Fees.ToString("N2");
                        TotalPaid.Text = dues.PriceAfterFees.ToString("N2");
                        ReceiptEmailedTo.Text = dues.EmailForReceipt;

                        progressIndicator.IsVisible = false;
                    });
                }
                catch (Exception exception)
                {
                    ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                }
            }));


        }
    }
}