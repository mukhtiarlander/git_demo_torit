using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{
    [Table("RDN_Calendar_Item_Type")]
    public class CalendarEventType : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CalendarEventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public int PointsForPresent { get; set; }
        public int PointsForPartial { get; set; }
        public int PointsForNotPresent { get; set; }
        public int PointsForExcused { get; set; }
        public int PointsForTardy { get; set; }

        public virtual Color.Color DefaultColor { get; set; }
        public virtual Calendar CalendarOwner { get; set; }
    }
}
