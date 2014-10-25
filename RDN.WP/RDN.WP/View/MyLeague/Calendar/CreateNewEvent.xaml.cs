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
using System.Threading.Tasks;
using RDN.WP.Library.Classes.League;
using RDN.Portable.Classes.League;
using RDN.Portable.Classes.League.Classes;
using System.Collections.ObjectModel;
using RDN.WP.Library.Classes.Public;
using RDN.Portable.Classes.Colors;
using System.Collections;
using RDN.Portable.Classes.Location;
using Coding4Fun.Toolkit.Controls;

namespace RDN.WP.View.MyLeague.Calendar
{
    public partial class CreateNewEvent : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        CalendarEventPortable calendarModel;
        ObservableCollection<LeagueGroup> Groups;
        ObservableCollection<CalendarEventType> EventTypes;
        ObservableCollection<ColorDisplay> Colors;
        ObservableCollection<Location> Locations;
        public CreateNewEvent()
        {
            InitializeComponent();
            GroupsList.SummaryForSelectedItemsDelegate = SummarizeItems;
        }
        /// <summary>
        /// summarizes the groups selected.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private string SummarizeItems(IList items)
        {
            if (items != null && items.Count > 0)
            {
                string summarizedString = "";
                for (int i = 0; i < items.Count; i++)
                {
                    summarizedString += ((LeagueGroup)items[i]).GroupName;

                    // If not last item, add a comma to seperate them
                    if (i != items.Count - 1)
                        summarizedString += ", ";
                }

                return summarizedString;
            }
            else
                return "Entire League";
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                if (e.NavigationMode == NavigationMode.New)
                {
                    calendarModel = new CalendarEventPortable();

                    //(App.Current as App).SecondPageObject = null;
                    progressIndicator = SystemTray.ProgressIndicator;
                    progressIndicator = new ProgressIndicator();
                    SystemTray.SetProgressIndicator(this, progressIndicator);
                    progressIndicator.IsIndeterminate = true;
                    progressIndicator.Text = "Pulling Info...";
                    if (SettingsMobile.Instance.User == null)
                    {
                        SqlFactory fact = new SqlFactory();
                        SettingsMobile.Instance.User = fact.GetProfile();
                    }
                    StartTime.Value = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0);
                    EndTime.Value = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0);
                    calendarModel.CalendarId = SettingsMobile.Instance.AccountSettings.CalendarId;
                    calendarModel.MemberId = SettingsMobile.Instance.User.MemberId;
                    calendarModel.UserId = SettingsMobile.Instance.User.LoginId;
                    LoadInitialData();
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        void LoadInitialData()
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.IsVisible = true;
            });
            try
            {
                LeagueMobile.PullLeagueGroups(SettingsMobile.Instance.User.MemberId.ToString(), SettingsMobile.Instance.User.LoginId.ToString(), new Action<List<LeagueGroupBasic>>(ReturnLeagueGroups));

                PopulateLocations();
                PopulateColors();
                PopulateEventTypes();
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }
        private void PopulateLocations()
        {
            Task.Run(new Action(() =>
            {
                try
                {
                    Locations = new ObservableCollection<Location>(LeagueMobile.GetLocations(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, SettingsMobile.Instance.AccountSettings.CalendarId));
                    Dispatcher.BeginInvoke(delegate
                    {
                        try
                        {
                            LocationList.ItemsSource = Locations;
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
        private void PopulateEventTypes()
        {
            Task.Run(new Action(() =>
            {
                try
                {
                    EventTypes = new ObservableCollection<CalendarEventType>(CalendarMobile.GetCalendarEventTypes(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, SettingsMobile.Instance.AccountSettings.CalendarId));
                    Dispatcher.BeginInvoke(delegate
                    {
                        try
                        {
                            eventTypeSelect.ItemsSource = EventTypes;
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

        private void PopulateColors()
        {
            Task.Run(new Action(() =>
            {
                try
                {
                    Colors = new ObservableCollection<ColorDisplay>(LeagueMobile.GetLeagueColors(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId));
                    Dispatcher.BeginInvoke(delegate
                    {
                        try
                        {
                            eventTypeColorSelect.ItemsSource = Colors;
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
        void ReturnLeagueGroups(List<LeagueGroupBasic> model)
        {
            try
            {
                Groups = new ObservableCollection<LeagueGroup>();
                var first = new LeagueGroup() { Id = 0, GroupName = "Entire League" };
                Groups.Add(first);
                for (int i = 0; i < model.Count; i++)
                {
                    Groups.Add(new LeagueGroup() { Id = model[i].Id, GroupName = model[i].GroupName });
                }
                Dispatcher.BeginInvoke(delegate
                {
                    GroupsList.ItemsSource = Groups;
                });
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(delegate
            {
                progressIndicator.Text = "Creating Event...";
                progressIndicator.IsVisible = true;
            });
            calendarModel.Name = EventName.Text;
            if (GroupsList.SelectedItems != null && GroupsList.SelectedItems.Count > 0)
            {
                for (int i = 0; i < GroupsList.SelectedItems.Count; i++)
                {
                    calendarModel.GroupsForEvent.Add((LeagueGroup)GroupsList.SelectedItems[i]);
                }
            }
            calendarModel.ColorTempSelected = ((ColorDisplay)eventTypeColorSelect.SelectedItem).HexColor;
            calendarModel.EndDate = new DateTime(EndDate.Value.GetValueOrDefault().Year, EndDate.Value.GetValueOrDefault().Month, EndDate.Value.GetValueOrDefault().Day, EndTime.Value.GetValueOrDefault().Hour, EndTime.Value.GetValueOrDefault().Minute, 0);
            calendarModel.StartDate = new DateTime(StartDate.Value.GetValueOrDefault().Year, StartDate.Value.GetValueOrDefault().Month, StartDate.Value.GetValueOrDefault().Day, StartTime.Value.GetValueOrDefault().Hour, StartTime.Value.GetValueOrDefault().Minute, 0);
            calendarModel.Link = WebsiteUrl.Text;
            calendarModel.TicketUrl = TicketUrl.Text;
            calendarModel.IsPublicEvent = PublicEvent.IsChecked.GetValueOrDefault();
            calendarModel.BroadcastEvent = BroadcastEvent.IsChecked.GetValueOrDefault();
            calendarModel.AllowSelfCheckIn = AllowSelfCheckIn.IsChecked.GetValueOrDefault();
            if (LocationList.SelectedItem != null)
            {
                calendarModel.Location = (Location)LocationList.SelectedItem;
            }
            if (eventTypeSelect.SelectedItem != null)
            {
                calendarModel.EventType = (CalendarEventType)eventTypeSelect.SelectedItem;
            }

            Task.Run(new Action(() =>
            {

                var tempModel = CalendarMobile.CreateNewEvent(calendarModel);
                Dispatcher.BeginInvoke(delegate
                {
                    if (tempModel.IsSuccessful)
                    {
                        ToastPrompt t = new ToastPrompt
                        {
                            Title = "Event Created",
                            TextOrientation = System.Windows.Controls.Orientation.Vertical
                        };
                        t.Show();
                        NavigationService.GoBack();
                    }
                    else
                    {
                        ToastPrompt t = new ToastPrompt
                        {
                            Title = "Something went wrong, please try again.",
                            TextOrientation = System.Windows.Controls.Orientation.Vertical
                        };
                        t.Show();
                    }
                    progressIndicator.IsVisible = true;
                });

            }));


        }

        private void StartDate_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime date = ((DatePicker)sender).Value.GetValueOrDefault();
            if (EndDate.Value.GetValueOrDefault() < date)
                EndDate.Value = date;
        }

        private void StartTime_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {

        }

        private void EndDate_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime date = ((DatePicker)sender).Value.GetValueOrDefault();
            if (StartDate.Value.GetValueOrDefault() > date)
                StartDate.Value = date;
        }

        private void EndTime_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {

        }

    }
}