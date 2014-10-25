using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Forum
{
    [DataContract]
    [ProtoContract]
    public class ForumPostModel
    {
        [ProtoMember(1)]
        [DataMember]
        public string Text { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public DateTime DatePosted { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string PostedByName { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public Guid PostedById { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string PostedByPictureUrl { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string DatePostedByHuman { get; set; }
    }
}
