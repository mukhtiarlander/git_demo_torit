using System;

namespace RDN.Library.Classes.League.Classes
{
    public class LeagueList
    {
        public Guid LeagueId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string LogoPath { get; set; }
    }
}
