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
    public class MemberDisplayModel: MemberDisplayBasic
    {
        [ProtoMember(101)]
        [DataMember]
        public bool IsChecked { get; set; }
    }
}
