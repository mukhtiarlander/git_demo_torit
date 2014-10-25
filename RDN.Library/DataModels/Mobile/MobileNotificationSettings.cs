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
    [Table("RDN_Mobile_Notification_Settings")]
    public class MobileNotificationSettings : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string NotificationId { get; set; }
        public byte MobileTypeEnum { get; set; }
        //public bool CanSendForumNotifications { get; set; }
        public bool AllowGameNotifications { get; set; }
        public virtual Member.Member Member { get; set; }
        
    }
}
