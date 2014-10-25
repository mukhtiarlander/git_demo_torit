using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations; 
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Forum
{
    /// <summary>
    /// this is just a notification table.  Notifies members there is a topic they haven't read yet.
    /// </summary>
    [Table("RDN_Forum_Topic_Watch_List")]
    public class ForumTopicWatchList : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InboxTopicId { get; set; }

        public virtual ForumTopic Topic { get; set; }
        public virtual Member.Member ToUser { get; set; }

    }
}
