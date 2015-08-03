using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Controls.Forum;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.ObjectModel;

namespace RDN.Library.DataModels.Forum
{
    [Table("RDN_Forum_Message")]
    public class ForumMessage : InheritDb
    {
        public ForumMessage()
        {
            Mentions = new Collection<ForumMessageMention>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MessageId { get; set; }

        public string MessageHTML { get; set; }
        public string MessagePlain { get; set; }
        public bool IsRemoved { get; set; }


        #region References
        [Required]
        public virtual Member.Member Member { get; set; }

        [Required]
        public virtual ForumTopic Topic { get; set; }

        public virtual ICollection<ForumMessageLike> MessagesLike { get; set; }
        public virtual ICollection<ForumMessageAgree> MessagesAgree { get; set; }
        public virtual ICollection<ForumMessageMention> Mentions { get; set; } 
        
        #endregion
    }
}
