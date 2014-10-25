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
    [Table("RDN_League_Contacts")]
    public class LeagueContacts: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// declares the contact type for them.  
        /// </summary>
        public byte ContactTypeEnum { get; set; }

        public string Notes { get; set; }
        

        #region references
        [Required]
        public virtual League League { get; set; }
        [Required]
        public virtual Contacts.Contact Contact{ get; set; }
        #endregion
    }
}
