using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Classes;
using System.Collections.ObjectModel;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.Forum
{
    public class ForumTopicJson
    {
        public ForumTopicJson()
        {
        }
        public string ForumOwnerTypeEnum { get; set; }
        public Guid ForumId { get; set; }
        public DateTime Created { get; set; }
        public string CreatedHuman { get; set; }
        public string LastPostHuman { get; set; }
        public MemberDisplayBasic CreatedByMember { get; set; }
        public MemberDisplayBasic LastPostByMember { get; set; }
        public long TopicId { get; set; }
        public string TopicTitle { get; set; }
        public string TopicTitleForUrl { get; set; }
        public long GroupId { get; set; }
        public int ViewCount { get; set; }
        public int Replies { get; set; }
        public DateTime LastModified { get; set; }
        public Guid CurrentMemberId { get; set; }
        public bool IsManagerOfTopic { get; set; }
        public bool IsRead { get; set; }
        public bool IsLocked { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPinned { get; set; }
        public string  Category{ get; set; }
        public long CategoryId { get; set; }
        public string ForumGroup { get; set; }
    }
}
