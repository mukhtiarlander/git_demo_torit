using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Admin.LeagueContacts
{
    [Table("RDN_Admin_League_Types")]
    public class LeagueType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeagueTypeId { get; set; }
        [MaxLength(20)]
        public string Short { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
