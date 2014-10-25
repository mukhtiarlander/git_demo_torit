using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar
{
    [ProtoContract]
    [DataContract]
    public class CalendarOwner
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid OwnerId { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public string OwnerName { get; set; }
    }
}
