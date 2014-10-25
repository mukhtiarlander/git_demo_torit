using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League.Group
{
    [Table("RDN_League_Group")]
    public class Group : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string GroupName { get; set; }
        public int GroupTypeEnum { get; set; }
        public bool IsPublicToWorld { get; set; }
        /// <summary>
        /// braodcase forum message to everyone
        /// </summary>
        public bool BroadcastToEveryone { get; set; }

        /// <summary>
        /// group was deleted from the league.
        /// </summary>
        public bool IsGroupRemoved { get; set; }
        #region references
        public virtual ICollection<GroupPhoto> Logos { get; set; }
        [Required]
        public virtual League League { get; set; }
        [Required]
        public virtual ICollection<GroupMember> GroupMembers { get; set; }
        public virtual ContactCard.ContactCard ContactCard { get; set; }

        #endregion

        public Group()
        {
            Logos = new Collection<GroupPhoto>();
            GroupMembers = new Collection<GroupMember>();
        }
    }
}
