using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;
using RDN.Library.DataModels.League.Documents;

namespace RDN.Library.DataModels.Tags
{
    [Table("RDN_Tags")]
    public class Tag : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TagId { get; set; }

        public string TagName { get; set; }
        
    }
}
