using RDN.Library.Classes.Account.Classes;
using RDN.Library.Util;
using System;
using System.Collections.Generic;
using RDN.Library.Classes.Account.Classes;
using RDN.Store.Models.Utilities;
using RDN.Library.Util;
using RDN.Portable.Classes.Account.Classes;

namespace RDN.Store.Models.OutModel
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