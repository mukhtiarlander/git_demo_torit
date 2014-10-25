using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Federation.Enums
{
    /// <summary>
    /// used to define the organizations of derby.
    /// </summary>
    [Flags]
    public enum FederationsEnum
    {
        None = 1,
        MADE = 4,
        //MADE_Coed = 8,
        OSDA = 16,
        //OSDA_Coed = 32,

        RDCL = 64,
        Renegade = 128,
        Texas_Derby = 256,
        WFTDA = 2,
        USARS = 512,
        MRDA = 1024,
        RDAC = 2048,
        JRDA = 4096,
        ERRD_AU = 8192
    }
}
