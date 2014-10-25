using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Location
{
    [Table("RDN_Location_Countries")]
    public class Country
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string Code { get; set; }
    }
}
