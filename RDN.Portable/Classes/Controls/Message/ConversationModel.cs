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
    public class ConversationModel
    {
        [ProtoMember(1)]
        [DataMember]
        public bool IsSuccessful { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public List<MessageSingleModel> Messages { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public long GroupId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public Guid UserId { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public Guid MemberId { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string Message { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid ConversationWithUser { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string FromName { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public Guid FromId { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public bool CanDelete { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string ToName { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public Guid OwnerUserId { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public bool IsConversationRead { get; set; }

        [ProtoMember(15)]
        [DataMember]
        public List<MemberDisplayMessage> Recipients { get; set; }
        [ProtoMember(16)]
        [DataMember]
        public DateTime LastPostDate { get; set; }
        [ProtoMember(17)]
        [DataMember]
        public string LastPostBy { get; set; }
        [ProtoMember(18)]
        [DataMember]
        public long GroupMessageId { get; set; }
        [ProtoMember(19)]
        [DataMember]
        public string Title { get; set; }
        [ProtoMember(20)]
        [DataMember]
        public bool SendEmailForMessage { get; set; }

        [ProtoMember(21)]
        [DataMember]
        public GroupOwnerTypeEnum OwnerType { get; set; }
        [ProtoMember(22)]
        [DataMember]
        public List<long> GroupIds { get; set; }

        [ProtoMember(23)]
        [DataMember]
        public MessageTypeEnum MessageTypeEnum { get; set; }



        public string LastPostNames
        {
            get
            {
                var rec=Recipients.Select(x => x.DerbyName).Take(3).ToList();
                string r = string.Empty;
                for(int i=0;i< rec.Count;i++)
                {
                    r += rec[i];
                    if (i + 1 < rec.Count)
                        r += ", ";
                }
                return r;
            }
        }
        public string LastPostThumbUrl
        {
            get
            {
                return Recipients.Select(x => x.ThumbUrl).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            }
        }
        public string LastPostDateTime
        {
            get
            {
                return Messages.OrderByDescending(x=>x.MessageCreated).Select(x => x.MessageCreated).FirstOrDefault().ToString("MMM dd");
            }
        }
        public string LastMessageText
        {
            get
            {
                return Messages.OrderByDescending(x=>x.MessageCreated).Select(x => x.MessageText).FirstOrDefault();
            }
        }

        public ConversationModel()
        {
            Messages = new List<MessageSingleModel>();
            Recipients = new List<MemberDisplayMessage>();
            GroupIds = new List<long>();
        }
    }
}
