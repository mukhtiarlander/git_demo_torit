using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Controls.Message.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Message
{
    [ProtoContract]
    [DataContract]
    public class MessageModel
    {
        [ProtoMember(1)]
        [DataMember]
        public string NameOfEntity { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public Guid IdOfEntity { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public GroupOwnerTypeEnum OwnerType { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public List<ConversationModel> Conversations { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public bool IsSuccessful { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public List<MemberDisplayBasic> Recipients { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public bool SendEmailForMessage { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public bool IsCarrierVerified { get; set; }
        public MessageModel()
        {
            Conversations = new List<ConversationModel>();
            Recipients = new List<MemberDisplayBasic>();
        }

    }
}
