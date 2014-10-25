using System.Collections.Generic;
using RDN.Library.Classes.Account.Classes;
using RDN.Models.Utilities;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Models.OutModel
{
    public abstract class Base
    {
        public List<string> Javascripts { get; set; } 
        public List<SiteMessage> MessageList { get; set; }        
        public MemberDisplay MemberDisplay { get; set; }

        protected Base()
        {
            Javascripts = new List<string>();
            MessageList = new List<SiteMessage>();
        }
    }
}