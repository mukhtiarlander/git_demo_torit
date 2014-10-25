using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.MiniServer.Json
{
  public   class TeamJson
    {
      public string teamId { get; set; }
      public string teamName { get; set; }
      public int currentJam { get; set; }
      public string currentJamId { get; set; }
      public int totalJams { get; set; }
      public List<TeamMemberJson> members { get; set; }
      public string gameName { get; set; }
      public TeamJson()
      {
          members = new List<TeamMemberJson>();
      }
    }
}
