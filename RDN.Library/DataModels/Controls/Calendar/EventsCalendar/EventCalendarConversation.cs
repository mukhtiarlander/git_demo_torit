using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar.EventsCalendar
{
    [Table("RDN_Calendar_Item_Conversation")]
    public class EventCalendarConversation : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ConversationId { get; set; }

        public virtual string Text { get; set; }
        
        #region References
        public virtual Member.Member Owner { get; set; }
        [Required]
        public virtual CalendarEvent CalEvent{ get; set; }
        #endregion
    }
}
