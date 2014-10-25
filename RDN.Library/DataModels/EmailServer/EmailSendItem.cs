using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.EmailServer.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.EmailServer
{
    /// <summary>
    /// An email. This item represents an email and will be sent via the email server.
    /// </summary>
    [Table("RDN_EmailServer")]
    internal class EmailSendItem : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailSendItemId { get; set; }
        [Required]
        public string Reciever { get; set; }
        [Required]
        public string From { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string DisplayNameFrom { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public byte Prio { get; set; }
        /// <summary>
        /// The layout of the email. The layouts are stored on the server.
        /// </summary>        
        [Required]
        [MaxLength(100)]
        public string EmailLayout { get; set; }

        //public int? EmailType_EmailTypeId { get; set; }
        /// <summary>
        /// All properties that is attached to this email. Each property will be parsed into the layout and then sent.
        /// </summary>
        public ICollection<EmailProperty> Properties { get; set; }

        /// <summary>
        /// The priority of the email. Emails marked as important are sent much faster.
        /// </summary>
        [NotMapped]
        public EmailPriority Priority
        {
            get
            {
                if (Prio == 1)
                    return EmailPriority.Important;
                return EmailPriority.Normal;
            }
            set
            {
                Prio = (byte)value;
            }
        }


        public EmailSendItem()
        {
            Prio = 0;
            Properties = new Collection<EmailProperty>();
        }

        public EmailSendItem(EmailPriority priority)
        {
            Prio = (byte)priority;
            Properties = new Collection<EmailProperty>();
        }
    }


}
