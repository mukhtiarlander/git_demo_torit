using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar.Enums
{
    [ProtoContract]
    [DataContract]
    public enum CalendarOwnerEntityEnum
    {
        none = 0,
        federation = 1,
        league = 2,
        member = 3
    }
}
