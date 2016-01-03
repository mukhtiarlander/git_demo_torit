using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Document
{
    [Table("RDN_Documents")]
    public class Document : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DocumentId { get; set; }

        [Required]
        public string SaveLocation { get; set; }

        public int DocumentSize { get; set; }

        public string FullText { get; set; }
        public bool HasScannedText { get; set; }

        public bool IsDeleted { set; get; }   

         

        public Document()
        {

        }

    }
}
