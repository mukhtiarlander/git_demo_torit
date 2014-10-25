using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Account.Classes
{
    public enum SkaterPositionDisplayEnum { Blocker, Jammer, Pivot }
    /// <summary>
    /// defines a skater position used for defining the displaying of members and their positions.
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class SkaterPositionDisplay
    {
        public SkaterPositionDisplay(SkaterPositionDisplayEnum skaterType)
        {
            this.SkaterPosition = skaterType;
        }
        [ProtoMember(1)]
        [DataMember]
        public SkaterPositionDisplayEnum SkaterPosition { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public int TotalGamesPlayed { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public int TotalJamsPlayed { get; set; }

        /// <summary>
        /// total points member scored for their career
        /// </summary>
        [ProtoMember(4)]
        [DataMember]
        public int PointsCareer { get; set; }
        /// <summary>
        /// total points scored for the season
        /// </summary>
        [ProtoMember(5)]
        [DataMember]
        public int PointsSeason { get; set; }
        /// <summary>
        /// this represents total number of points scored against jammer.
        /// So we can see real deltas..
        /// </summary>
        [ProtoMember(6)]
        [DataMember]
        public int PointsAgainstCareer { get; set; }
        /// <summary>
        /// points scored as jammer and difference of teams scoring on jammer
        /// </summary>
        [ProtoMember(7)]
        [DataMember]
        public double PointsCareerDelta { get; set; }
        /// <summary>
        /// this represents total number of points scored against jammer.
        /// So we can see real deltas..
        /// </summary>
        [ProtoMember(8)]
        [DataMember]
        public int PointsAgainstSeason { get; set; }

        [ProtoMember(9)]
        [DataMember]
        public double AveragePointsScoredPerGame { get; set; }
        [ProtoMember(10)]
        [DataMember]
        public double AveragePointsScoredPerJam { get; set; }
        /// <summary>
        /// points avertage scored against jammer during the jam
        /// </summary>
        [ProtoMember(11)]
        [DataMember]
        public double AveragePointsScoredAgainstPerGame { get; set; }
        /// <summary>
        /// points avertage scored against jammer during the jam
        /// </summary>
        [ProtoMember(12)]
        [DataMember]
        public double AveragePointsScoredAgainstPerJam { get; set; }
        /// <summary>
        /// Difference of points scored vs points scored on other team
        /// </summary>
        [ProtoMember(13)]
        [DataMember]
        public double AveragePointsScoredPerJamDelta { get; set; }
        /// <summary>
        /// Difference of points scored vs points scored on other team
        /// </summary>
        [ProtoMember(14)]
        [DataMember]
        public double AveragePointsScoredPerGameDelta { get; set; }
    }
}
