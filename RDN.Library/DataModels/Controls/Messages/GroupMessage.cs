using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Messages
{
    [Table("RDN_Messages_Group")]
    public class GroupMessage : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GroupId { get; set; }
        public string TitleOfMessage { get; set; }
        public Guid GroupOwnerId { get; set; }
        public int GroupOwnerTypeEnum { get; set; }
        public bool IsDeleted{ get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<MessageRecipient> Recipients { get; set; }

        public GroupMessage()
        {
            Messages = new Collection<Message>();
            Recipients = new Collection<MessageRecipient>();
        }
    }
}
