using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Location;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_League_Pendings")]
    public class Pending : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LeagueId { get; set; }
        [MaxLength(255)]
        public string LeagueName { get; set; }
        [MaxLength(255)]
        public string ContactTelephone { get; set; }
        [MaxLength(255)]
        public string ContactEmail { get; set; }
        [Column(TypeName = "text")]
        public string AdditionalInformation { get; set; }
        // NVAR MAX
        public string LogInformation { get; set; }
        [MaxLength(255)]
        public string StateRaw { get; set; }
        [MaxLength(255)]
        public string CityRaw { get; set; }

        public double TimeZone { get; set; }

        #region References

        
        public virtual Federation.Federation Federation { get; set; }
        public virtual Country Country { get; set; }
        [Required]
        public virtual Member.Member Creator { get; set; }

        #endregion
    }
}
