using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Messages
{
    /// <summary>
    /// recipients of the group. so we know who to assign messages to.
    /// </summary>
    [Table("RDN_Messages_Group_Recipients")]
    public class MessageRecipient : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RecipientId { get; set; }
        public bool IsRemovedFromGroup { get; set; }
        public virtual GroupMessage Group { get; set; }
        public virtual Member.Member Recipient { get; set; }
    }
}
