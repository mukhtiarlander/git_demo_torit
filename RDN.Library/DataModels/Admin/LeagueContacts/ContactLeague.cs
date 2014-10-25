using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Location;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Admin.LeagueContacts
{
    [Table("RDN_Admin_Leagues")]
    public class ContactLeague : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeagueId { get; set; }        
        [MaxLength(255)]
        public string State { get; set; }
        [MaxLength(255)]
        public string City { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(255)]        
        public string HomePage { get; set; }
        [MaxLength(255)]        
        public string Facebook { get; set; }
        public string Comments { get; set; }

        #region References

        public virtual Country Country { get; set; }        
        public virtual ICollection<LeagueAddress> LeagueAddresses { get; set; }
        public virtual LeagueAssociation LeagueAssociation { get; set; }
        public virtual LeagueType LeagueType { get; set; }

        #endregion

        #region Methods

        public ContactLeague()
        {
            LeagueAddresses = new Collection<LeagueAddress>();
        }

        #endregion
    }
}
