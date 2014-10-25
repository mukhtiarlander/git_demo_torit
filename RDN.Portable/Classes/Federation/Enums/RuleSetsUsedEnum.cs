using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Federation.Enums
{
    /// <summary>
    /// only for defining the rule sets of derby.
    /// </summary>
    [Flags]
    public enum RuleSetsUsedEnum
    {
        None = 1,

        MADE = 4,
        //MADE_Coed = 8,
        OSDA = 16,
        //OSDA_Coed = 32,

        RDCL = 64,
        Renegade = 128,
        Texas_Derby = 256,
        The_WFTDA = 2,
        USARS = 512

    }
}
