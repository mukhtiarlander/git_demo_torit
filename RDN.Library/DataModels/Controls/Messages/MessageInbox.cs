using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Messages
{
    [Table("RDN_Messages_Inbox")]
    public class MessageInbox : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InboxMessageId { get; set; }

        public bool IsRead { get; set; }
        public DateTime MessageReadDateTime { get; set; }
        public bool UserNotifiedViaEmail { get; set; }
        public DateTime? NotifiedEmailDateTime { get; set; }

        public virtual Message Message { get; set; }
        public virtual Member.Member ToUser { get; set; }
        
    }
}
