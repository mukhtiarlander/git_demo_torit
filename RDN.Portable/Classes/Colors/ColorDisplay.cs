using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Colors
{
    [ProtoContract]
    [DataContract]
    public class ColorDisplay
    {
        [ProtoMember(1)]
        [DataMember]
        public long ColorId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string NameOfColor { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public int CSharpColor { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string HexColor { get; set; }

    }
}
