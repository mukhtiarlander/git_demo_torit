using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Account.Enums
{
    public enum DefaultPositionEnum
    {
        //user must select 1 or better upon sign up, so we set 0 to the DB once they have joined a team, started a league or owned a federation.
        CompletedInitialRegistration = 0,
        The_Owner_of_a_Team_League = 1,
        A_Player = 2,
        //The_Owner_of_a_Federation = 3,
        Referee = 4,
        Official = 5,
        Other = 6,
        Writer_For_Sport_News = 7



    }
}
