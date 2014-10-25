using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Forum
{
    [DebuggerDisplay("[{this.TopicId} {this.TopicTitle}]")]
    [Table("RDN_Forum_Topic")]
    public class ForumTopic : InheritDb
    {
        public ForumTopic()
        {
            Messages = new Collection<ForumMessage>();
            TopicsInbox = new Collection<ForumTopicInbox>();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TopicId { get; set; }
        public string TopicTitle { get; set; }
        public int ViewCount { get; set; }
        public long GroupId { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? LastPostDateTime { get; set; }
        /// <summary>
        /// sits at the top of the forum
        /// </summary>
        public bool IsSticky { get; set; }
        /// <summary>
        /// can't add any more messages to topic.
        /// </summary>
        public bool IsLocked { get; set; }
        public bool IsArchived { get; set; }

        #region References
        [Required]
        public virtual Member.Member LastPostByMember { get; set; }
        [Required]
        public virtual Member.Member CreatedByMember { get; set; }
        [Required]
        public virtual Forum Forum { get; set; }
        public virtual ForumCategories Category { get; set; }
        public virtual ICollection<ForumMessage> Messages { get; set; }
        public virtual ICollection<ForumTopicInbox> TopicsInbox { get; set; }
        public virtual ICollection<ForumTopicWatchList> TopicWatchList { get; set; }

        #endregion
    }
}
