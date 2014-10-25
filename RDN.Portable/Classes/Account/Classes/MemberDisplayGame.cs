using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    [ProtoContract]
    [DataContract]
    public class MemberDisplayGame : MemberDisplayBasic
    {
        [ProtoMember(1)]
        [DataMember]
        public string DerbyLinkName { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public Guid MemberLinkId { get; set; }
    }
}
