using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Controls.Calendar.Enums
{
    public class AvailibilityEnumHelper
    {
        public static List<string> AvailibilityEnumTypes
        {
            get
            {
                return Enum.GetValues(typeof(AvailibilityEnum)).Cast<AvailibilityEnum>().Select(v => v.ToString().Replace("_", " ")).ToList();
            }
        }
    }
    public enum AvailibilityEnum
    {
        None = 0,
        Going = 1,
        Not_Going = 2,
        Maybe_Going = 3
    }
}
