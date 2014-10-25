using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.RN
{
    public class RSSFeedItem
    {
        public string NameOfOrganization { get; set; }
        public string MainUrl { get; set; }
        public string RSSUrl { get; set; }
        public long FeedId { get; set; }
        public DateTime LastChecked { get; set; }
        public long TotalPostsPulled { get; set; }
        public DateTime Created { get; set; }
        public bool IsChecked { get; set; }
        public string InitialImageUrl { get; set; }
        public string MainImageUrl { get; set; }
        public bool CanNotScanFeed { get; set; }
        public Guid AuthorUserId{ get; set; }
        public string AuthorUserName { get; set; }

        public IEnumerable<RSSFeedCategory> Categories { get; set; }
        public IEnumerable<RSSFeedTag> Tags { get; set; }

        public RSSFeedItem()
        {
            Categories = new List<RSSFeedCategory>();
            Tags = new List<RSSFeedTag>();
        }
    }
}
