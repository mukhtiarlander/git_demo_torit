using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Games.Tournament
{
   public  class TournamentBase
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid Id { get; set; }

        public long GetTotalSecondsToEvent
        {
            get
            {
                var ts = StartDate - DateTime.UtcNow;
                return (long)ts.TotalSeconds;
            }
        }
    }
}
