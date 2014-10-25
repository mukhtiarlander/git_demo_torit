using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RDN.Portable.Settings;
using RDN.WP.Library.Database;
using RDN.WP.Classes.Error;
using RDN.Portable.Config.Enums;
using System.Threading.Tasks;
using RDN.WP.Library.Classes.Public;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.Controls.Calendar;

namespace RDN.WP.View.MyLeague.Calendar
{
    public partial class CalendarList : PhoneApplicationPage
    {

        ProgressIndicator progressIndicator;
        RDN.Portable.Classes.Controls.Calendar.Calendar calendarModel;
        public CalendarList()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {

                calendarModel = new Portable.Classes.Controls.Calendar.Calendar();

                //(App.Current as App).SecondPageObject = null;
                progressIndicator = SystemTray.ProgressIndicator;
                progressIndicator = new ProgressIndicator();
                SystemTray.SetProgressIndicator(this, progressIndicator);
                progressIndicator.IsIndeterminate = true;
                progressIndicator.Text = "Pulling Events...";
                if (SettingsMobile.Instance.User == null)
                {
                    SqlFactory fact = new SqlFactory();
                    SettingsMobile.Instance.User = fact.GetProfile();
                }
                if (SettingsMobile.Instance.AccountSettings.IsEventsCoordinatorOrBetter)
                {
                    ApplicationBar.IsVisible = true;
                }
                PullDues();

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
                    calendarModel = CalendarMobile.GetCalendarEvents(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, SettingsMobile.Instance.AccountSettings.CalendarId, CalendarOwnerEntityEnum.league, DateTime.UtcNow.Year, DateTime.UtcNow.Month);

                    Dispatcher.BeginInvoke(delegate
                    {
                        EventList.ItemsSource = calendarModel.Events;
                        progressIndicator.IsVisible = false;
                    });

                }));

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void CheckIn_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as CalendarEventPortable;
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/CheckSelfIntoEvent.xaml?evId=" + blah.CalendarItemId + "&calId=" + calendarModel.CalendarId + "&name=" + blah.Name, UriKind.Relative));

        }

        private void CheckInMembers_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as CalendarEventPortable;
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/CalendarCheckInMembers.xaml?evId=" + blah.CalendarItemId + "&calId=" + calendarModel.CalendarId + "&name=" + blah.Name, UriKind.Relative));

        }

        private void RSVP_Click(object sender, RoutedEventArgs e)
        {
            var blah = ((MenuItem)sender).DataContext as CalendarEventPortable;
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/SetAvailabilityForEvent.xaml?evId=" + blah.CalendarItemId + "&calId=" + calendarModel.CalendarId + "&name=" + blah.Name, UriKind.Relative));


        }

        private void EventList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var ev = (CalendarEventPortable)e.AddedItems[0];
                NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/ViewCalendarEvent.xaml?evId=" + ev.CalendarItemId + "&calId=" + calendarModel.CalendarId + "&name=" + ev.Name, UriKind.Relative));

            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MyLeague/Calendar/CreateNewEvent.xaml", UriKind.Relative));
        }
    }
}