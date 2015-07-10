using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Calendar.Enums
{
    /// <summary>
    /// when updating thes points, make sure to update the main.js file and the check in files
    /// within league
    /// </summary>
    public enum CalendarPointTypeEnum
    {
        None = 0,
        Showed = 1,
        Late = 2,
        Excuse = 3
    }
}
