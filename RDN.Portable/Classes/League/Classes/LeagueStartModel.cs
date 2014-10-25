using ProtoBuf;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Classes.Voting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.League.Classes
{
    [DataContract]
    [ProtoContract]
 public    class LeagueStartModel
    {
        [DataMember]
    [ProtoMember(1)]
        public Calendar Calendar { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public List<ForumTopicModel> ForumTopics { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public PollBase Polls { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public Guid CurrentLeagueId{ get; set; }

        public LeagueStartModel()
        {
            ForumTopics = new List<ForumTopicModel>();
        }
        
    }
}
