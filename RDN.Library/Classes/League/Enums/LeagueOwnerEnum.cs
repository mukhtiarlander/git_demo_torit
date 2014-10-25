using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League.Enums
{
    [Obsolete("Use LeagueOwnersEnum")]
    public enum LeagueOwnerEnum
    {
        None = 0,
        //can do anything in the league
        Owner = 1,
        //can add and edit members in league.
        Manager = 2,
        //can upload old games
        HeadNso = 3
    }

}
