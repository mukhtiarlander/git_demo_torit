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
    public class MemberDisplayMessage : MemberDisplayBasic
    {
        [ProtoMember(901)]
        [DataMember]
        public bool HasNotReadConversation { get; set; }


        public MemberDisplayMessage()
        { }
    }
}
