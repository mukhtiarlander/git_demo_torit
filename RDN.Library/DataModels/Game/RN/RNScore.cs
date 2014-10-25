using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Game.Members;


namespace RDN.Library.DataModels.Game
{
    [Table("RDN_Game_RN_Score")]
    public class RNScore : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ScoreId { get; set; }

        //id of the score from the game it self generated on the client machine.
        public string Team1Name { get; set; }
        public int Team1Score { get; set; }
        public string Team2Name { get; set; }
        public int Team2Score { get; set; }
        public DateTime? GameDateTime { get; set; }
        public long RuleSetEnum { get; set; }
        public bool IsPublished { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? PublishDateTime { get; set; }
        public long SanctionedByFederationEnum { get; set; }

        public string Notes { get; set; }

        public bool EmailWhenBoutIsPosted { get; set; }

        public virtual Member.Member UserSubmitted { get; set; }

        public virtual League.League League1 { get; set; }
        public virtual League.League League2 { get; set; }
        public virtual Tournaments.GameTournament Tournament { get; set; }
    }
}
