using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Calendar.Enums;

namespace RDN.Library.Classes.Calendar
{
   public  class CalendarEventPoint
    {
       public long PointId { get; set; }
       public int PointsForEvent { get; set; }
       public CalendarPointTypeEnum PointType {get;set;}
    }
}
