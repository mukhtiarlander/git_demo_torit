using System;
using System.Collections.Generic;
using RDN.Library.Util;

namespace RDN.Library.Classes.Roster
{
    public class Roster
    {
        public long RosterId { get; set; }

        public string RosterName { get; set; }

        public DateTime GameDate { get; set; }

        public Guid LeagueId { get;set; }

        public List<KeyValueHelper> Members { get; set; }

        public string RosterMemberIds { get; set; }

        public int RosterSize { get; set; }

        public List<KeyValueHelper> RosterMembers { get; set; }

        public int InsuranceTypeId { get; set; }

        public long RuleSetsUsedEnum { get; set; }

    }
}