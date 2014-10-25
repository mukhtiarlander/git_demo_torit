using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Forum
{
    public class ForumGroup
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public int UnreadTopics { get; set; }
        public List<ForumTopic> Topics { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int NumberOfRecords { get; set; }
        public int NumberOfPages { get; set; }
        /// <summary>
        /// braodcase forum message to everyone
        /// </summary>
        public bool BroadcastToEveryone { get; set; }

        public ForumGroup()
        {
            Topics = new List<ForumTopic>();
        }
    }
}
