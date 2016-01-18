using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace RDN.Portable.Classes.Sponsorship
{
    [ProtoContract]
    [DataContract]
    public class SponsorshipDisplay
    {
        [ProtoMember(1)]
        [DataMember]
        public long SponsorshipId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string SponsorshipName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public double Price { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public bool IsRemoved { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string Description { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public DateTime? ExpiresDate { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid OwnerId { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public RDN.Portable.Classes.League.Classes.League League { get; set; }
    }
}
