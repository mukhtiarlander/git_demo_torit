using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Members
{
    [Table("RDN_Game_Members")]
    public class GameMember : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid GameMemberDbId { get; set; }

        public Guid GameMemberId { get; set; }

        

        
       /// <summary>
        /// member id of the actual member table for RDNation
       /// </summary>
        public Guid MemberLinkId { get; set; }
        [MaxLength(200)]
        [Required]
        public string MemberName { get; set; }
        [MaxLength(50)]
        public string MemberNumber { get; set; }

        public GameMember()
        {
            Photos = new Collection<GameMemberPhoto>();
            GameMemberPenalties = new Collection<GameMemberPenaltyBox>();
            PointsScored = new Collection<GameScore>();
            Blocks = new Collection<GameMemberBlock>();
            Assists = new Collection<GameMemberAssist>();
        }


        #region References

        [Required]
        public virtual GameTeam Team { get; set; }

        public virtual ICollection<GameMemberPhoto> Photos { get; set; }

        public virtual ICollection<GameMemberPenaltyBox> GameMemberPenalties { get; set; }

        public virtual ICollection<GameScore> PointsScored { get; set; }
        public virtual ICollection<GameMemberBlock> Blocks { get; set; }
        public virtual ICollection<GameMemberAssist> Assists { get; set; }

        #endregion
    }
}
