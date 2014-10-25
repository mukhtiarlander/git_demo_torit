using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using RDN.Library.DataModels.Game.Members;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game
{
    [Table("RDN_Game_Jams")]
    public class GameJam : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GameJamId { get; set; }
        public Guid JamId { get; set; }
        public long GameTimeElapsedMillisecondsStart { get; set; }
        public long GameTimeElapsedMillisecondsEnd { get; set; }

        public int CurrentPeriod { get; set; }
        public int JamNumber { get; set; }

        public byte TeamLeadingJam { get; set; }

        public int TotalPointsForJamT1 { get; set; }
        public int TotalPointsForJamT2 { get; set; }
        public bool DidJamEndWithInjury { get; set; }
        public bool DidJamGetCalledByJammerT1 { get; set; }
        public bool DidJamGetCalledByJammerT2 { get; set; }

        public bool DidTeam1JammerLoseLeadEligibility { get; set; }
        public bool DidTeam2JammerLoseLeadEligibility { get; set; }

        #region References

        public virtual ICollection<GameLeadJammer> LeadJammers { get; set; }
        public virtual ICollection<GameJamPasses> JamPasses { get; set; }
        public virtual GameMember JammerTeam1 { get; set; }
        public virtual GameMember PivotTeam1 { get; set; }
        public virtual GameMember Blocker1Team1 { get; set; }
        public virtual GameMember Blocker2Team1 { get; set; }
        public virtual GameMember Blocker3Team1 { get; set; }
        public virtual GameMember Blocker4Team1 { get; set; }
        public virtual GameMember JammerTeam2 { get; set; }
        public virtual GameMember PivotTeam2 { get; set; }
        public virtual GameMember Blocker1Team2 { get; set; }
        public virtual GameMember Blocker2Team2 { get; set; }
        public virtual GameMember Blocker3Team2 { get; set; }
        public virtual GameMember Blocker4Team2 { get; set; }

        [Required]
        public virtual Game Game { get; set; }

        #endregion


        public GameJam()
        {
            LeadJammers = new Collection<GameLeadJammer>();
            JamPasses = new Collection<GameJamPasses>();
        }
    }
}
