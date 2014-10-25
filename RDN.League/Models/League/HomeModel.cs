using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RDN.Library.Classes.Forum;
using RDN.Library.Classes.Store.Display;
using RDN.Library.Classes.Controls.Voting;
using RDN.Portable.Classes.Voting;

namespace RDN.League.Models.League
{
    public class HomeModel
    {
        public RDN.Portable.Classes.League.Classes.League League { get; set; }
        public RDN.Portable.Classes.Controls.Calendar.Calendar Calendar { get; set; }
        public Guid ForumId { get; set; }
        public List<ForumTopicJson> Forum { get; set; }
        public DisplayStore Store { get; set; }
        public PollBase Polls { get; set; }
    }
}