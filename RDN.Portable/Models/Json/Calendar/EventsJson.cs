using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json.Calendar
{
    [DataContract]
    public class EventsJson : DataJson
    {
        [DataMember]
        public List<EventJson> Events { get; set; }
        public EventsJson()
        {
            Events = new List<EventJson>();
        }
    }
}
