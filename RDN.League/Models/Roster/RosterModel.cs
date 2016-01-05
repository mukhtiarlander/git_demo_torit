using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Util;

namespace RDN.League.Models.Roster
{
    public class RosterModel
    {
        public long RosterId { get; set; }

        public string RosterName { get; set; }

        public DateTime GameDate { get; set; }

        public Guid LeagueId { get; set; }

        public List<KeyValueHelper> Members { get; set; }

        public string RosterMemberIds { get; set; }

        public int RosterSize { get; set; }

        public List<KeyValueHelper> RosterMembers { get; set; }

        public int InsuranceTypeId { get; set; }

        public long RuleSetsUsedEnum { get; set; }

        public SelectList InsuranceTypes { get; set; }

        public SelectList RuleSets { get; set; }
    }
}