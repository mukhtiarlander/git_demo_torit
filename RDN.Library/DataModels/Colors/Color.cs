using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Color
{
    [Table("RDN_Colors")]
    public class Color : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ColorId { get; set; }
        
        [Required]
        public string ColorName { get; set; }
        [Required]
        public int ColorIdCSharp{get;set;}
        
        public Guid OwnerGuid { get; set; }

        public Color()
        {
            
        }

    }
}
