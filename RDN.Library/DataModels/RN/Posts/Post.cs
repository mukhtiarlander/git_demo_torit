using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.Library.DataModels.RN.Posts
{
    [Table("RN_Posts")]
    public class Post : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PostId { get; set; }

        /// <summary>
        /// total times been pushed to facebook automatically.
        /// </summary>
        public long TotalFacebookPosts { get; set; }
        public DateTime? LastTimePostedToFacebook { get; set; }

        public long CurrentMonthlyViews { get; set; }
        public long TotalViews { get; set; }
        public bool DisabledAutoPosting { get; set; }

        public bool DisablePaymentsForPost { get; set; }

        public virtual RSSFeed Feed { get; set; }
    }
}
