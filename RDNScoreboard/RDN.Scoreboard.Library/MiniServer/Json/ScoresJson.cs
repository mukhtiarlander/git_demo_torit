using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.MiniServer.Json
{
  public   class ScoresJson
    {

      public int totalScores { get; set; }
      public int scoreForJam { get; set; }
      public int pointsForPass { get; set; }

      public ScoresJson()
      { }
    }
}
