using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Games.Scoreboard.Game
{
    [ProtoContract]
    [DataContract]
    public class GameScore
    {
        [ProtoMember(1)]
        [DataMember]
        public long ScoreId { get; set; }
        //id of the score from the game it self generated on the client machine.
        [ProtoMember(2)]
        [DataMember]
        public Guid GameScoreId { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public DateTime DateTimeScored { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public int Point { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public int JamNumber { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public Guid JamId { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public int PeriodNumber { get; set; }
        [ProtoMember(8)]
        [DataMember]
        public long PeriodTimeRemainingMilliseconds { get; set; }


    }
}
