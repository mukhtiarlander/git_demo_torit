using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Models.League
{
    public class LeagueModel : RDN.Portable.Classes.League.Classes.League
    {
        [AllowHtml]
        public string InternalWelcomeMessageModel { get; set; }
        public SelectList ColorList { get; set; }
        public SelectList CultureList { get; set; }

    }
}