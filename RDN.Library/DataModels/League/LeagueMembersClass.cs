using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_League_Members_Class")]
    public class LeagueMemberClass : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ClassId { get; set; }
        [MaxLength(255)]
        public string ClassName { get; set; }
         
        
    }
}
