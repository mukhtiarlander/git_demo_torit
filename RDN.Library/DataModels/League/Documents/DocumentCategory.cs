using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League.Documents
{
    /// <summary>
    /// instead of a category, this is more like a folder.
    /// </summary>
    [Table("RDN_League_Document_Categories")]
    public class DocumentCategory : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CategoryId { get; set; }

        public string CategoryName { get; set; }
        public bool IsRemoved { get; set; }

        public virtual DocumentCategory ParentFolder { get; set; }
        public virtual Group.Group Group { get; set; }
        public virtual League League { get; set; }
        public virtual List<DocumentCategory> SubFolders { get; set; }
        public DocumentCategory()
        {

        }

    }
}
