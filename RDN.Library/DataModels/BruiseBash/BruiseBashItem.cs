using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.BruiseBash
{
    [Table ("RDN_BruiseBash_Items")]
    public class BruiseBashItem : InheritDb
    {
        public BruiseBashItem() {
            Comments = new Collection<BruiseBashComment>();
            Ratings = new Collection<BruiseBashRating>();
        }


        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BruiseBashId { get; set; }
        /// <summary>
        /// Title of bruise
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A user generated story about the bruise
        /// </summary>
        public string Story { get; set; }


        #region References
        public virtual Member.Member Owner { get; set; }
        public virtual ICollection<BruiseBashComment> Comments { get; set; }
        [Required]
        public virtual BruiseBashImage Image { get; set; }
        public virtual ICollection<BruiseBashRating> Ratings { get; set; }

        #endregion
    }
}
