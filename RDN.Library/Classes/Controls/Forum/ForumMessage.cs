using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Library.Classes.Forum
{
    public class ForumMessage
    {
        public long TopicId { get; set; }
        public Guid ForumId { get; set; }
        public DateTime Created { get; set; }
        public string CreatedHumanRelative { get; set; }
        public DateTime LastModified { get; set; }
        public MemberDisplay Member { get; set; }
        public string MessageHTML { get; set; }
        public string MessageMarkDown { get; set; }
        public long MessageId { get; set; }
        public string MessagePlain { get; set; }
        /// <summary>
        /// Get Total Message Liked
        /// </summary>
        public long MessageLikeCount { get; set; } //Get Total like 
        /// <summary>
        /// Get Tolal Message Agreed
        /// </summary>
        public long MessageAgreeCount { get; set; } //Get Total Agree
        

    }
}
