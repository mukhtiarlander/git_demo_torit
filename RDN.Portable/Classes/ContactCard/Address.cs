using ProtoBuf;
using RDN.Portable.Classes.ContactCard.Enums;
using RDN.Portable.Classes.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.ContactCard
{
    [ProtoContract]
    [DataContract]
    public class Address
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid AddressId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Address1 { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string Address2 { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string CityRaw { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string StateRaw { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string Zip { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public string Country { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public bool IsDefault { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public AddressTypeEnum Type { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public GeoCoordinate Coords { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public int CountryId { get; set; }
    }
}
