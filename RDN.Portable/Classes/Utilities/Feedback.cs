using ProtoBuf;
using RDN.Portable.Classes.Utilities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Utilities
{
    [DataContract]
    [ProtoContract]
    public class Feedback
    {
        [ProtoMember(1)]
        [DataMember]
        public DateTime DateTime { get; set; }
        [DataMember]
        [ProtoMember(2)]
        public string League { get; set; }
        [DataMember]
        [ProtoMember(3)]
        public string Email { get; set; }
        [DataMember]
        [ProtoMember(4)]
        public string FeedbackText { get; set; }
        [DataMember]
        [ProtoMember(5)]
        public FeedbackTypeEnum FeedbackType { get; set; }

        [DataMember]
        [ProtoMember(6)]
        public bool IsSuccessful { get; set; }



    }
}
