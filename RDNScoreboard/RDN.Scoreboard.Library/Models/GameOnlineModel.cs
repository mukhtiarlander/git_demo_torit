using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scoreboard.Library.Models
{
    public class GameOnlineModel
    {
        public string result { get; set; }
        /// <summary>
        /// url for administering game online.
        /// </summary>
        public string url { get; set; }
        public string id { get; set; }
        public bool IsGameOnline { get; set; }
    }
}
