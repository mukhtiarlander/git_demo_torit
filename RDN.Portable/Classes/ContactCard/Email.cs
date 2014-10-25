using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.ContactCard
{
    [ProtoContract]
    [DataContract]
    public class Email
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid EmailId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string EmailAddress { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public bool IsDefault { get; set; }
    }
}
