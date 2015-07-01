﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.DataModels.Forum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RDN.Library.DataModels.Forum
{
    [Table("RDN_Forum_Message_Mention")]
    public class ForumMessageMention
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MentionId { get; set; }
        public  ForumMessage Messages { get; set; }
        public Member.Member Member { get; set; }
    }
}