using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.BruiseBash
{
    [Table("RDN_BruiseBash_Comments")]
    public class BruiseBashComment : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CommentId { get; set; }

        [Required]
        public string Comment { get; set; }




        #region References
        public virtual Member.Member Owner { get; set; }
        public virtual BruiseBashItem BruiseBashItem { get; set; }
        #endregion
    }
}
