using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RDN.League.Models.Forum
{
    public class MovePost
    {
        public string ForumType { get; set; }
        public Guid ForumId { get; set; }
        public string TopicTitle { get; set; }
        public long TopicId { get; set; }
        public long GroupId { get; set; }
         public bool IsManagerOrBetter { get; set; }
        public long MessageId { get; set; }
        public SelectList ForumCategories { get; set; }
        public long ChosenCategory { get; set; }
        public HttpPostedFileBase File { get; set; }
        [Display(Name = "Subject: ")]
        public string Subject { get; set; }

        public long ChosenForum { get; set; }
        public SelectList MoveToForums { get; set; }
        

    }
}