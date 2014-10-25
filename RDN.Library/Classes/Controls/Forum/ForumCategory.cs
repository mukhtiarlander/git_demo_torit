using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Forum
{
    public class ForumCategory
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long GroupId { get; set; }
        public int UnreadTopics { get; set; }
        public string  GroupName { get; set; }

        public ForumCategory()
        {
            
        }
        public ForumCategory(long categoryId)
        {
            CategoryId = categoryId;
        }

        public static ForumCategory DisplayCategory(DataModels.Forum.ForumCategories cat)
        {
            ForumCategory c = new ForumCategory();
            c.CategoryId = cat.CategoryId;
            c.CategoryName = cat.NameOfCategory;
            c.GroupId = cat.GroupId;
            return c;
        }

    }
}
