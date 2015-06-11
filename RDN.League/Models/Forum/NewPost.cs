using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RDN.Library.Classes.Forum;
using RDN.Portable.Classes.Forum.Enums;

namespace RDN.League.Models.Forum
{
    public class NewPost
    {
        public NewPost()
        {
            Messages = new List<ForumMessage>();
        }
        public List<ForumMessage> Messages { get; set; }
        public ForumOwnerTypeEnum ForumType { get; set; }
        public Guid ForumId { get; set; }
        public string TopicTitle { get; set; }
        public long TopicId { get; set; }
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public bool BroadcastMessage { get; set; }
        public bool PinMessage { get; set; }
        public bool LockMessage { get; set; }
        public bool IsManagerOrBetter { get; set; }
        public long MessageId { get; set; }
        public SelectList ForumCategories { get; set; }
        public long ChosenCategory { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string Subject { get; set; }
        public string Mentions { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Message: ")]
        public string Message { get; set; }
        public string MessageMarkDown { get; set; }
    }

}