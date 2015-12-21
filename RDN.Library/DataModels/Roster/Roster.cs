using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RDN.Library.DataModels.Base;

namespace RDN.Library.DataModels.Roster
{
    [Table("RDN_Roster")]
    public class Roster : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RosterId { get; set; }

        [Required]
        public string RosterName { get; set; }
        
        public DateTime GameDate { get; set; }

        public long RuleSetsUsedEnum { get; set; }

        #region References

        public virtual RDN.Library.DataModels.League.League League { get; set; }

        public virtual ICollection<RosterMember> RosterMembers { get; set; }

        #endregion

        #region Methods

        public Roster()
        {
            RosterMembers = new Collection<RosterMember>();
        }

        #endregion
    }
}
