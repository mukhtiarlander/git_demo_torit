using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar
{
    [ProtoContract]
    [DataContract]
    public class CalendarAttendance : MemberDisplayBasic
    {
        [ProtoMember(1001)]
        [DataMember]
        public long AttedanceId { get; set; }
      
        [ProtoMember(1006)]
        [DataMember]
        public string Note { get; set; }
        [ProtoMember(1007)]
        [DataMember]
        public CalendarEventPointTypeEnum PointType { get; set; }
        [ProtoMember(1008)]
        [DataMember]
        public CalendarEventPointTypeEnum SecondaryPointType { get; set; }
        [ProtoMember(1009)]
        [DataMember]
        public bool IsCheckedIn { get; set; }
        [ProtoMember(1010)]
        [DataMember]
        public int TotalPoints { get; set; }
        [ProtoMember(1011)]
        [DataMember]
        public long TotalHoursAttendedEventType { get; set; }
        [ProtoMember(1012)]
        [DataMember]
        public double PointsAverage { get; set; }
        [ProtoMember(1013)]
        [DataMember]
        public int PointsForEvent { get; set; }
        [ProtoMember(1014)]
        [DataMember]
        public Enums.AvailibilityEnum Availability { get; set; }
        [ProtoMember(1015)]
        [DataMember]
        public string AvailableNotes { get; set; }
        [ProtoMember(1016)]
        [DataMember]
        public int AdditionalPoints { get; set; }
        /// <summary>
        /// readable points.
        /// </summary>
        [ProtoMember(1017)]
        [DataMember]
        public string PointsStringForReading { get; set; }
        public CalendarAttendance()
        { }
        public CalendarAttendance(long attendanceId, Guid memberId, string firstName, string lastName, string memberName, string note, Enums.CalendarEventPointTypeEnum pointType, Enums.CalendarEventPointTypeEnum secondaryPoints, string memberNumber, bool isCheckedIn, Enums.AvailibilityEnum availability, string availableNote)
        {
            this.AttedanceId = attendanceId;
            this.MemberId = memberId;
            this.DerbyName = memberName;
            this.Firstname= firstName;
            this.LastName = lastName;
            this.Note = note;
            this.PointType = pointType;
            this.SecondaryPointType = secondaryPoints;
            this.PlayerNumber= memberNumber;
            this.IsCheckedIn = isCheckedIn;
            this.Availability = availability;
            this.AvailableNotes = availableNote;
        }
    }
}
