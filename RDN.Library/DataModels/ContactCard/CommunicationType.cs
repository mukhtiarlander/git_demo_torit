using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.ContactCard
{
    [Table("RDN_ContactCard_Communication_Types")]
    public class CommunicationType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommunicationTypeId { get; set; }
        [MaxLength(255)]
        public string Type { get; set; }
    }
}
