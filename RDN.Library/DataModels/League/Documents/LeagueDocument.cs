using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League.Documents
{
    [Table("RDN_League_Documents")]
    public class LeagueDocument : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DocumentId { get; set; }

        public string Name { get; set; }

        public virtual Group.Group Group { get; set; }
        public virtual League League { get; set; }

        [Required]
        public virtual Document.Document Document { get; set; }

        public bool IsRemoved { get; set; }

        public bool IsArchived { get; set; }

        public virtual DocumentCategory Category { get; set; }
        public virtual List<DocumentComment> Comments { get; set; }
        public virtual ICollection<DocumentTag> DocumentTags { get; set; }

        public LeagueDocument()
        {
            Comments = new List<DocumentComment>();
            DocumentTags = new List<DocumentTag>();
        }

    }
}
