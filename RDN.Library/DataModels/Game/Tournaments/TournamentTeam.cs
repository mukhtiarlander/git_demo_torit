using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Game.Members;
using RDN.Library.DataModels.Game.Tournaments;
using System.ComponentModel.DataAnnotations.Schema;


namespace RDN.Library.DataModels.Game
{
    [Table("RDN_Game_Tournament_Teams")]
    public class TournamentTeam : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamId { get; set; }
        /// <summary>
        /// this is the id for the team in the game from the scoreboard.
        /// </summary>
        //public Guid TeamIdInGame { get; set; }
        // public Guid TeamScoreboardId { get; set; }
        //this is the team id for the actual teams of RDNation
        //public Guid TeamIdLink { get; set; }
        public string LeageName { get; set; }
        public string TeamName { get; set; }
        public bool IsRemoved { get; set; }
        public int SeedRating { get; set; }
        public int PoolNumber { get; set; }
        public Guid LogoId { get; set; }

        #region References

        [Required]
        public virtual GameTournament Tournament { get; set; }
        public virtual Team.TeamLogo Logo { get; set; }
        public virtual List<TournamentPairingTeam> Pairs { get; set; }
        #endregion

        #region Contructor

        public TournamentTeam()
        {
            Pairs = new List<TournamentPairingTeam>();
        }

        #endregion
    }
}
