using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Util;

namespace RDN.Models.Home
{
    public class HomeModel
    {
             public Tumblr Tumblr { get; set; }
        public int LeagueCount { get; set; }
        public DisplayStore Store { get; set; }
        public List<Tournament> Tournaments { get; set; }

        public HomeModel()
        {
            Tournaments = new List<Tournament>();
        }
    }
}