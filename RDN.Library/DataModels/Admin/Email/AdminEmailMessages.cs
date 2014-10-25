using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Admin.Email
{
    [Table("RDN_Admin_Email_Messages")]
    public class AdminEmailMessages : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmailId { get; set; }

        public string Message { get; set; }

    }
}
