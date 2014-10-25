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
using System.Collections.ObjectModel;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using System.Threading.Tasks;
using RDN.WP.Library.Classes.Public;
using RDN.Portable.Util.Strings;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using Microsoft.Phone.Tasks;

namespace RDN.WP.View.MyLeague.Calendar
{
    public partial class ViewCalendarEvent : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        CalendarEventPortable calendarModel;


        public ViewCalendarEvent()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                if (e.NavigationMode == NavigationMode.New || e.NavigationMode == NavigationMode.Back)
                {
                    calendarModel = new CalendarEventPortable();

                    //(App.Current as App).SecondPageObject = null;
                    progressIndicator = SystemTray.ProgressIndicator;
                    progressIndicator = new ProgressIndicator();
                    SystemTray.SetProgressIndicator(this, progressIndicator);
                    progressIndicator.IsIndeterminate = true;
                    progressIndicator.Text = "Pulling Event...";
                    if (SettingsMobile.Instance.User == null)
                    {
                        SqlFactory fact = new SqlFactory();
                        SettingsMobile.Instance.User = fact.GetProfile();
                    }
                    if (!SettingsMobile.Instance.AccountSettings.IsEventsCoordinatorOrBetter)
                    {

                        ApplicationBarIconButton b = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                        b.IsEnabled = false;
                    }
                    calendarModel.CalendarItemId = new Guid(this.NavigationContext.QueryString["evId"]);
                    PullDues();
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
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
                    try
                    {
                        calendarModel = CalendarMobile.GetCalendarEvent(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, SettingsMobile.Instance.AccountSettings.CalendarId, calendarModel.CalendarItemId);


                        var checkedIn = calendarModel.Attendees.Where(x => x.PointType != CalendarEventPointTypeEnum.None).ToList();
                        var available = calendarModel.Attendees.Where(x => x.Availability != AvailibilityEnum.None).ToList();


                        Dispatcher.BeginInvoke(delegate
                        {
                            try
                            {
                                pivotTitle.Title = calendarModel.Name;
                                Description.Text = StringExt.HtmlDecode(calendarModel.NotesHtml);
                                StartDate.Text = calendarModel.StartDate.ToLongDateString();
                                StartTime.Text = calendarModel.StartDate.ToShortTimeString();
                                EndTime.Text = calendarModel.EndDate.ToShortDateString();
                                if (calendarModel.Location != null)
                                {
                                    Location.Text = calendarModel.Location.LocationName;
                                    if (calendarModel.Location.Contact.Addresses.FirstOrDefault() != null)
                                    {
                                        Address1.Text = calendarModel.Location.Contact.Addresses.FirstOrDefault().Address1;
                                        if (!String.IsNullOrEmpty(calendarModel.Location.Contact.Addresses.FirstOrDefault().Address2))
                                        {
                                            Address1.Text += "," + calendarModel.Location.Contact.Addresses.FirstOrDefault().Address2;
                                        }
                                        if (!String.IsNullOrEmpty(calendarModel.Location.Contact.Addresses.FirstOrDefault().CityRaw))
                                        {
                                            Address2.Text = calendarModel.Location.Contact.Addresses.FirstOrDefault().CityRaw;
                                        }
                                        if (!String.IsNullOrEmpty(calendarModel.Location.Contact.Addresses.FirstOrDefault().StateRaw))
                                        {
                                            Address2.Text += ", " + calendarModel.Location.Contact.Addresses.FirstOrDefault().StateRaw;
                                        }
                                        if (!String.IsNullOrEmpty(calendarModel.Location.Contact.Addresses.FirstOrDefault().Zip))
                                        {
                                            Address2.Text += " " + calendarModel.Location.Contact.Addresses.FirstOrDefault().Zip;
                                        }
                                        if (!String.IsNullOrEmpty(calendarModel.Location.Contact.Addresses.FirstOrDefault().Country))
                                        {
                                            Address3.Text = calendarModel.Location.Contact.Addresses.FirstOrDefault().Country;
                                        }
                                    }
                                }
                                RDNationUrl.Text = calendarModel.RDNationLink;
                                TicketUrl.Text = calendarModel.TicketUrl;
                                WebsiteUrl.Text = calendarModel.Link;
                                RosterList.ItemsSource = available;
                                checkedInList.ItemsSource = checkedIn;
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
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void TicketUrl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(calendarModel.TicketUrl);
            wbt.Show();
        }

        private void RDNationUrl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(calendarModel.RDNationLink);
            wbt.Show();
        }

        private void WebsiteUrl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri(calendarModel.Link);
            wbt.Show();
        }

        private void SetAvailability_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/SetAvailabilityForEvent.xaml?evId=" + calendarModel.CalendarItemId + "&calId=" + calendarModel.CalendarId + "&name=" + calendarModel.Name, UriKind.Relative));
        }

        private void CheckMembersIn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/CalendarCheckInMembers.xaml?evId=" + calendarModel.CalendarItemId + "&calId=" + calendarModel.CalendarId + "&name=" + calendarModel.Name, UriKind.Relative));
        }

    }
}