using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.League.Enums
{

    [Flags]
    public enum LeagueOwnersEnum
    {
        None = 1,
        Owner = 2,
        Manager = 4,
        Head_Ref = 8,
        Treasurer = 16,
        Secretary = 32,
        Head_NSO = 64,
        Shops = 128,
        Events_Coord = 256,
        Medical = 512,
        Attendance = 1024,
        Polls = 2048,
        Inventory = 4096,
        Sponsorship = 8192
    }
}
