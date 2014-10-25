using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League.Group
{
    /// <summary>
    /// shows who the members of the federation are.
    /// which is added by the federation.
    /// </summary>
    [Table("RDN_League_Group_Members")]
    public class GroupMember : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// declares the member type for them.  Inactive, Active etc...
        /// </summary>
        public int MemberAccessLevelEnum { get; set; }
        /// <summary>
        /// turns notifications off for any emails.
        /// </summary>
        public bool TurnOffEmailNotifications { get; set; }
        public bool TurnOffBroadcastNotifications { get; set; }
        public string Notes { get; set; }
        /// <summary>
        /// we can't delete member so we just set a flag to remove it.
        /// </summary>
        public bool IsMemRemoved { get; set; }

        #region references

        [Required]
        public virtual Group Group { get; set; }
        [Required]
        public virtual Member.Member Member { get; set; }

        #endregion
    }
}
