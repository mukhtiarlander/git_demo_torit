using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using RDN.Library.DataModels.Calendar.EventsCalendar;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{
    [Table("RDN_Calendar_Item_Reoccuring_Groups")]
    public class CalendarEventReoccuringGroup : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CalendarItemId { get; set; }

        public virtual League.Group.Group Group { get; set; }
        public virtual CalendarEventReoccuring Event { get; set; }


        public CalendarEventReoccuringGroup()
        {

        }
    }
}
