using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tournaments.Standard.Records
{
    public  class RRRank
    {
        public TournamentTeam Team { get; set; }
        public int Rank { get; set; }
        public RRWinRecord Record { get; set; }
    }
}
