using Scoreboard.Library.ViewModel.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Game.Tournaments
{
  public   class TournamentStandings
    {
            public double Rank { get; set; }

            public TeamViewModel Team { get; set; }

            public string Details { get; set; }
        
    }
}
