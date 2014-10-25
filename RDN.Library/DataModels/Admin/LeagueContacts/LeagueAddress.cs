using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Admin.LeagueContacts
{
    [Table("RDN_Admin_League_Addresses")]
    public class LeagueAddress : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeagueAddressId { get; set; }
        [MaxLength(255)]
        public string EmailAddress { get; set; }
        public bool IsMain { get; set; }
        public string Comments { get; set; }

        #region References

        [Required]
        public virtual ContactLeague ContactLeague { get; set; }

        #endregion
    }
}
