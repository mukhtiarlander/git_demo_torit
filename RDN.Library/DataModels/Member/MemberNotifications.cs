using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Member
{

    [Table("RDN_Member_Notifications")]
    public class MemberNotifications : InheritDb
    {
        /// <summary>
        /// 
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NotificationId { get; set; }

        /// <summary>
        /// notified via email of forum broadcasts
        /// </summary>
        public bool EmailForumBroadcastsTurnOff { get; set; }
        public bool EmailForumWeeklyRoundupTurnOff { get; set; }
        /// <summary>
        /// email notify about brand new forum post.
        /// </summary>
        public bool EmailForumNewPostTurnOff { get; set; }

        public bool EmailCalendarNewEventBroadcastTurnOff { get; set; }
        public bool EmailMessagesReceivedTurnOff { get; set; }
        
        
    }
}
