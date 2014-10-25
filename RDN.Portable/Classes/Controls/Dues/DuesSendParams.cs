using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Dues
{
    [ProtoContract]
    [DataContract]
  public   class DuesSendParams
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid DuesId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public long DuesItemId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public Guid MemberId { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public Guid UserId { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public long DuesCollectedId { get; set; }

        [ProtoMember(6)]
        [DataMember]
        public bool IsSuccessful { get; set; }


        [ProtoMember(7)]
        [DataMember]
        public double Amount{ get; set; }

        [ProtoMember(8)]
        [DataMember]
        public string  Note{ get; set; }
        [ProtoMember(9)]
        [DataMember]
        public Guid CurrentMemberId { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public DateTime PayBy { get; set; }
    }
}
