using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Forum
{
    [DataContract]
    public class ForumGroupModel
    {
        [DataMember]
        public long GroupId { get; set; }
        [DataMember]
        public string GroupName { get; set; }
        [DataMember]
        public List<ForumTopicModel> Topics { get; set; }

        [DataMember]
        public int UnreadTopicsCount { get; set; }


        [DataMember]
        public List<ForumCategory> Categories { get; set; }


        public ForumGroupModel()
        {
            Topics = new List<ForumTopicModel>();
        }
    }
}
