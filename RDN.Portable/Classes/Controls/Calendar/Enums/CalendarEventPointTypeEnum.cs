using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar.Enums
{
    public class CalendarEventPointTypeEnumHelper
    {
        public static List<string> CalendarEventPointTypes
        {
            get
            {
return Enum.GetValues(typeof(CalendarEventPointTypeEnum)).Cast<CalendarEventPointTypeEnum>().Select(v => v.ToString().Replace("_", " ")).ToList();
            }
        }
    }

    public enum CalendarEventPointTypeEnum
    {
        None = 0,
        Present = 1,
        Partial = 2,
        Not_Present = 3,
        Excused = 4,
        Tardy = 5
    }
}
