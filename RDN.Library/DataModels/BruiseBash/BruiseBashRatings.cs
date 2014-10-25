using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.BruiseBash
{
    [Table("RDN_BruiseBash_Ratings")]
    public class BruiseBashRating : InheritDb
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RatingId { get; set; }

        public string IpAddress { get; set; }

        #region References

        public virtual BruiseBashItem Winner { get; set; }
        public virtual BruiseBashItem Loser { get; set; }
        public virtual Member.Member Rater { get; set; }

        #endregion

    }
}
