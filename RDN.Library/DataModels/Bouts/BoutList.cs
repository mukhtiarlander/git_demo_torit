using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Bouts
{
    [Table("RDN_BoutList_Request")]
    public class BoutList : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ChallengeId { get; set; }

        public DateTime DateOfEvent { get; set; }
        public DateTime? EndDateOfEvent { get; set; }
        public string Location { get; set; }
        public bool IsOfferTravelStipend { get; set; }
        public double CrowdSize { get; set; }
        public bool IsStreamLive { get; set; }
        public bool IsSkaterHousingOffered { get; set; }
        public bool IsHangOverBoutOffered { get; set; }
        public int RuleSetId { get; set; }
        public string TravelStipendAmount { get; set; }
        public string CurrentRanking { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublic { get; set; }
        public bool IsAwayGame { get; set; }

        public string EventInformation { get; set; }
        public virtual RDN.Library.DataModels.League.League League { get; set; }
    }
}
