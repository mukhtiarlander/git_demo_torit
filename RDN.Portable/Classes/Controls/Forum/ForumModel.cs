using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Controls.Forum
{
    [DataContract]
    public class ForumModel
    {
        [DataMember]
        public Guid ForumId { get; set; }
        [DataMember]
        public long GroupId { get; set; }
        [DataMember]
        public long CategoryId { get; set; }
        [DataMember]
        public List<ForumGroupModel> Groups { get; set; }
        
        public ForumModel()
        {
            Groups = new List<ForumGroupModel>();
        }
    }
}
