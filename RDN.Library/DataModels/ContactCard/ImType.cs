using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.ContactCard
{
    [Table("RDN_ContactCard_Im_Types")]
    public class ImType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImTypeId { get; set; }
        public string Type { get; set; }
    }
}
