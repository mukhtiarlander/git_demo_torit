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
  public   class ForumTopicInbox
    {
        [ProtoMember(1)]
      [DataMember]
      public Guid MemberId { get; set; }
        [ProtoMember(2)]
        [DataMember]
      public string DerbyName { get; set; }
        [ProtoMember(3)]
        [DataMember]
      public long TopicInboxId { get; set; }
      public ForumTopicInbox()
      { }


    }
}
