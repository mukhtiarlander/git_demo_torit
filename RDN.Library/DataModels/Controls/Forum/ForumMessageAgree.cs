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
    [Table("RDN_Forum_Message_I_Agree")]
    public class ForumMessageAgree : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IAgreeId { get; set; }
        public long TotalCount { get; set; }

        #region Reference

        public virtual ForumMessage Messages { get; set; }
        public virtual Member.Member Member { get; set; }

        #endregion
    }
}
