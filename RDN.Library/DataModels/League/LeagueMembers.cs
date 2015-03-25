using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    /// <summary>
    /// shows who the members of the federation are.
    /// which is added by the federation.
    /// </summary>
    [Table("RDN_League_Members")]
    public class LeagueMember : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// declares the member type for them.  Inactive, Active etc...
        /// </summary>
        public int MemberType { get; set; }

        /// <summary>
        /// any comments the federation has for this member.
        /// </summary>
        public string CommentsForMember { get; set; }
        /// <summary>
        /// turns notifications off for any email notifications
        /// </summary>
        public bool TurnOffEmailNotifications { get; set; }
        public bool IsInactiveForLeague { get; set; }

        public DateTime? MembershipDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? PassedWrittenExam { get; set; }
        public bool HasLeftLeague { get; set; }
        public DateTime? SkillsTestDate { get; set; }
        public string Notes { get; set; }
        [Obsolete]
        public byte LeagueOwnersEnum { get; set; }
        public int LeagueOwnersEnums { get; set; }

        #region references
        public LeagueMemberClass SkaterClass { get; set; }
        [Required]
        public virtual League League { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }
        #endregion
    }
}
