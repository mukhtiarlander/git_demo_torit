using System.Collections.Generic;
using RDN.Library.Classes.Account.Classes;
using RDN.League.Models.Utilities;
using RDN.Library.Util;

namespace RDN.League.Models.OutModel
{
    public abstract class Base
    {
        public List<string> Javascripts { get; set; } 
        public List<SiteMessage> MessageList { get; set; }        
        public MemberDisplayFactory MemberDisplay { get; set; }

        protected Base()
        {
            Javascripts = new List<string>();
            MessageList = new List<SiteMessage>();
        }
    }
}