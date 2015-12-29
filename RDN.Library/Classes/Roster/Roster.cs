using System;
using System.Collections.Generic;
using RDN.Library.Util;

namespace RDN.Library.Classes.Roster
{
    /// <summary>
    /// 
    /// </summary>
    public class Roster
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Roster"/> class.
        /// </summary>
        public Roster()
        {
            RosterMembers = new List<KeyValueHelper>();
            Members = new List<KeyValueHelper>();
        }

        /// <summary>
        /// Gets or sets the roster identifier.
        /// </summary>
        /// <value>
        /// The roster identifier.
        /// </value>
        public long RosterId { get; set; }

        /// <summary>
        /// Gets or sets the name of the roster.
        /// </summary>
        /// <value>
        /// The name of the roster.
        /// </value>
        public string RosterName { get; set; }

        /// <summary>
        /// Gets or sets the game date.
        /// </summary>
        /// <value>
        /// The game date.
        /// </value>
        public DateTime GameDate { get; set; }

        /// <summary>
        /// Gets or sets the league identifier.
        /// </summary>
        /// <value>
        /// The league identifier.
        /// </value>
        public Guid LeagueId { get;set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        public List<KeyValueHelper> Members { get; set; }

        /// <summary>
        /// Gets or sets the roster member ids.
        /// </summary>
        /// <value>
        /// The roster member ids.
        /// </value>
        public string RosterMemberIds { get; set; }

        /// <summary>
        /// Gets or sets the size of the roster.
        /// </summary>
        /// <value>
        /// The size of the roster.
        /// </value>
        public int RosterSize { get; set; }

        /// <summary>
        /// Gets or sets the roster members.
        /// </summary>
        /// <value>
        /// The roster members.
        /// </value>
        public List<KeyValueHelper> RosterMembers { get; set; }

        /// <summary>
        /// Gets or sets the insurance type identifier.
        /// </summary>
        /// <value>
        /// The insurance type identifier.
        /// </value>
        public int InsuranceTypeId { get; set; }

        /// <summary>
        /// Gets or sets the rule sets used enum.
        /// </summary>
        /// <value>
        /// The rule sets used enum.
        /// </value>
        public long RuleSetsUsedEnum { get; set; }

    }
}