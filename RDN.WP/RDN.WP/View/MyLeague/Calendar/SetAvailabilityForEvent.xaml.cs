using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.WP.Library.Classes.Public;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyLeague.Calendar
{
    public partial class SetAvailabilityForEvent : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        CalendarEventPortable ev;
        public SetAvailabilityForEvent()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                ev = new CalendarEventPortable();
                var ob = (App.Current as App).SecondPageObject;
                if (ob != null)
                    ev = (CalendarEventPortable)ob;
                (App.Current as App).SecondPageObject = null;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Saving Availability...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
                CheckInTypeSelect.ItemsSource = AvailibilityEnumHelper.AvailibilityEnumTypes;
                ev.CalendarItemId = new Guid(this.NavigationContext.QueryString["evId"]);
                ev.CalendarId = new Guid(this.NavigationContext.QueryString["calId"]);
                ev.Name = this.NavigationContext.QueryString["name"];
                if (!String.IsNullOrEmpty(ev.Name))
                    EventName.Text = ev.Name;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }

        }

        private void SavePost_Click(object sender, EventArgs e)
        {
            try
            {
                var pointType = AvailibilityEnum.None;
                string notes = Notes.Text;

                if (CheckInTypeSelect != null && CheckInTypeSelect.SelectedItem != null)
                    pointType = (AvailibilityEnum)Enum.Parse(typeof(AvailibilityEnum), CheckInTypeSelect.SelectedItem.ToString().Replace(" ", "_"));

                progressIndicator.Text = "Setting Availability...";
                progressIndicator.IsVisible = true;
                Task.Run(new Action(() =>
                {
                    try
                    {
                        var cal = CalendarMobile.SetAvailabilityForEvent(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, ev.CalendarId, ev.CalendarItemId, notes, pointType);

                        if (cal.IsSuccessful)
                        {

                            Dispatcher.BeginInvoke(delegate
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Availability Set",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                };
                                t.Show();
                                progressIndicator.IsVisible = false;
                                NavigationService.GoBack();
                            });
                        }
                        else
                        {

                            Dispatcher.BeginInvoke(delegate
                            {
                                ToastPrompt t = new ToastPrompt
                                {
                                    Title = "Something happened, try again later.",
                                    TextOrientation = System.Windows.Controls.Orientation.Vertical
                                };
                                t.Show();
                                progressIndicator.IsVisible = false;
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
                        ToastPrompt t = new ToastPrompt
                        {
                            Title = "Something happened, try again later.",
                            TextOrientation = System.Windows.Controls.Orientation.Vertical
                        };
                        t.Show();

                    });
                }));
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
    }
}