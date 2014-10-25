using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.EmailServer
{
    [Obsolete("Use RDN.Core.*")]
    [Table("RDN_Email_Scubscribers")]
    public class SubscribersList : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UnSubscribeId { get; set; }
        [MaxLength(255)]
        public string EmailToAddToList { get; set; }
    }
}
