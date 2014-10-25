using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.RN.Posts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.RN.RSS
{
    [Table("RN_RSS_Feed_Tags")]
    public class RSSFeedTag: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TagId { get; set; }

        public string TagName { get; set; }

        public virtual RSSFeed Feed { get; set; }
    }
}
