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
    public class ForumTopicModel
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid ForumId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public long TopicId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string TopicName { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public int ViewCount { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public int PostCount { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string StartedByName { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public Guid StartedById { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string StartedRelativeTime { get; set; }
        [ProtoMember(9)]
        [DataMember]
        public string LastPostByName { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public Guid LastPostById { get; set; }
        [ProtoMember(11)]
        [DataMember]
        public string LastPostRelativeTime { get; set; }
        [ProtoMember(12)]
        [DataMember]
        public string Category { get; set; }
        [ProtoMember(13)]
        [DataMember]
        public long CategoryId { get; set; }
        [ProtoMember(14)]
        [DataMember]
        public List<ForumPostModel> Posts { get; set; }
        [ProtoMember(15)]
        [DataMember]
        public bool HasRead{ get; set; }
        [ProtoMember(16)]
        [DataMember]
        public bool IsManagerOfTopic{ get; set; }
        [ProtoMember(17)]
        [DataMember]
        public bool IsPinned{ get; set; }
        [ProtoMember(18)]
        [DataMember]
        public bool IsLocked{ get; set; }
        [ProtoMember(19)]
        [DataMember]
        public bool IsWatching{ get; set; }
        [ProtoMember(20)]
        [DataMember]
        public List<ForumTopicInbox> TopicInbox { get; set; }
        
        

        public ForumTopicModel()
        {
            Posts = new List<ForumPostModel>();
        }

    }
}
