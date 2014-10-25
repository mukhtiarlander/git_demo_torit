using ProtoBuf;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Portable.Classes.Controls.Calendar.Enums;
using RDN.Portable.Classes.League.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Library.Classes.Controls.Calendar
{
    [ProtoContract]
    [DataContract]
    public class CalendarEvent : CalendarEventPortable
    {
        [ProtoMember(29)]
        [DataMember]
        public ScheduleWidget.ScheduledEvents.Event EventReoccurring { get; set; }


        public CalendarEvent()
        {
        }

    }
}
