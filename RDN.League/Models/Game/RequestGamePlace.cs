using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Federation.Enums;
using RDN.Portable.Classes.Federation.Enums;

namespace RDN.League.Models.Game
{
    public class RequestGamePlace
    {
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; }
        [DataType(DataType.Date) ]
        public DateTime DateOfEvent { get; set; }
        public string Location { get; set; }
        public bool IsOfferTravelStipend { get; set; }
        public string TravelStipendAmount { get; set; }
        public double CrowdSize { get; set; }
        public string CurrentRanking { get; set; }
        public bool IsStreamLive { get; set; }
        public bool IsSkaterHousingOffered { get; set; }
        public bool IsHangOverBoutOffered { get; set; }

        public RequestGamePlace()
        {
            RuleSetList = new List<RuleSetsUsedEnum>();
        }
        
        public int RuleSetId { get; set; }
        public IEnumerable<RuleSetsUsedEnum> RuleSetList { get; set; }
        public string EventInformation { get; set; }
    }
}