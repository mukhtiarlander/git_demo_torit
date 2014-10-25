using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Game.Tournaments
{
  public   class TournamentRoundsClass
    {
        public long Id { get; set; }
        public long RoundNumber { get; set; }
        public  List<TournamentPairingClass> Pairings { get; set; }

        public TournamentRoundsClass()
        {
            Pairings = new List<TournamentPairingClass>();
        }
    }
}
