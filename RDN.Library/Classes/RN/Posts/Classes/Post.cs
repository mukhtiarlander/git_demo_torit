using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.RN.Posts.Classes
{
    [DebuggerDisplay("[{this.Id @ this.AuthorUserId}]")]
    public class Post
    {
        public Guid Id { get; set; }
        public long TotalViews { get; set; }
        public long TotalMonthlyViews { get; set; }
        public Guid AuthorUserId { get; set; }
        public double PercentageOfTotalViews { get; set; }
        public bool DisabledAutoPosting { get; set; }

        public bool DisablePaymentsForPost { get; set; }
        public string FeedName { get; set; }
        public string FeedUrl { get; set; }
        public bool FromFeed { get; set; }
    }
}
