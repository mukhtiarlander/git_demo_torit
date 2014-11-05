using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Util;
using RDN.Portable.Models.Json.Public;

namespace RDN.Models.Home
{
    public class HomeModel
    {
             public Tumblr Tumblr { get; set; }
        public int LeagueCount { get; set; }
        public int MemberCount{ get; set; }
        public DisplayStore Store { get; set; }
        public List<Tournament> Tournaments { get; set; }
        public List<LeagueJsonDataTable> RandomLeagues{ get; set; }
        public List<RDN.Portable.Models.Json.SkaterJson> RandomSkaters { get; set; }

        public HomeModel()
        {
            Tournaments = new List<Tournament>();
        }
    }
}