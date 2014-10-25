using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Classes;
using System.Collections.ObjectModel;
using RDN.Library.Classes.Controls.Forum;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.Forum
{
    public class ForumTopic
    {
        public ForumTopic()
        {
            Messages = new List<ForumMessage>();
            Category = new ForumCategory();
            TopicInbox = new List<ForumTopicInbox>();
            Watchers = new List<ForumTopicWatchers>();
        }
        public bool BroadcastForumTopic { get; set; }
        public string ForumName { get; set; }
        public string Url { get; set; }
        public ForumOwnerTypeEnum ForumType { get; set; }
        public Guid ForumId { get; set; }
        public DateTime Created { get; set; }
        public string CreatedHuman { get; set; }
        public string LastPostHuman { get; set; }
        public MemberDisplay CreatedByMember { get; set; }
        public MemberDisplay LastPostByMember { get; set; }
        public long TopicId { get; set; }
        public string TopicTitle { get; set; }
        public long GroupId { get; set; }
        public int ViewCount { get; set; }
        public int Replies { get; set; }
        public List<ForumMessage> Messages { get; set; }
        public List<ForumTopicInbox> TopicInbox { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedHuman { get; set; }
        public Guid CurrentMemberId { get; set; }
        public bool IsManagerOfTopic { get; set; }
        public ForumCategory Category { get; set; }
        public bool IsRead { get; set; }
        public bool IsLocked { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPinned { get; set; }
        public bool IsWatching { get; set; }
        /// <summary>
        /// those members watching the forum topic
        /// </summary>
        public List<ForumTopicWatchers> Watchers { get; set; }
       
    }
}
