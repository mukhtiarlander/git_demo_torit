using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Location
{
    [ProtoContract]
    [DataContract]
    public class Location
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid LocationId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string LocationName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public ContactCard.ContactCard Contact { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Website { get; set; }
        public Location()
        {
            Contact = new ContactCard.ContactCard();
        }
    }
}
