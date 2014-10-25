using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Forum
{
    [ProtoContract]
    [DataContract]
    public class AddForumTopicModel
    {
        [ProtoMember(1)]
        [DataMember]
        public string TopicTitle { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Text { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public DateTime DatePosted { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string UserId { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string MemberId { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public long GroupId { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public long CategoryId { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public Guid ForumId { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public bool IsSuccessful { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public string ForumType { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public bool BroadcastMessage { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public bool PinMessage { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public bool LockMessage { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public long TopicId { get; set; }
    }
}
