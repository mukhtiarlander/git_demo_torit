using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.ContactCard
{
    [Table("RDN_ContactCard")]
    public class ContactCard : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ContactCardId { get; set; }        

        #region References

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Communication> Communications { get; set; }
        public virtual ICollection<Email> Emails { get; set; }

        #endregion

        #region Methods

        public ContactCard()
        {
            Addresses = new Collection<Address>();
            Communications = new Collection<Communication>();
            Emails = new Collection<Email>();
        }

        #endregion
    }
}
