using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Message
{
    [ProtoContract]
    [DataContract]
    public class MessageSingleModel
    {
        [ProtoMember(1)]
        [DataMember]
        public long MessageId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string Title { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string MessageText { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string MessageTextHtml { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public DateTime MessageCreated { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public Guid FromId { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public string FromName { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public string ThumbUrl { get; set; }

    }
}
