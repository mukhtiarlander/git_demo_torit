using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Utilities
{
    [Table("RDN_Sitemap")]
    public class Sitemap : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [MaxLength(750)]
        [Required]
        public string Url { get; set; }
        [Required]
        public int ChangeFrequency { get; set; }
    }
}
