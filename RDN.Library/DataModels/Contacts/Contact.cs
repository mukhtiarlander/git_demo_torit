using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Federation;
using RDN.Library.DataModels.League;
using RDN.Library.DataModels.Team;
using System.ComponentModel.DataAnnotations.Schema;


namespace RDN.Library.DataModels.Contacts
{
    [Table("RDN_Contacts")]
    public class Contact : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContactId { get; set; }

        [MaxLength(255)]
        public string Firstname { get; set; }
        [MaxLength(255)]
        public string Lastname { get; set; }

        public string CompanyName { get; set; }
        public string Link { get; set; }

        public bool IsViewableToLeagueMember { get; set; }

        #region Constructor
        public Contact()
        {

        }
        #endregion

        #region References
        public virtual ContactCard.ContactCard ContactCard { get; set; }
        #endregion
    }
}
