using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.RN.RSS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.DataModels.RN.Posts
{
    /// <summary>
    /// rss feeds of the RN persuasion to import into RN.
    /// </summary>
    [Table("RN_RSS_Feeds")]
    public class RSSFeed : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RssId { get; set; }

        public string NameOfSite { get; set; }
        public string UrlOfSite { get; set; }

        public string UrlOfRss { get; set; }
        public DateTime LastCheck { get; set; }
        public long TotalPostsImported { get; set; }
        public bool IsRemoved { get; set; }
        public bool CanNotScanFeed { get; set; }

        public Guid UserIdToAssignPostsTo { get; set; }

        public string InitialImageUrl { get; set; }
        public string MainImageUrl { get; set; }


        public virtual List<Post> Posts { get; set; }
        public virtual List<RSSFeedCategory> InitialCategories { get; set; }
        public virtual List<RSSFeedTag> InitialTags { get; set; }

        public RSSFeed()
        {
            Posts = new List<Post>();
            InitialCategories = new List<RSSFeedCategory>();
            InitialTags = new List<RSSFeedTag>();
        }



    }
}
