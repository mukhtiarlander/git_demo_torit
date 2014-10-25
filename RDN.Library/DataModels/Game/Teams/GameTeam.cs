using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Game.Members;
using System.ComponentModel.DataAnnotations.Schema;


namespace RDN.Library.DataModels.Game
{
    [Table("RDN_Game_Teams")]
    public class GameTeam : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GameTeamId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid TeamId { get; set; }
        // public Guid TeamScoreboardId { get; set; }
        //this is the team id for the actual teams of RDNation
        public Guid TeamIdLink { get; set; }
        public string LeageName { get; set; }
        [MaxLength(200)]
        [Required]
        public string TeamName { get; set; }
        [Required]
        public int CurrentTimeouts { get; set; }
        [Required]
        public int CurrentScore { get; set; }

        public Guid LogoId { get; set; }

        #region References

        [Required]
        public virtual Game Game { get; set; }
        public virtual Team.TeamLogo Logo { get; set; }
        public virtual Teams.GameTeamLineupSettings LineupSettings { get; set; }
        public virtual ICollection<GameMember> GameMembers { get; set; }
        public virtual ICollection<GameScore> GameScores { get; set; }

        #endregion

        #region Contructor

        public GameTeam()
        {
            GameMembers = new Collection<GameMember>();
            GameScores = new Collection<GameScore>();
        }

        #endregion
    }
}
