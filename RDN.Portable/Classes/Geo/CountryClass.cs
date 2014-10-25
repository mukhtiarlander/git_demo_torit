using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Geo
{
    [ProtoContract]
    [DataContract]
    public class CountryClass
    {
        [ProtoMember(1)]
        [DataMember]
        public int CountryId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Name { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string Code { get; set; }
    }
}
