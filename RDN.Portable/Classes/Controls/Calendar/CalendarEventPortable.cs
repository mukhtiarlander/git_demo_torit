using ProtoBuf;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.League.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar
{
    [ProtoContract]
    [DataContract]
    public class CalendarEventPortable
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid CalendarId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string CalendarType { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public Guid CalendarItemId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string OrganizersName { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public Guid OrganizersId { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string OrganizerUrl { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public string Name { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string NameUrl { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public DateTime StartDate { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string StartTime { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string StartDateDisplay { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public DateTime StartDateReoccurring { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public string StartDateReoccurringDisplay { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public DateTime EndDate { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public long TimeSpan { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public bool IsInUTCTime { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public int CalendarTimeZone { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public bool IsCalendarInUTC { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public string EndTime { get; set; }
        [ProtoMember(20)]
        [DataMember]
        public string EndDateDisplay { get; set; }
        [ProtoMember(21)]
        [DataMember]
        public string Address { get; set; }
        [ProtoMember(22)]
        [DataMember]
        public string AddressUrl { get; set; }
        [ProtoMember(23)]
        [DataMember]
        public string ImageUrl { get; set; }
        [ProtoMember(24)]
        [DataMember]
        public string GoogleCalendarUrl { get; set; }
        [ProtoMember(25)]
        [DataMember]
        public string RDNationLink { get; set; }
        [ProtoMember(26)]
        [DataMember]
        public string ToGroupIds { get; set; }
        /// <summary>
        /// this is the end reoccuring date.  If the value is null, there is no end and will continue forever.
        /// </summary>
        [ProtoMember(27)]
        [DataMember]
        public DateTime? EndDateReoccurring { get; set; }
        [ProtoMember(28)]
        [DataMember]
        public string EndDateReoccurringDisplay { get; set; }
        //[ProtoMember(29)]
        //[DataMember]
        //public ScheduleWidget.ScheduledEvents.Event EventReoccurring { get; set; }
        [ProtoMember(30)]
        [DataMember]
        public string Link { get; set; }
        [ProtoMember(31)]
        [DataMember]
        public bool AllowSelfCheckIn { get; set; }
        [ProtoMember(32)]
        [DataMember]
        public AvailibilityEnum Availibility { get; set; }
        [ProtoMember(33)]
        [DataMember]
        public string AvailableNotes { get; set; }
        [ProtoMember(34)]
        [DataMember]
        public bool BroadcastEvent { get; set; }
        [ProtoMember(35)]
        [DataMember]
        public bool IsRemovedFromCalendar { get; set; }
        [ProtoMember(36)]
        [DataMember]
        public bool IsPublicEvent { get; set; }
        [ProtoMember(37)]
        [DataMember]
        public string Notes { get; set; }
        [ProtoMember(38)]
        [DataMember]
        public string NotesHtml { get; set; }
        [ProtoMember(39)]
        [DataMember]
        public Guid CalendarReoccurringId { get; set; }
        [ProtoMember(40)]
        [DataMember]
        public bool IsReoccurring { get; set; }
        [ProtoMember(41)]
        [DataMember]
        public CalendarEventType EventType { get; set; }

        /// <summary>
        /// is the current logged in member apart of the event that was assigned to them when adding groups to event.
        /// </summary>
        [ProtoMember(42)]
        [DataMember]
        public bool IsCurrentMemberApartOfEvent { get; set; }

        [ProtoMember(43)]
        [DataMember]
        public string TicketUrl { get; set; }
        [ProtoMember(44)]
        [DataMember]
        public Guid NextEventId { get; set; }
        [ProtoMember(45)]
        [DataMember]
        public Guid PreviousEventId { get; set; }
        [ProtoMember(46)]
        [DataMember]
        public string ColorTempSelected { get; set; }

        [ProtoMember(47)]
        [DataMember]
        public virtual Location.Location Location { get; set; }

        //public virtual List<CalendarEventPoint> PointsForEvent { get; set; }
        [ProtoMember(48)]
        [DataMember]
        public virtual List<CalendarAttendance> Attendees { get; set; }
        [ProtoMember(49)]
        [DataMember]
        public virtual List<CalendarAttendance> MembersApartOfEvent { get; set; }
        [ProtoMember(50)]
        [DataMember]
        public List<CalendarAttendance> MembersToCheckIn { get; set; }
        [ProtoMember(51)]
        [DataMember]
        public List<LeagueGroup> GroupsForEvent { get; set; }

        [ProtoMember(52)]
        [DataMember]
        public Guid MemberId{ get; set; }

        [ProtoMember(53)]
        [DataMember]
        public Guid UserId{ get; set; }

        [ProtoMember(54)]
        [DataMember]
        public bool IsSunday { get; set; }
        [ProtoMember(55)]
        [DataMember]
        public bool IsMonday { get; set; }
        [ProtoMember(56)]
        [DataMember]
        public bool IsTuesday { get; set; }
        [ProtoMember(57)]
        [DataMember]
        public bool IsWednesday { get; set; }
        [ProtoMember(58)]
        [DataMember]
        public bool IsThursday { get; set; }
        [ProtoMember(59)]
        [DataMember]
        public bool IsFriday { get; set; }
        [ProtoMember(60)]
        [DataMember]
        public bool IsSaturday { get; set; }
        [ProtoMember(61)]
        [DataMember]
        public EndsWhenReoccuringEnum EndsWhenReoccuringEnum { get; set; }
        [ProtoMember(62)]
        [DataMember]
        public FrequencyTypeCalendarEnum Frequency { get; set; }

        [ProtoMember(63)]
        [DataMember]
        public int OccurrencesTillEnd { get; set; }
        [ProtoMember(64)]
        [DataMember]
        public bool IsSuccessful{ get; set; }
        [ProtoMember(65)]
        [DataMember]
        public bool CanCurrentUserCheckIn{ get; set; }
        [ProtoMember(66)]
        [DataMember]
        public bool IsAttendanceManagerOrBetter{ get; set; }

        [ProtoMember(67)]
        [DataMember]
        public bool IsCurrentMemberCheckedIn { get; set; }
        [ProtoMember(68)]
        [DataMember]
        public bool HasCurrentMemberSetAvailability { get; set; }

        public CalendarEventPortable()
        {
            //PointsForEvent = new List<CalendarEventPoint>();
            Attendees = new List<CalendarAttendance>();
            EventType = new CalendarEventType();
            Location = new Location.Location();
            MembersApartOfEvent = new List<CalendarAttendance>();
            MembersToCheckIn = new List<CalendarAttendance>();
            GroupsForEvent = new List<LeagueGroup>();
        }

    }
}
