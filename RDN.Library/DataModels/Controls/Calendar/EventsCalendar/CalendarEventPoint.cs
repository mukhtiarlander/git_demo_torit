using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Calendar
{
    [Table("RDN_Calendar_Item_Points")]
    public class CalendarEventPoint : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CalendarPointId { get; set; }
        [Required]
        public int PointsForEvent { get; set; }
        [Required]
        public int PointTypeEnum { get; set; }
        

    }
}
