using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League
{
    [Obsolete]
    [Table("RDN_League_Sponsorship")]
    public class Sponsorship : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SponsorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PromoCode { get; set; }
        public string Website { get; set; }
        public bool IsDeleted { get; set; }
        public long UsedCount { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Sponsored { get; set; }

        #region References
        public virtual RDN.Library.DataModels.League.League SponsorForLeague { get; set; }
        public virtual RDN.Library.DataModels.Member.Member SponsorAddByMember { get; set; }
        #endregion

    }
}
