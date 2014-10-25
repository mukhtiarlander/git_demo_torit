using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Messages
{
    /// <summary>
    /// actual message between users.
    /// </summary>
    [Table("RDN_Messages")]
    public class Message : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MessageId { get; set; }
        public string MessageText { get; set; }
        public Member.Member FromUser { get; set; }

        public virtual GroupMessage GroupBelongsTo { get; set; }
        public virtual ICollection<MessageInbox> MessagesInbox { get; set; }
    }
}
