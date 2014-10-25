using ProtoBuf;
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
    public class CalendarAttendance
    {
        [ProtoMember(1)]
        [DataMember]
        public long AttedanceId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public Guid MemberId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string MemberName { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string FullName { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string MemberNumber { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string Note { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public CalendarEventPointTypeEnum PointType { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public CalendarEventPointTypeEnum SecondaryPointType { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public bool IsCheckedIn { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public int TotalPoints { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public long TotalHoursAttendedEventType { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public double PointsAverage { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public int PointsForEvent { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public Enums.AvailibilityEnum Availability { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public string AvailableNotes { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public int AdditionalPoints { get; set; }
        /// <summary>
        /// readable points.
        /// </summary>
        [ProtoMember(17)]
        [DataMember]
        public string PointsStringForReading { get; set; }
        public CalendarAttendance()
        { }
        public CalendarAttendance(long attendanceId, Guid memberId, string fullName, string memberName, string note, Enums.CalendarEventPointTypeEnum pointType, Enums.CalendarEventPointTypeEnum secondaryPoints, string memberNumber, bool isCheckedIn, Enums.AvailibilityEnum availability, string availableNote)
        {
            this.AttedanceId = attendanceId;
            this.MemberId = memberId;
            this.MemberName = memberName;
            this.FullName = fullName;
            this.Note = note;
            this.PointType = pointType;
            this.SecondaryPointType = secondaryPoints;
            this.MemberNumber = memberNumber;
            this.IsCheckedIn = isCheckedIn;
            this.Availability = availability;
            this.AvailableNotes = availableNote;
        }
    }
}
