using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.League.Documents
{
    [Table("RDN_League_Document_Comments")]
    public class DocumentComment : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CommentId { get; set; }

        public Member.Member Commentor { get; set; }
        public string Comment { get; set; }
        public bool IsRemoved { get; set; }

        public virtual LeagueDocument LeagueDocument { get; set; }

        public DocumentComment()
        {

        }

    }
}
