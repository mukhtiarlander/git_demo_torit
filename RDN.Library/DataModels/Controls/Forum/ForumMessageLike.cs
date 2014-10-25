using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Forum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Controls.Forum
{
    [Table("RDN_Forum_Like")]
    public class ForumMessageLike : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long LikeId { get; set; }
        public long TotalCount { get; set; }

        #region Reference

        public virtual ForumMessage Messages { get; set; }
        public virtual Member.Member Member { get; set; }

        #endregion
    }
}
