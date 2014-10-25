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
using RDN.WP.Library.Classes.Public;
using System.Collections.ObjectModel;
using RDN.Portable.Util.Strings;
using Coding4Fun.Toolkit.Controls;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using Microsoft.Phone.Tasks;

namespace RDN.WP.View.MyLeague.Calendar
{

    public partial class CalendarCheckInMembers : PhoneApplicationPage
    {
        ProgressIndicator progressIndicator;
        CalendarEventPortable calendarModel;
        ObservableCollection<CalendarAttendance> Members;
        public CalendarCheckInMembers()
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
                    //if (SettingsMobile.Instance.AccountSettings.IsEventsCoordinatorOrBetter)
                    //{
                    //    ApplicationBar.IsVisible = true;
                    //}
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
                    calendarModel = CalendarMobile.GetCalendarEvent(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, SettingsMobile.Instance.AccountSettings.CalendarId, calendarModel.CalendarItemId);

                    Members = new ObservableCollection<CalendarAttendance>();
                    for (int i = 0; i < calendarModel.Attendees.Count; i++)
                        Members.Add(calendarModel.Attendees[i]);
                    for (int i = 0; i < calendarModel.MembersToCheckIn.Count; i++)
                        Members.Add(calendarModel.MembersToCheckIn[i]);

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
                            RosterList.ItemsSource = Members;
                            progressIndicator.IsVisible = false;
                        }
                        catch (Exception exception)
                        {
                            ErrorHandler.Save(exception, MobileTypeEnum.WP8);
                        }



                    });

                }));

            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


        private void NoteInput_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((TextBox)sender).Text.Equals("Notes...", StringComparison.OrdinalIgnoreCase))
                {
                    ((TextBox)sender).Text = string.Empty;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }


        private void NoteInput_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(((TextBox)sender).Text))
                {
                    ((TextBox)sender).Text = "Notes...";
                }
                else
                {
                    var blah = ((TextBox)sender).DataContext as CalendarAttendance;
                    var mem = Members.Where(x => x.MemberId == blah.MemberId).FirstOrDefault();
                    mem.Note = ((TextBox)sender).Text;
                }
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void RemoveCheckIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                progressIndicator.Text = "Removing Checkin...";
                progressIndicator.IsVisible = true;
                var mem = ((Button)sender).DataContext as CalendarAttendance;

                int index = Members.IndexOf(mem);
                if (index == 0)
                    index = 1;
                Members.Remove(mem);
                mem.IsCheckedIn = false;
                mem.Note = "";
                mem.SecondaryPointType = CalendarEventPointTypeEnum.None;
                mem.PointType = CalendarEventPointTypeEnum.None;
                mem.PointsStringForReading = String.Empty;

                RosterList.UpdateLayout();
                Members.Insert(index, mem);
                RosterList.UpdateLayout();




                Task.Run(new Action(() =>
                {
                    try
                    {
                        var cal = CalendarMobile.RemoveCheckInEvent(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, calendarModel.CalendarId, calendarModel.CalendarItemId, mem.MemberId);
                        Dispatcher.BeginInvoke(delegate
                        {
                            try
                            {
                                if (cal.IsSuccessful)
                                {
                                    ToastPrompt t = new ToastPrompt
                                    {
                                        Title = "Removed " + mem.MemberName + " CheckIn",
                                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                                    };
                                    t.Show();

                                }
                                else
                                {
                                    ToastPrompt t = new ToastPrompt
                                    {
                                        Title = "Something happened, try again later.",
                                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                                    };
                                    t.Show();

                                }
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

        private void SaveCheckin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mem = ((Button)sender).DataContext as CalendarAttendance;
                int index = Members.IndexOf(mem);
                Members.Remove(mem);
                if (index == 0)
                    index = 1;

                progressIndicator.Text = "Checking In...";
                progressIndicator.IsVisible = true;
                int additionalPoints = 0;
                if (!String.IsNullOrEmpty(AdditionalPtsText.Text))
                    additionalPoints = Convert.ToInt32(AdditionalPtsText.Text);
                bool isTardy = false;
                mem.PointType = CalendarEventPointTypeEnum.None;
                if (PresentBtn.IsChecked.GetValueOrDefault())
                    mem.PointType = CalendarEventPointTypeEnum.Present;
                else if (PartialBtn.IsChecked.GetValueOrDefault())
                    mem.PointType = CalendarEventPointTypeEnum.Partial;
                else if (ExcusedBtn.IsChecked.GetValueOrDefault())
                    mem.PointType = CalendarEventPointTypeEnum.Excused;
                else if (NotPresentBtn.IsChecked.GetValueOrDefault())
                    mem.PointType = CalendarEventPointTypeEnum.Not_Present;

                if (mem.SecondaryPointType == CalendarEventPointTypeEnum.Tardy)
                    isTardy = true;



                mem.IsCheckedIn = true;

                RosterList.UpdateLayout();
                Members.Insert(index, mem);
                RosterList.UpdateLayout();

                Task.Run(new Action(() =>
                {
                    try
                    {
                        var cal = CalendarMobile.CheckMemberIntoEvent(SettingsMobile.Instance.User.MemberId, SettingsMobile.Instance.User.LoginId, calendarModel.CalendarId, calendarModel.CalendarItemId, mem.Note, isTardy, mem.PointType, mem.MemberId, additionalPoints);
                        Dispatcher.BeginInvoke(delegate
                        {
                            try
                            {
                                if (cal.IsSuccessful)
                                {
                                    ToastPrompt t = new ToastPrompt
                                    {
                                        Title = mem.MemberName + " Checked In",
                                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                                    };
                                    t.Show();

                                }
                                else
                                {
                                    ToastPrompt t = new ToastPrompt
                                    {
                                        Title = "Something happened, try again later.",
                                        TextOrientation = System.Windows.Controls.Orientation.Vertical
                                    };
                                    t.Show();

                                }
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

        private void SaveCheckInTardy_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var blah = ((CheckBox)sender).DataContext as CalendarAttendance;
                var mem = Members.Where(x => x.MemberId == blah.MemberId).FirstOrDefault();
                mem.SecondaryPointType = CalendarEventPointTypeEnum.Tardy;
            }
            catch (Exception exception)
            {
                ErrorHandler.Save(exception, MobileTypeEnum.WP8);
            }
        }

        private void SaveCheckInTardy_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var blah = ((CheckBox)sender).DataContext as CalendarAttendance;
                var mem = Members.Where(x => x.MemberId == blah.MemberId).FirstOrDefault();
                mem.SecondaryPointType = CalendarEventPointTypeEnum.None;
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
    }
}