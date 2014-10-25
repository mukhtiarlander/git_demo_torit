using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Federation
{
    /// <summary>
    /// Federations including MADE, WFTDA, MRD etc...
    /// </summary>
    [Table("RDN_Federations")]
    public class Federation : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FederationId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public bool IsVerified { get; set; }

        public DateTime? Founded { get; set; }
        public string Website { get; set; }

        #region references
        public virtual FederationPhoto Logo { get; set; }
        public virtual ContactCard.ContactCard ContactCard { get; set; }
        public virtual ICollection<FederationOwnership> Owners { get; set; }
        public virtual ICollection<FederationMember> Members { get; set; }
        //public virtual ICollection<League.League> Leagues { get; set; }
        public virtual ICollection<FederationLeague> Leagues { get; set; }
        #endregion

        #region Methods

        public Federation()
        {
            Owners = new Collection<FederationOwnership>();
            Members = new Collection<FederationMember>();
        }

        #endregion
    }
}
