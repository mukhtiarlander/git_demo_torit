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
    public enum CalendarImportTypeEnum
    {
        None = 0,
        Google = 1
    }
}
