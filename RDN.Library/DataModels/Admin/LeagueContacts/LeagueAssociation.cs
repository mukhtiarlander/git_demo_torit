using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Admin.LeagueContacts
{
    [Table("RDN_Admin_League_Associations")]
    public class LeagueAssociation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeagueAssociationId { get; set; }
        [MaxLength(20)]
        public string Short { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
