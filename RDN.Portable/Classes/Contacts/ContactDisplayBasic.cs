using ProtoBuf;
using RDN.Portable.Classes.Contacts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Contacts
{
    /// <summary>
    /// basic member, so we can pass JSON back and forth with the most important information.
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class ContactDisplayBasic
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid ContactId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string FirstName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string LastName { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string Email { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string PhoneNumber { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string ContactTypeSelected { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public ContactTypeEnum ContactType { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public ContactTypeForOrganizationEnum ContactTypeForOrg { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public ContactCard.ContactCard ContactCard { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string CompanyName { get; set; }

        [ProtoMember(11)]
        [DataMember]
        public string Link { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string Notes { get; set; }

        [ProtoMember(13)]
        [DataMember]
        public Guid AddressId { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public string Address1 { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public string Address2 { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public string CityRaw { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public string StateRaw { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public string Zip { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public int CountryId { get; set; }
        [ProtoMember(20)]
        [DataMember]
        public string CountryName { get; set; }
        [ProtoMember(21)]
        [DataMember]
        public bool IsDefault { get; set; }


        public ContactDisplayBasic()
        {
            ContactCard = new ContactCard.ContactCard();
        }
    }
}
