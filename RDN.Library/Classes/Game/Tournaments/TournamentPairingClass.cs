using Scoreboard.Library.ViewModel.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Game.Tournaments
{
  public   class TournamentPairingClass
    {
        public long Id { get; set; }
        public long PairingId { get; set; }
        public string TimeToStartDisplay { get; set; }
        public DateTime TimeToStart { get; set; }
        public string TrackId { get; set; }
        public Guid GameId { get; set; }
        public int GroupId { get; set; }
      
        public List<TeamViewModel> Teams { get; set; }

        public TournamentPairingClass()
        {
            Teams = new List<TeamViewModel>();
        }
    }
}
