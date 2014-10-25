using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Forum
{
    [DataContract]
    public class ForumCategory
    {
        [DataMember]
        public long CategoryId { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
        [DataMember]
        public long GroupId { get; set; }
        [DataMember]
        public int UnreadTopics { get; set; }
        [DataMember]
        public string GroupName { get; set; }

        public ForumCategory()
        {

        }
        public ForumCategory(long categoryId)
        {
            CategoryId = categoryId;
        }

        public static ForumCategory DisplayCategory(long categoryId, string categoryName, long groupId)
        {
            ForumCategory c = new ForumCategory();
            c.CategoryId = categoryId;
            c.CategoryName = categoryName;
            c.GroupId = groupId;
            return c;
        }

    }
}
