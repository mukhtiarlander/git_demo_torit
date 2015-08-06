using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.League.Models.Filters;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Calendar.Enums;
using RDN.Library.Cache;
using RDN.Library.Classes.Location;
using RDN.League.Models.Calendar;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Account.Classes;
using RDN.Utilities.Dates;
using RDN.League.Models.Utilities;
using ScheduleWidget.ScheduledEvents;
using RDN.League.Models.Enum;
using System.Collections.Specialized;
using RDN.Library.Classes.Calendar.Report;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using OfficeOpenXml;
using System.Data;
using RDN.Utilities.Strings;
using RDN.Library.Classes.Controls.Calendar.Models;
using RDN.Library.Classes.Colors;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Portable.Classes.Account.Enums.Settings;
using ScheduleWidget.Enums;
using RDN.Library.Classes.Config;

namespace RDN.League.Controllers
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class CalendarController : BaseController
    {

        #region Events
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsSecretary = true, IsManager = true, IsEventCourdinator = true)]
        public ActionResult ExportEventRoster(NewCalendarEvent calEvent)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var league = MemberCache.GetLeagueOfMember(memId);

            var calEventTemp = CalendarEventFactory.GetEvent(calEvent.CalendarItemId, memId, calEvent.CalendarId);

            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = LibraryConfig.WebsiteShortName;
                    p.Workbook.Properties.Title = "Event Roster " + calEventTemp.Name + " - " + calEventTemp.StartDate.ToString("yyyy-M-d");

                    //we create the first sheet.
                    ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Roster");
                    reportSheet.Name = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly("Roster " + calEventTemp.Name + " - " + calEventTemp.StartDate.ToString("yyyy-M-d")); //Setting Sheet's name
                    reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    reportSheet.Cells[1, 1].Value = RDN.Library.Classes.Config.LibraryConfig.MemberName;
                    reportSheet.Cells[1, 2].Value = "#";
                    reportSheet.Cells[1, 3].Value = "Full Name";
                    reportSheet.Cells[1, 4].Value = "Present-" + calEventTemp.EventType.PointsForNotPresent;
                    reportSheet.Cells[1, 5].Value = "Partial-" + calEventTemp.EventType.PointsForPartial;
                    reportSheet.Cells[1, 6].Value = "Not Present-" + calEventTemp.EventType.PointsForNotPresent;
                    reportSheet.Cells[1, 7].Value = "Excused-" + calEventTemp.EventType.PointsForExcused;
                    reportSheet.Cells[1, 8].Value = "Tardy-" + calEventTemp.EventType.PointsForTardy;
                    reportSheet.Cells[1, 9].Value = "Addnl Pnts.";
                    reportSheet.Cells[1, 10].Value = "Notes";

                    int rowReport = 2;
                    foreach (var attendee in calEventTemp.Attendees)
                    {
                        try
                        {
                            reportSheet.Cells[rowReport, 1].Value = attendee.MemberName;
                            reportSheet.Cells[rowReport, 2].Value = attendee.MemberNumber;
                            reportSheet.Cells[rowReport, 3].Value = attendee.FullName;

                            switch (attendee.PointType)
                            {
                                case CalendarEventPointTypeEnum.Present:
                                    reportSheet.Cells[rowReport, 4].Value = "X";
                                    break;
                                case CalendarEventPointTypeEnum.Partial:
                                    reportSheet.Cells[rowReport, 5].Value = "X";
                                    break;
                                case CalendarEventPointTypeEnum.Not_Present:
                                    reportSheet.Cells[rowReport, 6].Value = "X";
                                    break;
                                case CalendarEventPointTypeEnum.Excused:
                                    reportSheet.Cells[rowReport, 7].Value = "X";
                                    break;
                                case CalendarEventPointTypeEnum.Tardy:
                                    reportSheet.Cells[rowReport, 8].Value = "X";
                                    break;
                            }
                            reportSheet.Cells[rowReport, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            reportSheet.Cells[rowReport, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            reportSheet.Cells[rowReport, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            reportSheet.Cells[rowReport, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            reportSheet.Cells[rowReport, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            if (attendee.AdditionalPoints != 0)
                                reportSheet.Cells[rowReport, 9].Value = attendee.AdditionalPoints;
                            reportSheet.Cells[rowReport, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            reportSheet.Cells[rowReport, 10].Value = attendee.Note;

                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    //create the remaining sheets with the names.
                    foreach (var attendee in calEventTemp.MembersToCheckIn)
                    {
                        try
                        {
                            reportSheet.Cells[rowReport, 1].Value = attendee.MemberName;
                            reportSheet.Cells[rowReport, 2].Value = attendee.MemberNumber;
                            reportSheet.Cells[rowReport, 3].Value = attendee.FullName;

                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    reportSheet.Cells["A1:K20"].AutoFitColumns();
                    for (int i = 1; i < 11; i++)
                    {
                        for (int j = 1; j < rowReport; j++)
                        {
                            reportSheet.Cells[j, i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            reportSheet.Cells[j, i].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            reportSheet.Cells[j, i].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            reportSheet.Cells[j, i].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    reportSheet.Cells[1, 1, rowReport, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);
                    reportSheet.Cells[rowReport, 5, rowReport, 6].Merge = true;
                    reportSheet.Cells[rowReport, 5, rowReport, 6].Value = "Provided by " + @RDN.Library.Classes.Config.LibraryConfig.WebsiteShortName;


                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();
                string file = "EventRoster_" + RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(calEventTemp.Name) + "_" + calEventTemp.StartDate.ToString("yyyyMMdd") + ".xlsx";
                return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
            }
        }


        [HttpPost]
        public ActionResult EventCalendarExport(CalendarEventPortable cal)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                return new FileStreamResult(CalendarEventFactory.ExportEvent(cal.CalendarItemId, memId), "text/calendar") { FileDownloadName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(cal.Name) + ".ics" };

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult EditEvent(NewCalendarEvent eventUpdated)
        {
            try
            {
                List<long> listOfGroupIds = new List<long>();
                if (!String.IsNullOrEmpty(eventUpdated.ToGroupIds))
                {
                    foreach (string guid in eventUpdated.ToGroupIds.Split(','))
                    {
                        long temp = new long();
                        if (Int64.TryParse(guid, out temp))
                            listOfGroupIds.Add(temp);
                    }
                }

                var calEvent = CalendarEventFactory.UpdateEvent(eventUpdated.CalendarId, eventUpdated.CalendarItemId, Convert.ToDateTime(eventUpdated.StartDateDisplay, new System.Globalization.CultureInfo("en-US")), Convert.ToDateTime(eventUpdated.EndDateDisplay, new System.Globalization.CultureInfo("en-US")), new Guid(eventUpdated.SelectedLocationId), eventUpdated.Name, eventUpdated.Link, eventUpdated.Notes, eventUpdated.AllowSelfCheckIn, Convert.ToInt64(eventUpdated.SelectedEventTypeId), eventUpdated.TicketUrl, eventUpdated.ColorTempSelected, eventUpdated.IsPublicEvent, listOfGroupIds);
                if (MemberCache.GetCalendarDefaultView(RDN.Library.Classes.Account.User.GetUserId()) == CalendarDefaultViewEnum.List_View)
                    return Redirect(Url.Content("~/calendar/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.re));
                else
                    return Redirect(Url.Content("~/calendar/view/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.re));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: eventUpdated.StartDateDisplay + "," + eventUpdated.EndDateDisplay);
            }
            if (MemberCache.GetCalendarDefaultView(RDN.Library.Classes.Account.User.GetUserId()) == CalendarDefaultViewEnum.List_View)
                return Redirect(Url.Content("~/calendar/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
            else
                return Redirect(Url.Content("~/calendar/view/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult EditEvent(string type, string calId, string eventId)
        {
            NewCalendarEvent cal = new NewCalendarEvent();
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var calEvent = CalendarEventFactory.GetEvent(new Guid(eventId), memId, new Guid(calId));
                if (calEvent == null)
                    return Redirect(Url.Content("~/calendar/" + type + "/" + calId));

                cal = new NewCalendarEvent(calEvent);
                cal.CalendarId = new Guid(calId);
                cal.CalendarType = type;
                cal.LeagueId = MemberCache.GetLeagueIdOfMember(memId);
                var colors = ColorDisplayFactory.GetLeagueColors(cal.LeagueId);
                cal.ColorList = new SelectList(colors, "HexColor", "NameOfColor");

                var locs = RDN.Library.Classes.Calendar.CalendarFactory.GetLocationsOfCalendar(new Guid(calId));
                var eventTypes = RDN.Library.Classes.Calendar.CalendarFactory.GetEventTypesOfCalendar(new Guid(calId));
                var AllowSelfCheckin = CalendarFactory.GetCalendar(new Guid(calId), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type));

                if (cal.Location != null)
                    cal.Locations = new SelectList(locs, "LocationId", "LocationName", (object)cal.Location.LocationId);
                else
                    cal.Locations = new SelectList(locs, "LocationId", "LocationName");

                if (cal.EventType != null)
                    cal.EventTypes = new SelectList(eventTypes, "CalendarEventTypeId", "EventTypeName", (object)cal.EventType.CalendarEventTypeId);
                else
                    cal.EventTypes = new SelectList(eventTypes, "CalendarEventTypeId", "EventTypeName");

                cal.AllowSelfCheckIn = AllowSelfCheckin.AllowSelfCheckIn;

                var repeatsFrequency = (from ScheduleWidget.Enums.FrequencyTypeEnum d in Enum.GetValues(typeof(ScheduleWidget.Enums.FrequencyTypeEnum))
                                        where d.ToString() != "None"
                                        where d.ToString() != "EveryWeekDay"
                                        where d.ToString() != "EveryMonWedFri"
                                        where d.ToString() != "EveryTuTh"
                                        select new SelectListItem { Value = ((int)d).ToString(), Text = d.ToString(), Selected = FrequencyTypeEnum.Weekly == d });

                if (String.IsNullOrEmpty(cal.RepeatsFrequencySelectedId))
                    cal.RepeatsFrequencyDropDown = new SelectList(repeatsFrequency, "Value", "Text", ((object)2));
                else
                    cal.RepeatsFrequencyDropDown = new SelectList(repeatsFrequency, "Value", "Text", ((object)cal.RepeatsFrequencySelectedId));

                cal.EndsOccurences = "0";
                List<string> repeatCount = new List<string>();
                for (int i = 1; i < 50; i++)
                {
                    repeatCount.Add(i.ToString());
                }

                var montlhyInterval = (from ScheduleWidget.Enums.MonthlyIntervalEnum d in Enum.GetValues(typeof(ScheduleWidget.Enums.MonthlyIntervalEnum))
                                       where d.ToString() != "None"
                                       select new SelectListItem { Value = ((int)d).ToString(), Text = d.ToString(), Selected = MonthlyIntervalEnum.First == d });
                cal.MonthlyInterval = new SelectList(montlhyInterval, "Value", "Text", ((object)1));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }

        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult EditReoccurringEvent(NewCalendarEvent eventUpdated)
        {
            var memId = RDN.Library.Classes.Account.User.GetUserId();
            try
            {
                List<long> listOfGroupIds = new List<long>();
                if (!String.IsNullOrEmpty(eventUpdated.ToGroupIds))
                {
                    foreach (string guid in eventUpdated.ToGroupIds.Split(','))
                    {
                        long temp = new long();
                        if (Int64.TryParse(guid, out temp))
                            listOfGroupIds.Add(temp);
                    }
                }

                int monthlyIntervalId = 0;
                if (!String.IsNullOrEmpty(eventUpdated.MonthlyIntervalId))
                    monthlyIntervalId = Convert.ToInt32(eventUpdated.MonthlyIntervalId);
                FrequencyTypeEnum frequency = (FrequencyTypeEnum)Enum.Parse(typeof(FrequencyTypeEnum), eventUpdated.RepeatsFrequencySelectedId);
                EndsWhenReoccuringEnum endsWhenn = (EndsWhenReoccuringEnum)Enum.Parse(typeof(EndsWhenReoccuringEnum), eventUpdated.EndsWhen);
                var calEvent = CalendarEventFactory.UpdateEventReOcurring(eventUpdated.CalendarId, eventUpdated.CalendarItemId, Convert.ToDateTime(eventUpdated.StartDateDisplay), Convert.ToDateTime(eventUpdated.EndDateDisplay), new Guid(eventUpdated.SelectedLocationId), eventUpdated.Name, eventUpdated.Link, eventUpdated.Notes, eventUpdated.AllowSelfCheckIn, frequency, eventUpdated.IsSunday, eventUpdated.IsMonday, eventUpdated.IsTuesday, eventUpdated.IsWednesday, eventUpdated.IsThursday, eventUpdated.IsFriday, eventUpdated.IsSaturday, endsWhenn, endsWhenn == EndsWhenReoccuringEnum.On ? Convert.ToDateTime(eventUpdated.EndsDate) : new DateTime(), Convert.ToInt64(eventUpdated.SelectedEventTypeId), monthlyIntervalId, eventUpdated.ColorTempSelected, eventUpdated.IsPublicEvent, listOfGroupIds, memId);
                if (MemberCache.GetCalendarDefaultView(memId) == CalendarDefaultViewEnum.List_View)
                    return Redirect(Url.Content("~/calendar/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.re));
                else
                    return Redirect(Url.Content("~/calendar/view/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.re));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: eventUpdated.StartDateDisplay + ":" + eventUpdated.EndDateDisplay);
            }
            if (MemberCache.GetCalendarDefaultView(memId) == CalendarDefaultViewEnum.List_View)
                return Redirect(Url.Content("~/calendar/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
            else
                return Redirect(Url.Content("~/calendar/view/" + eventUpdated.CalendarType + "/" + eventUpdated.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
        }


        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult EditReoccurringEvent(string type, string calId, string reoccuringEventId)
        {
            NewCalendarEvent cal = new NewCalendarEvent();
            try
            {
                var calEvent = CalendarEventFactory.GetEventReocurring(new Guid(reoccuringEventId), new Guid(calId));
                if (calEvent == null)
                    return Redirect(Url.Content("~/calendar/" + type + "/" + calId));

                cal = new NewCalendarEvent(calEvent);
                cal.CalendarId = new Guid(calId);
                cal.CalendarType = type;
                cal.LeagueId = MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId());
                var colors = ColorDisplayFactory.GetLeagueColors(cal.LeagueId);
                cal.ColorList = new SelectList(colors, "HexColor", "NameOfColor");

                var locs = RDN.Library.Classes.Calendar.CalendarFactory.GetLocationsOfCalendar(new Guid(calId));
                var eventTypes = RDN.Library.Classes.Calendar.CalendarFactory.GetEventTypesOfCalendar(new Guid(calId));
                var AllowSelfCheckin = CalendarFactory.GetCalendar(new Guid(calId), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type));

                if (cal.Location != null)
                    cal.Locations = new SelectList(locs, "LocationId", "LocationName", (object)cal.Location.LocationId);
                else
                    cal.Locations = new SelectList(locs, "LocationId", "LocationName");

                if (cal.EventType != null)
                    cal.EventTypes = new SelectList(eventTypes, "CalendarEventTypeId", "EventTypeName", (object)cal.EventType.CalendarEventTypeId);
                else
                    cal.EventTypes = new SelectList(eventTypes, "CalendarEventTypeId", "EventTypeName");

                cal.AllowSelfCheckIn = AllowSelfCheckin.AllowSelfCheckIn;

                var repeatsFrequency = (from ScheduleWidget.Enums.FrequencyTypeEnum d in Enum.GetValues(typeof(ScheduleWidget.Enums.FrequencyTypeEnum))
                                        where d.ToString() != "None"
                                        select new SelectListItem { Value = ((int)d).ToString(), Text = d.ToString(), Selected = FrequencyTypeEnum.Weekly == d });
                if (String.IsNullOrEmpty(cal.RepeatsFrequencySelectedId))
                    cal.RepeatsFrequencyDropDown = new SelectList(repeatsFrequency, "Value", "Text", ((object)2));
                else
                    cal.RepeatsFrequencyDropDown = new SelectList(repeatsFrequency, "Value", "Text", ((object)cal.RepeatsFrequencySelectedId));

                var montlhyInterval = (from ScheduleWidget.Enums.MonthlyIntervalEnum d in Enum.GetValues(typeof(ScheduleWidget.Enums.MonthlyIntervalEnum))
                                       select new SelectListItem { Value = ((int)d).ToString(), Text = d.ToString(), Selected = MonthlyIntervalEnum.First == d });
                if (String.IsNullOrEmpty(cal.MonthlyIntervalId))
                    cal.MonthlyInterval = new SelectList(montlhyInterval, "Value", "Text", ((object)1));
                else
                    cal.MonthlyInterval = new SelectList(montlhyInterval, "Value", "Text", ((object)cal.MonthlyIntervalId));

                cal.EndsOccurences = "0";
                List<string> repeatCount = new List<string>();
                for (int i = 1; i < 50; i++)
                {
                    repeatCount.Add(i.ToString());
                }

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarEditEventType(string type, string calId, string eventTypeId)
        {
            CalendarEventTypeModel eventType = null;
            try
            {
                eventType = new CalendarEventTypeModel(CalendarFactory.GetEventType(Convert.ToInt64(eventTypeId)));
                eventType.LeagueId = MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId());
                var colors = ColorDisplayFactory.GetLeagueColors(eventType.LeagueId);
                eventType.ColorList = new SelectList(colors, "HexColor", "NameOfColor");

                eventType.CalendarId = new Guid(calId);
                eventType.OwnerEntity = type;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(eventType);
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarEditEventType(CalendarEventType eventType)
        {
            try
            {
                long eventTypeid = CalendarFactory.UpdateCalendarEventType(eventType);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/calendar/settings/" + eventType.OwnerEntity.ToString() + "/" + eventType.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.et));
        }
        /// <summary>
        /// gets the event type values so that users know what points they are using when creating an event.
        /// </summary>
        /// <param name="eventTypeId"></param>
        /// <returns></returns>
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult GetEventTypeValues(string eventTypeId)
        {
            var type = CalendarFactory.GetEventType(Convert.ToInt32(eventTypeId));
            return Json(new { isSuccess = true, type = type }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarNewEventType(string type, string calId)
        {
            CalendarEventTypeModel eventType = new CalendarEventTypeModel();
            try
            {
                eventType.CalendarId = new Guid(calId);
                eventType.OwnerEntity = type;
                eventType.PointsForPresent = 5;
                eventType.PointsForPartial = 4;
                eventType.PointsForNotPresent = 0;
                eventType.PointsForExcused = 3;
                eventType.PointsForTardy = -1;
                eventType.LeagueId = MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId());
                var colors = ColorDisplayFactory.GetLeagueColors(eventType.LeagueId);
                eventType.ColorList = new SelectList(colors, "HexColor", "NameOfColor");
                ViewBag.IsSuccessful = false;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(eventType);
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarNewEventType(CalendarEventTypeModel eventType)
        {
            try
            {
                long eventTypeid = CalendarFactory.AddCalendarEventType(eventType);
                string returnUrl = string.Empty;
                if (Request.UrlReferrer != null)
                {
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.UrlReferrer.Query);
                    returnUrl = nameValueCollection["returnUrl"];
                }
                // if a create another was clicked instead of just submitting the event.
                if (Request.Form["addAnother"] != null)
                {
                    ViewBag.IsSuccessful = true;
                    eventType.EventTypeName = "";
                    return View(eventType);
                }
                if (String.IsNullOrEmpty(returnUrl))
                    return Redirect(Url.Content("~/calendar/settings/" + eventType.OwnerEntity.ToString() + "/" + eventType.CalendarId.ToString().Replace("-", "")));
                else
                    return Redirect(returnUrl);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/calendar/settings/" + eventType.OwnerEntity.ToString() + "/" + eventType.CalendarId.ToString().Replace("-", "")));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CheckInLarge(string type, string calendarId, string eventId)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();
            var calen = CalendarEventFactory.GetEvent(new Guid(eventId), memId, new Guid(calendarId));
            NewCalendarEvent cal = new NewCalendarEvent(calen);
            try
            {
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);
                ViewBag.IsSuccessful = false;
                if (calen == null)
                    return Redirect(Url.Content("~/calendar/" + type + "/" + calendarId));

                cal.CalendarId = new Guid(calendarId);
                Guid entityId = CalendarFactory.GetEntityOwnerId(new Guid(calendarId), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type));

                cal.CalendarType = type;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }



        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CheckInSmall(string type, string calendarId, string eventId)
        {
            var memId = RDN.Library.Classes.Account.User.GetMemberId();

            var calen = CalendarEventFactory.GetEvent(new Guid(eventId), memId, new Guid(calendarId));
            NewCalendarEvent cal = new NewCalendarEvent(calen);
            try
            {
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                ViewBag.IsSuccessful = false;
                if (calen == null)
                    return Redirect(Url.Content("~/calendar/" + type + "/" + calendarId));

                cal.CalendarId = new Guid(calendarId);
                Guid entityId = CalendarFactory.GetEntityOwnerId(new Guid(calendarId), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type));

                cal.CalendarType = type;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }
        /// <summary>
        /// allows the user to check them self into the event by just clickin on the checkin link on calendar list
        /// </summary>
        /// <param name="calendarId"></param>
        /// <param name="eventId"></param>
        /// <param name="note"></param>
        /// <param name="pointType"></param>
        /// <returns></returns>
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public JsonResult CheckSelfIntoEvent(string calendarId, string eventId, string note, string eventTypePoints, string isTardy)
        {
            try
            {
                CalendarEventPointTypeEnum ty = CalendarEventPointTypeEnum.Present;
                if (!String.IsNullOrEmpty(eventTypePoints))
                    ty = (CalendarEventPointTypeEnum)Enum.Parse(typeof(CalendarEventPointTypeEnum), eventTypePoints);
                var isSuccess = CalendarFactory.CheckSelfIn(new Guid(calendarId), new Guid(eventId), RDN.Library.Classes.Account.User.GetMemberId(), note, ty, Convert.ToBoolean(isTardy), 0);
                return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public JsonResult SetAvailabilityForEvent(string calendarId, string eventId, string note, string eventTypePoints)
        {
            try
            {
                AvailibilityEnum ty = AvailibilityEnum.Going;
                if (!String.IsNullOrEmpty(eventTypePoints))
                    ty = (AvailibilityEnum)Enum.Parse(typeof(AvailibilityEnum), eventTypePoints);
                var isSuccess = CalendarFactory.SetAvailabilityForEvent(new Guid(calendarId), new Guid(eventId), RDN.Library.Classes.Account.User.GetMemberId(), note, ty);
                return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public JsonResult CheckMemberIntoEvent(string calendarId, string eventId, string memberId, string pointType, string note, string isTardy, string addPoints)
        {
            try
            {
                CalendarEventPointTypeEnum tempPointType;
                bool success = Enum.TryParse<CalendarEventPointTypeEnum>(pointType, out tempPointType);
                if (!success)
                    tempPointType = CalendarEventPointTypeEnum.Present;
                int addPointsTemp = 0;
                Int32.TryParse(addPoints, out addPointsTemp);
                var isSuccess = CalendarFactory.CheckSelfIn(new Guid(calendarId), new Guid(eventId), new Guid(memberId), note, tempPointType, Convert.ToBoolean(isTardy), addPointsTemp);
                return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: calendarId + ":" + eventId + ":" + memberId + ":" + pointType + ":" + note + ":" + isTardy);
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public JsonResult CheckInMemberRemove(string calendarId, string eventId, string memberId)
        {
            try
            {
                var isSuccess = CalendarFactory.CheckInRemove(new Guid(calendarId), new Guid(eventId), new Guid(memberId));
                return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult ViewEvent(string type, string calId, string eventId)
        {
            NewCalendarEvent cal = new NewCalendarEvent();
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);
                var calEvent = CalendarEventFactory.GetEvent(new Guid(eventId), memId, new Guid(calId));
                if (calEvent == null)
                    if (MemberCache.GetCalendarDefaultView(memId) == CalendarDefaultViewEnum.List_View)
                        return Redirect(Url.Content("~/calendar/" + type + "/" + calId + "?u=" + SiteMessagesEnum.dex));
                    else
                        return Redirect(Url.Content("~/calendar/view/" + type + "/" + calId + "?u=" + SiteMessagesEnum.dex));


                cal = new NewCalendarEvent(calEvent);
                cal.CalendarId = new Guid(calId);
                cal.CalendarType = type;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult ViewBirthday(string id, string name)
        {
            NewCalendarEvent cal = new NewCalendarEvent();
            try
            {
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();

                var calEvent = CalendarEventFactory.GetBirthdayEvent(new Guid(id));
                calEvent.CalendarId = MemberCache.GetCalendarIdForMemberLeague(memberId);
                calEvent.CalendarType = CalendarOwnerEntityEnum.league.ToString();
                cal = new NewCalendarEvent(calEvent);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult ViewStartedPlaying(string id, string name)
        {
            NewCalendarEvent cal = new NewCalendarEvent();
            try
            {
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();
                var calEvent = CalendarEventFactory.GetStartedSkatingEvent(new Guid(id));
                calEvent.CalendarId = MemberCache.GetCalendarIdForMemberLeague(memberId);
                calEvent.CalendarType = CalendarOwnerEntityEnum.league.ToString();
                cal = new NewCalendarEvent(calEvent);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsEventCourdinator = true, HasPaidSubscription = true, IsSecretary = true)]
        public ActionResult DeleteEvent(string type, string calId, string eventId)
        {
            try
            {
                var calEvent = CalendarEventFactory.DeleteEvent(new Guid(calId), new Guid(eventId));
                if (MemberCache.GetCalendarDefaultView(RDN.Library.Classes.Account.User.GetUserId()) == CalendarDefaultViewEnum.List_View)
                    return Redirect(Url.Content("~/calendar/" + type + "/" + calId + "?u=" + SiteMessagesEnum.de));
                else
                    return Redirect(Url.Content("~/calendar/view/" + type + "/" + calId + "?u=" + SiteMessagesEnum.de));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            if (MemberCache.GetCalendarDefaultView(RDN.Library.Classes.Account.User.GetUserId()) == CalendarDefaultViewEnum.List_View)
                return Redirect(Url.Content("~/calendar/" + type + "/" + calId + "?u=" + SiteMessagesEnum.evwd));
            else
                return Redirect(Url.Content("~/calendar/view/" + type + "/" + calId + "?u=" + SiteMessagesEnum.evwd));
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, IsEventCourdinator = true, HasPaidSubscription = true, IsSecretary = true)]
        public ActionResult DeleteEventReoccurring(string type, string calId, string eventId)
        {
            try
            {
                var calEvent = CalendarEventFactory.DeleteEventReccurring(new Guid(calId), new Guid(eventId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            if (MemberCache.GetCalendarDefaultView(RDN.Library.Classes.Account.User.GetUserId()) == CalendarDefaultViewEnum.List_View)
                return Redirect(Url.Content("~/calendar/" + type + "/" + calId));
            else
                return Redirect(Url.Content("~/calendar/view/" + type + "/" + calId));


        }
        /// <summary>
        /// opends a new event window
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult NewEvent(string type, string id, string check)
        {
            Models.Calendar.NewCalendarEvent post = new Models.Calendar.NewCalendarEvent();
            try
            {
                bool didLocationGetAdded = false;
                post.CalendarId = new Guid(id);
                post.CalendarType = type;
                var locs = RDN.Library.Classes.Calendar.CalendarFactory.GetLocationsOfCalendar(new Guid(id));
                var eventTypes = RDN.Library.Classes.Calendar.CalendarFactory.GetEventTypesOfCalendar(new Guid(id));
                var AllowSelfCheckin = CalendarFactory.GetCalendar(new Guid(id), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type));

                if (!String.IsNullOrEmpty(check))
                {
                    //if the check is true, it means this page came from "Create Event and Add Another"
                    if (check == "true")
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Success;
                        message.Message = "Event Was Created.  You can now add another.";
                        this.AddMessage(message);
                    }
                    else // check is actually a location id.
                        didLocationGetAdded = true;
                }

                post.EventTypes = new SelectList(eventTypes, "CalendarEventTypeId", "EventTypeName");

                if (didLocationGetAdded)
                {
                    //an id will exist if they just created a new location.
                    var location = LocationFactory.GetLocation(new Guid(check));
                    locs.Add(location);
                    post.Locations = new SelectList(locs, "LocationId", "LocationName", (object)location.LocationId);
                }
                else
                {
                    post.Locations = new SelectList(locs, "LocationId", "LocationName");
                }
                if (AllowSelfCheckin != null) post.AllowSelfCheckIn = AllowSelfCheckin.AllowSelfCheckIn;
                post.StartDate = DateTime.Now;
                post.EndDate = DateTime.Now;
                var repeatsFrequency = (from ScheduleWidget.Enums.FrequencyTypeEnum d in Enum.GetValues(typeof(ScheduleWidget.Enums.FrequencyTypeEnum))
                                        where d.ToString() != "None"
                                        where d.ToString() != "EveryWeekDay"
                                        where d.ToString() != "EveryMonWedFri"
                                        where d.ToString() != "EveryTuTh"
                                        select new SelectListItem { Value = ((int)d).ToString(), Text = d.ToString(), Selected = FrequencyTypeEnum.Weekly == d });
                post.RepeatsFrequencyDropDown = new SelectList(repeatsFrequency, "Value", "Text", ((object)2));
                var montlhyInterval = (from ScheduleWidget.Enums.MonthlyIntervalEnum d in Enum.GetValues(typeof(ScheduleWidget.Enums.MonthlyIntervalEnum))
                                       where d.ToString() != "None"
                                       select new SelectListItem { Value = ((int)d).ToString(), Text = d.ToString(), Selected = MonthlyIntervalEnum.First == d });
                post.MonthlyInterval = new SelectList(montlhyInterval, "Value", "Text", ((object)1));

                post.EndsOccurences = "0";
                List<string> repeatCount = new List<string>();
                for (int i = 1; i < 50; i++)
                {
                    repeatCount.Add(i.ToString());
                }

                post.LeagueId = MemberCache.GetLeagueIdOfMember(RDN.Library.Classes.Account.User.GetMemberId());
                var colors = ColorDisplayFactory.GetLeagueColors(post.LeagueId);
                post.ColorList = new SelectList(colors, "HexColor", "NameOfColor");
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(post);
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult NewEvent(NewCalendarEvent newEvent)
        {
            // if a create another was clicked instead of just submitting the event.
            bool createAnother = false;
            if (Request.Form["createAnother"] != null)
                createAnother = true;
            bool createAndTrack = false;
            if (Request.Form["createTrack"] != null)
                createAndTrack = true;

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                List<long> listOfGroupIds = new List<long>();
                if (!String.IsNullOrEmpty(newEvent.ToGroupIds))
                {
                    foreach (string guid in newEvent.ToGroupIds.Split(','))
                    {
                        long temp = new long();
                        if (Int64.TryParse(guid, out temp))
                            listOfGroupIds.Add(temp);
                    }
                }

                EndsWhenReoccuringEnum endsWhenn = (EndsWhenReoccuringEnum)Enum.Parse(typeof(EndsWhenReoccuringEnum), newEvent.EndsWhen);
                int monthlyIntervalId = 0;
                if (!String.IsNullOrEmpty(newEvent.MonthlyIntervalId))
                    monthlyIntervalId = Convert.ToInt32(newEvent.MonthlyIntervalId);
                FrequencyTypeEnum frequency = (FrequencyTypeEnum)Enum.Parse(typeof(FrequencyTypeEnum), newEvent.RepeatsFrequencySelectedId);
                //we set if the event is reocurring
                Guid eventIdd = new Guid();
                if (!newEvent.IsReoccurring)
                    eventIdd = CalendarEventFactory.CreateNewEvent(newEvent.CalendarId, newEvent.StartDate, newEvent.EndDate, new Guid(newEvent.SelectedLocationId), newEvent.Name, newEvent.Link, newEvent.Notes, newEvent.AllowSelfCheckIn, newEvent.IsPublicEvent, false, new Guid(), Convert.ToInt64(newEvent.SelectedEventTypeId), newEvent.BroadcastEvent, newEvent.TicketUrl, newEvent.ColorTempSelected, listOfGroupIds, memId);
                else
                    eventIdd = CalendarEventFactory.CreateNewEventReOcurring(newEvent.CalendarId, newEvent.StartDate, newEvent.EndDate, new Guid(newEvent.SelectedLocationId), newEvent.Name, newEvent.Link, newEvent.Notes, newEvent.AllowSelfCheckIn, frequency, newEvent.IsSunday, newEvent.IsMonday, newEvent.IsTuesday, newEvent.IsWednesday, newEvent.IsThursday, newEvent.IsFriday, newEvent.IsSaturday, endsWhenn, Convert.ToInt32(newEvent.EndsOccurences), endsWhenn == EndsWhenReoccuringEnum.On ? Convert.ToDateTime(newEvent.EndsDate) : new DateTime(), Convert.ToInt64(newEvent.SelectedEventTypeId), newEvent.BroadcastEvent, newEvent.IsPublicEvent, monthlyIntervalId, newEvent.TicketUrl, newEvent.ColorTempSelected, listOfGroupIds, memId);

                if (createAnother)
                {
                    return Redirect(Url.Content("~/calendar/new/" + newEvent.CalendarType.ToString().Replace("-", "") + "/" + newEvent.CalendarId.ToString().Replace("-", "") + "/true"));
                }
                else if (createAndTrack)
                {
                    return Redirect(Url.Content("~/calendar/event/checkin-l/" + newEvent.CalendarType.ToString().Replace("-", "") + "/" + newEvent.CalendarId.ToString().Replace("-", "") + "/" + eventIdd.ToString().Replace("-", "")));
                }
                else
                {
                    return Redirect(Url.Content("~/calendar/event/" + newEvent.CalendarType.ToString().Replace("-", "") + "/" + newEvent.CalendarId.ToString().Replace("-", "") + "/" + eventIdd.ToString().Replace("-", "")));

                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: newEvent.SelectedLocationId);
            }

            if (MemberCache.GetCalendarDefaultView(RDN.Library.Classes.Account.User.GetUserId()) == CalendarDefaultViewEnum.List_View)
                return Redirect(Url.Content("~/calendar/" + newEvent.CalendarType.ToString().Replace("-", "") + "/" + newEvent.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
            else
                return Redirect(Url.Content("~/calendar/view/" + newEvent.CalendarType.ToString().Replace("-", "") + "/" + newEvent.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.sww));
        }



        #endregion

        #region Calendar
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarImport(string type, string calId)
        {
            var calen = CalendarFactory.GetCalendar(new Guid(calId), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type));
            try
            {
                CalendarMessages(Request.Url.Query);
                if (calen == null)
                    return Redirect(Url.Content("~/calendar/" + type + "/" + calId));

                CalendarImport import = new CalendarImport();
                import.CalendarId = calen.CalendarId;
                import.GoogleCalendarUrl = calen.ImportFeedUrl;
                import.ImportType = calen.ImportFeedType;
                import.OwnerEntity = calen.OwnerEntity;
                import.TimeZones = calen.TimeZones;
                import.TimeZone = calen.TimeZone;
                import.TimeZoneId = calen.TimeZoneId;


                return View(import);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/calendar/" + type + "/" + calId));
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarImport(CalendarImport calendarImport)
        {

            try
            {
                var calen = CalendarFactory.UpdateCalendarFeedSettings(calendarImport);
                var import = CalendarFactory.ImportFeedFromCalendar(calendarImport.CalendarId);


                return Redirect(Url.Content("~/calendar/import/" + calendarImport.OwnerEntity + "/" + calendarImport.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/calendar/import/" + calendarImport.OwnerEntity + "/" + calendarImport.CalendarId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.f));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarSettings(string type, string calId)
        {
            var calen = CalendarFactory.GetCalendar(new Guid(calId), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type));
            try
            {
                CalendarMessages(Request.Url.Query);

                ViewBag.IsSuccessful = false;
                if (calen == null)
                    return Redirect(Url.Content("~/calendar/" + type + "/" + calId));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(calen);
        }
        [HttpPost]
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsManager = true, HasPaidSubscription = true, IsEventCourdinator = true, IsSecretary = true, IsAttendanceManager = true)]
        public ActionResult CalendarSettings(Calendar calendar)
        {
            try
            {
                CalendarFactory.UpdateCalendarSettings(calendar);
                var calen = CalendarFactory.GetCalendar(calendar.CalendarId, calendar.OwnerEntity);
                ViewBag.IsSuccessful = true;
                if (calen == null)
                    return Redirect(Url.Content("~/calendar/" + calendar.OwnerEntity.ToString() + "/" + calendar.CalendarId.ToString().Replace("-", "")));

                return View(calen);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(calendar);
        }

        [Authorize]
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult CreateCalendar(string OwnerEntity)
        {

            Calendar cal = new Calendar();
            try
            {
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();
                if (OwnerEntity == CalendarOwnerEntityEnum.league.ToString())
                {
                    Guid ownerId = MemberCache.GetLeagueIdOfMember(memberId);
                    cal = CalendarFactory.CreateCalendar(ownerId, CalendarOwnerEntityEnum.league);
                }
                else if (OwnerEntity == CalendarOwnerEntityEnum.federation.ToString())
                {
                    Guid ownerId = MemberCache.GetFederationIdOfMember(memberId);
                    cal = CalendarFactory.CreateCalendar(ownerId, CalendarOwnerEntityEnum.federation);
                }

                MemberCache.Clear(memberId);
                MemberCache.ClearApiCache(memberId);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/calendar/" + OwnerEntity + "/" + cal.CalendarId.ToString().Replace("-", "")));
        }

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult CalendarList(string type, string id, int? year, int? month)
        {
            RDN.Portable.Classes.Controls.Calendar.Calendar cal = new RDN.Portable.Classes.Controls.Calendar.Calendar();
            try
            {
                Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();
                bool isAttendanceManagerOrBetter = MemberCache.IsAttendanceManagerOrBetterOfLeague(memberId);
                var league = MemberCache.GetLeagueOfMember(memberId);
                if (league != null)
                    SetCulture(league.CultureSelected);
                CalendarMessages(Request.Url.Query);

                cal.CalendarId = new Guid();
                cal.OwnerEntity = (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type);
                if (year != null && month != null)
                    cal.CurrentDateSelected = new DateTime(year.Value, month.Value, 15);
                else
                    cal.CurrentDateSelected = DateTime.Now;
                if (new Guid(id) != new Guid())
                {
                    var topics = CalendarFactory.GetCalendarSchedule(new Guid(id), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type), DateTimeExt.FirstDayOfMonthFromDateTime(cal.CurrentDateSelected).AddDays(-2), DateTimeExt.LastDayOfMonthFromDateTime(cal.CurrentDateSelected).AddDays(2), memberId, isAttendanceManagerOrBetter);

                    if (!topics.IsCalendarInUTC)
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "Please set the TimeZone of your calendar in Settings.";
                        this.AddMessage(message);
                    }

                    topics.CurrentDateSelected = cal.CurrentDateSelected;
                    if (!topics.DisableBirthdays)
                        topics.Events.AddRange(MemberCache.GetMemberBirthdays(memberId, DateTimeExt.FirstDayOfMonthFromDateTime(cal.CurrentDateSelected).AddDays(-1), DateTimeExt.LastDayOfMonthFromDateTime(cal.CurrentDateSelected).AddDays(1)));
                    if (!topics.DisableSkatingStartDates)
                        topics.Events.AddRange(MemberCache.GetMemberStartDates(memberId, DateTimeExt.FirstDayOfMonthFromDateTime(cal.CurrentDateSelected).AddDays(-1), DateTimeExt.LastDayOfMonthFromDateTime(cal.CurrentDateSelected).AddDays(1)));
                    return View(topics);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }

        private void CalendarMessages(string query)
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(query);
            string updated = nameValueCollection["u"];

            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.re.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Success;
                message.Message = "Event Updated.";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Success;
                message.Message = "Event Removed.";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.evwd.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Warning;
                message.Message = "Event wasn't deleted, please contact " + LibraryConfig.DefaultInfoEmail + ".";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sww.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Warning;
                message.Message = "Something Went Wrong.  Developers have been notified. Please try again later.";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Success;
                message.Message = "Event Type Updated.";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "Successfully Updated.";
                this.AddMessage(message);
            }
            if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.f.ToString())
            {
                SiteMessage message = new SiteMessage();
                message.MessageType = SiteMessageType.Info;
                message.Message = "Update Failed.";
                this.AddMessage(message);
            }
        }
        /// <summary>
        /// loads the json for the calendar view
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <returns></returns>
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult LoadCalendarView(string type, string id, string startDt, string endDt)
        {
            RDN.Portable.Classes.Controls.Calendar.Calendar cal = new Calendar();
            Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();
            CalendarMessages(Request.Url.Query);
            try
            {
                cal.CalendarId = new Guid();
                cal.OwnerEntity = (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type);
                if (new Guid(id) != new Guid())
                {
                    DateTime st = DateTimeExt.FromUnixTime(Convert.ToInt64(startDt));
                    DateTime en = DateTimeExt.FromUnixTime(Convert.ToInt64(endDt));
                    var topics = CalendarFactory.GetCalendarScheduleForView(new Guid(id), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type), st, en, memberId);
                    if (!topics.DisableBirthdays)
                        topics.EventsJson.AddRange(MemberCache.GetMemberBirthdaysJson(memberId, st, en));
                    if (!topics.DisableSkatingStartDates)
                        topics.EventsJson.AddRange(MemberCache.GetMemberStartDatesJson(memberId, st, en));
                    return Json(new { events = topics.EventsJson }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(new { view = "" }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult CalendarView(string type, string id)
        {
            Calendar cal = new Calendar();
            Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();
            bool isAttendanceManagerOrBetter = MemberCache.IsAttendanceManagerOrBetterOfLeague(memberId);
            CalendarMessages(Request.Url.Query);
            try
            {
                cal.CalendarId = new Guid();
                cal.OwnerEntity = (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type);
                if (new Guid(id) != new Guid())
                {
                    var topics = CalendarFactory.GetCalendarSchedule(new Guid(id), (CalendarOwnerEntityEnum)Enum.Parse(typeof(CalendarOwnerEntityEnum), type), DateTime.Today.AddDays(-2), DateTime.Today.AddDays(31), memberId, isAttendanceManagerOrBetter);
                    if (!topics.IsCalendarInUTC)
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "Please set the TimeZone of your calendar in Settings.";
                        this.AddMessage(message);
                    }
                    topics.Events.AddRange(MemberCache.GetMemberBirthdays(memberId, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(31)));
                    topics.Events.AddRange(MemberCache.GetMemberStartDates(memberId, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(31)));
                    return View(topics);
                }
                else
                {
                    return Redirect(Url.Content("~/calendar/league/" + id));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }



        #endregion

        #region Reporting

        [Authorize]
        [LeagueAuthorize(EmailVerification = true, HasPaidSubscription = true)]
        public ActionResult CalendarReport(string type, string id)
        {
            CalendarReport cal = new CalendarReport();
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var groups = MemberCache.GetLeagueGroupsOfMember(memId);
                cal.GroupsForReport = groups;
                cal.CalendarId = new Guid(id);
                cal.EntityName = type;
                cal.IsSubmitted = false;
                cal.DaysBackwards = 60;
                cal.StartDateSelected = DateTime.UtcNow;
                cal.EndDateSelected = DateTime.UtcNow;
                cal.StartDateSelectedDisplay = DateTime.UtcNow.AddDays(-60).ToShortDateString();
                cal.EndDateSelectedDisplay = DateTime.UtcNow.ToShortDateString();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return View(cal);
        }

        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, HasPaidSubscription = true)]
        public ActionResult CalendarReport(CalendarReport report)
        {
            CalendarReport cal = new CalendarReport();
            try
            {
                long groupId = 0;
                if (!String.IsNullOrEmpty(report.ToGroupIds))
                {
                    foreach (string guid in report.ToGroupIds.Split(','))
                    {
                        if (Int64.TryParse(guid, out groupId))
                            break;
                    }
                }
                if (HttpContext.Request.Form["byDate"] != null || HttpContext.Request.Form["byDateExport"] != null)
                {
                    cal = RDN.Library.Classes.Calendar.Report.CalendarReport.GetReportForCalendar(report.CalendarId, report.EntityName, Convert.ToDateTime(report.StartDateSelectedDisplay), Convert.ToDateTime(report.EndDateSelectedDisplay), groupId, report.PullGroupEventsOnly);
                }
                else if (HttpContext.Request.Form["byDays"] != null || HttpContext.Request.Form["byDaysExport"] != null)
                {
                    DateTime dt = DateTime.UtcNow.AddDays(-report.DaysBackwards);
                    cal = RDN.Library.Classes.Calendar.Report.CalendarReport.GetReportForCalendar(report.CalendarId, report.EntityName, dt, DateTime.UtcNow, groupId, report.PullGroupEventsOnly);
                }

                cal.StartDateSelectedDisplay = cal.StartDateSelected.ToShortDateString();
                cal.EndDateSelectedDisplay = cal.EndDateSelected.ToShortDateString();
                cal.IsSubmitted = true;

                if (HttpContext.Request.Form["byDateExport"] != null || HttpContext.Request.Form["byDaysExport"] != null)
                {
                    return ExportExcelReport(cal);
                }

                return View(cal);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: report.EndDateSelectedDisplay + ":" + report.StartDateSelectedDisplay);
            }
            return View(cal);
        }
        /// <summary>
        /// exports the calendar report to the excel spreadsheet.
        /// </summary>
        /// <param name="cal"></param>
        /// <returns></returns>
        private ActionResult ExportExcelReport(CalendarReport cal)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                try
                {
                    p.Workbook.Properties.Author = LibraryConfig.WebsiteShortName;
                    p.Workbook.Properties.Title = "Calendar Report For " + cal.EntityName;

                    //we create the first sheet.
                    ExcelWorksheet reportSheet = p.Workbook.Worksheets.Add("Report Totals");
                    reportSheet.Name = "Report Totals"; //Setting Sheet's name
                    reportSheet.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                    reportSheet.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
                    reportSheet.Cells[1, 1].Value = "Member";
                    reportSheet.Cells[1, 2].Value = "Total Points For Events Attended";
                    reportSheet.Cells[1, 3].Value = "Points Earned";
                    reportSheet.Cells[1, 4].Value = "Percentage";
                    reportSheet.Cells[1, 5].Value = "Total Hours";
                    int rowReport = 2;
                    //create the remaining sheets with the names.
                    foreach (var attendee in cal.Attendees)
                    {
                        try
                        {
                            string memberName = "NONAME";
                            if (!String.IsNullOrEmpty(attendee.MemberName))
                                memberName = attendee.MemberName;

                            reportSheet.Cells[rowReport, 1].Value = memberName;
                            ExcelWorksheet ws;
                            try
                            {
                                ws = p.Workbook.Worksheets.Add(StringExt.ToExcelFriendly(memberName));
                                ws.Name = StringExt.ToExcelFriendly(memberName); //Setting Sheet's name
                            }
                            catch
                            {
                                try
                                {
                                    ws = p.Workbook.Worksheets.Add(StringExt.ToExcelFriendly(memberName + "-1"));
                                    ws.Name = StringExt.ToExcelFriendly(memberName + "-1"); //Setting Sheet's name
                                }
                                catch
                                {
                                    ws = p.Workbook.Worksheets.Add(StringExt.ToExcelFriendly(memberName + "-2"));
                                    ws.Name = StringExt.ToExcelFriendly(memberName + "-2");
                                }
                            }
                            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                            ws.Cells[1, 1].Value = "Event Name";
                            ws.Cells[1, 2].Value = "Event Type";
                            ws.Cells[1, 3].Value = "Date Start";
                            ws.Cells[1, 4].Value = "Date End";
                            ws.Cells[1, 5].Value = "Points Possible";
                            ws.Cells[1, 6].Value = "Points Earned";
                            ws.Cells[1, 7].Value = "Percentage";
                            ws.Cells[1, 8].Value = "Total Hours";
                            ws.Cells[1, 9].Value = "Notes";

                            // write the details
                            int totalPoints = 0;
                            int pointsEarned = 0;
                            //long totalHours = 0;
                            int index = 2;
                            foreach (var evAttend in attendee.EventsAttended)
                            {
                                ws.Cells[index, 1].Value = evAttend.Name;
                                ws.Cells[index, 2].Value = evAttend.EventType.EventTypeName;
                                ws.Cells[index, 3].Value = evAttend.StartDate.ToShortDateString() + " " + evAttend.StartDate.ToShortTimeString();
                                ws.Cells[index, 4].Value = evAttend.EndDate.ToShortDateString() + " " + evAttend.EndDate.ToShortTimeString();
                                ws.Cells[index, 5].Value = evAttend.EventType.PointsForPresent;

                                totalPoints += evAttend.EventType.PointsForPresent;
                                if (evAttend.Attendees.Where(x => x.MemberId == attendee.MemberId).FirstOrDefault() != null)
                                {
                                    var person = evAttend.Attendees.Where(x => x.MemberId == attendee.MemberId).FirstOrDefault();
                                    ws.Cells[index, 6].Value = person.PointsForEvent;
                                    pointsEarned += person.PointsForEvent;

                                    ws.Cells[index, 8].Value = RDN.Utilities.Dates.DateTimeExt.ToHumanReadableHours(person.TotalHoursAttendedEventType);
                                    ws.Cells[index, 9].Value = person.Note;
                                }
                                else
                                    ws.Cells[index, 8].Value = "0:00";

                                //calculates the  pointsEarned / PointsForPResent
                                //=IF(Score!B10="", "",Score!B10)
                                ws.Cells[index, 7].Formula = "=F" + index + " / E" + index;
                                index += 1;
                            }
                            ws.Cells[index, 1].Value = "";
                            ws.Cells[index, 3].Value = "";
                            ws.Cells[index, 4].Value = "";
                            ws.Cells[index, 5].Value = totalPoints;
                            ws.Cells[index, 6].Value = pointsEarned;
                            //pointsEarned/TotalPoints

                            ws.Cells[index, 7].Formula = "=F" + index + " / E" + index;
                            ws.Cells[index, 8].Value = RDN.Utilities.Dates.DateTimeExt.ToHumanReadableHours(attendee.TotalHoursAttendedEventType);

                            reportSheet.Cells[rowReport, 2].Formula = "='" + ws.Name + "'!" + StringExt.GetExcelColumnName(5) + index;
                            reportSheet.Cells[rowReport, 3].Formula = "='" + ws.Name + "'!" + StringExt.GetExcelColumnName(6) + index;
                            reportSheet.Cells[rowReport, 4].Formula = "=C" + rowReport + " / B" + rowReport;
                            reportSheet.Cells[rowReport, 5].Value = RDN.Utilities.Dates.DateTimeExt.ToHumanReadableHours(attendee.TotalHoursAttendedEventType);

                            ws.Cells["A1:K20"].AutoFitColumns();
                            rowReport += 1;
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    reportSheet.Cells["A1:K20"].AutoFitColumns();
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
                //Generate A File with Random name
                Byte[] bin = p.GetAsByteArray();
                string file = "CalendarReport_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".xlsx";
                return File(bin, RDN.Utilities.IO.FileExt.GetMIMEType(file), file);
            }
        }
        #endregion

    }
}
