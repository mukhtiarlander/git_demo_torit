using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.League.Models.Utilities
{
    [Flags]
    public enum MemberStatusTypes
    {
        Member = 0x01,
        Manager = 0x02,
        Treasurer = 0x04,
        President = 0x08
    }
}