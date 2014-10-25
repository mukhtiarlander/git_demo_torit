using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Portable.Classes.Forum.Enums;
using RDN.Portable.Classes.Controls.Forum;
using RDN.Portable.Classes.League.Classes;

namespace RDN.Library.Classes.Forum
{
    public class ForumSettings
    {
        public string ForumName { get; set; }
        public string GroupName { get; set; }
        public ForumOwnerTypeEnum ForumType { get; set; }
        public Guid ForumId { get; set; }
        public long GroupId { get; set; }
        public bool BroadCastPostsDefault { get; set; }
        public List<LeagueGroup> Groups { get; set; }
        public List<ForumCategory> Categories { get; set; }
    }
}
