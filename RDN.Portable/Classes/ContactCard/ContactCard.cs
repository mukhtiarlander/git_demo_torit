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
    public class ContactCard
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid ContactCardId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public List<Email> Emails { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public List<Im> Ims { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public List<Address> Addresses { get; set; }

        public ContactCard()
        {
            Emails = new List<Email>();
            Ims = new List<Im>();
            Addresses = new List<Address>();
        }
    }
}
