using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_League_News")]
    public class News : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NewsId { get; set; }
        [MaxLength(255)]
        public string Headline { get; set; }
        [Column(TypeName = "text")]
        public string Text { get; set; }        
        
        #region References

        [Required]
        public virtual League League { get; set; }

        #endregion
    }
}
