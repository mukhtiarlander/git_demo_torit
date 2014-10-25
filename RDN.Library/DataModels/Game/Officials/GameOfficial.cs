using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Game.Officials
{
    [Table("RDN_Game_Officials")]
    public class GameOfficial : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GameMemberId { get; set; }

        public Guid GameOfficialId { get; set; }

        //member id of the actual member table for RDNation
        public Guid MemberLinkId { get; set; }
        [MaxLength(200)]
        [Required]
        public string MemberName { get; set; }
        [MaxLength(50)]
        public string MemberNumber { get; set; }
        
        public int OfficialTypeEnum { get; set; }
        public int RefereeType { get; set; }
        public byte CertificationLevelEnum { get; set; }
        public GameOfficial()
        {
            Photos = new Collection<GameOfficialPhoto>();
        }


        #region References

        [Required]
        public virtual Game Game { get; set; }
        public virtual ICollection<GameOfficialPhoto> Photos { get; set; }
        #endregion
    }
}
