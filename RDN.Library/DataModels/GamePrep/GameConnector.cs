using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.GamePrep
{
    [Table("RDN_GamePrep_Connector")]
    public class GameConnector : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GameConnectorId { get; set; }

        [Required]
        [MaxLength(20)]
        public string GameKey { get; set; }

        public DateTime? GameDate { get; set; }
        [MaxLength(200)]
        public string GameLocation { get; set; }
        public bool IsPending { get; set; }

        [Required]
        public virtual Team.Team ChallengerTeam { get; set; }

        public virtual Team.Team OpposingTeam { get; set; }

        public GameConnector()
        {
            IsPending = true;
        }
    }
}
