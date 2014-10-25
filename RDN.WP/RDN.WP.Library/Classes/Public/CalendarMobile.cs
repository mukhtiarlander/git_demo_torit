using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RDN.Portable.Config;
using RDN.Portable.Models.Json.Calendar;
using RDN.Portable.Classes.Utilities;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Portable.Network;
using System.IO;
using RDN.Portable.Classes.Controls.Calendar.Enums;

namespace RDN.WP.Library.Classes.Public
{
    public class CalendarMobile
    {

        public static CalendarEventPortable CreateNewEvent(CalendarEventPortable ev)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(ev), MobileConfig.LEAGUE_CALENDAR_CREATE_NEW_EVENT_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<CalendarEventPortable>(json);
        }
        public static List<CalendarEventType> GetCalendarEventTypes(Guid memId, Guid uid, Guid calendarId)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new CalendarSendParams() { CurrentMemberId = memId, UserId = uid, CalendarId = calendarId }), MobileConfig.LEAGUE_CALENDAR_GET_EVENT_TYPES_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<List<CalendarEventType>>(json);
        }
        public static CalendarSendParams CheckMemberIntoEvent(Guid memId, Guid uid, Guid calendarId, Guid eventId, string notes, bool isTardy, CalendarEventPointTypeEnum pointType, Guid memberId, int addpoints)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new CalendarSendParams() { CurrentMemberId = memId, UserId = uid, EventId = eventId, Note = notes, IsTardy = isTardy, PointType = pointType, CalendarId = calendarId, MemberId = memberId, AddPoints = addpoints }), MobileConfig.LEAGUE_CALENDAR_CHECK_MEMBER_IN_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<CalendarSendParams>(json);
        }
        public static CalendarSendParams RemoveCheckInEvent(Guid memId, Guid uid, Guid calendarId, Guid eventId, Guid memberId)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new CalendarSendParams() { CurrentMemberId = memId, UserId = uid, EventId = eventId, CalendarId = calendarId, MemberId = memberId }), MobileConfig.LEAGUE_CALENDAR_CHECK_MEMBER_IN_REMOVE_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<CalendarSendParams>(json);
        }
        public static CalendarSendParams CheckSelfIntoEvent(Guid memId, Guid uid, Guid calendarId, Guid eventId, string notes, bool isTardy, CalendarEventPointTypeEnum pointType)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new CalendarSendParams() { CurrentMemberId = memId, UserId = uid, EventId = eventId, Note = notes, IsTardy = isTardy, PointType = pointType, CalendarId = calendarId }), MobileConfig.LEAGUE_CALENDAR_CHECK_SELF_IN_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<CalendarSendParams>(json);
        }
        public static CalendarSendParams SetAvailabilityForEvent(Guid memId, Guid uid, Guid calendarId, Guid eventId, string notes, AvailibilityEnum availType)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new CalendarSendParams() { CurrentMemberId = memId, UserId = uid, EventId = eventId, Note = notes, Availability = availType, CalendarId = calendarId }), MobileConfig.LEAGUE_CALENDAR_SET_AVAILABILITY_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<CalendarSendParams>(json);
        }
        public static CalendarEventPortable GetCalendarEvent(Guid memId, Guid uid, Guid calendarId, Guid eventId)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new CalendarSendParams() { CurrentMemberId = memId, UserId = uid, EventId = eventId, CalendarId = calendarId }), MobileConfig.LEAGUE_CALENDAR_VIEW_EVENT_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<CalendarEventPortable>(json);
        }
        public static Calendar GetCalendarEvents(Guid memId, Guid uid, Guid calendarId, CalendarOwnerEntityEnum ownerType, int year, int month)
        {
            Random r = new Random();
            var response = Network.SendPackage(Network.ConvertObjectToStream(new CalendarSendParams() { CurrentMemberId = memId, UserId = uid, Year = year, Month = month, CalendarId = calendarId, CalendarType = ownerType }), MobileConfig.LEAGUE_CALENDAR_LIST_URL + "?r=" + r.Next());
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<Calendar>(json);
        }
        public static void PullCurrentEventsByLocation(int page, int count, double longitude, double latitude, Action<EventsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<EventsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.SEARCH_CALENDAR_EVENTS_BY_LL_URL + "p=" + page + "&c=" + count + "&lat=" + latitude + "&lon=" + longitude));
        }
        public static void PullCurrentEvents(int page, int count, Action<EventsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<EventsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.GET_CALENDAR_EVENTS_URL + "p=" + page + "&c=" + count));
        }
        public static void SearchCurrentEvents(int page, int count, string s, Action<EventsJson> callback)
        {
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (sender, e) =>
            {
                try
                {
                    var data = Json.DeserializeObject<EventsJson>(e.Result);
                    callback(data);
                }
                catch (Exception exception)
                {

                }
            };
            webClient.Encoding = System.Text.Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(MobileConfig.SEARCH_CALENDAR_EVENTS_URL + "p=" + page + "&c=" + count + "&s=" + s));
        }
    }
}
