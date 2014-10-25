using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.MiniServer.Json.Announcer
{
   public  class LivePlayerStat
    {
       public string rosterName { get; set; }
       public string rosterNumber { get; set; }
       public int rosterJammerJams { get; set; }
       public int rosterJammerPts { get; set; }
       public int rosterBlockerJams { get; set; }    
       public string currentJam { get; set; }
       public string rosterBlockerPointsPer { get; set; }
       public string rosterPens { get; set; }
       public LivePlayerStat()
       { }
    }
}
