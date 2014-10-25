using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Util
{
    [ProtoContract]
    [DataContract]
    public class Culture
    {
        [ProtoMember(1)]
        [DataMember]
        public string Name { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public int LCID { get; set; }
    }
}
