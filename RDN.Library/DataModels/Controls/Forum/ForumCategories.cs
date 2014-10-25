using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Forum
{
    [Table("RDN_Forum_Category")]
    public class ForumCategories : InheritDb
    {
        public ForumCategories()
        {
            Topics = new Collection<ForumTopic>();
        }


        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CategoryId { get; set; }
        public string NameOfCategory { get; set; }
        public bool IsRemoved { get; set; }
        public long GroupId { get; set; }

        #region References

        //[Required]
        public virtual Member.Member CreatedByMember { get; set; }
        //[Required]
        public virtual Forum Forum { get; set; }
        public virtual ICollection<ForumTopic> Topics { get; set; }

        #endregion
    }
}
