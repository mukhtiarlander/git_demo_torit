using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Portable.Classes.Account.Enums
{
     [Flags]
    public enum MemberType
    {
        None = 0,
        Player = 1,
        Coach = 2,
        Referee = 4,
        Volunteer = 8,
        Official = 16
    }
}
