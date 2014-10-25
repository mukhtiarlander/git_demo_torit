using System.Collections.Generic;
using RDN.Raspberry.Models.Utilities;

namespace RDN.Raspberry.Models.OutModel
{
    public abstract class Base
    {
        public List<string> Javascripts { get; set; } 
        public List<SiteMessage> MessageList { get; set; }                

        protected Base()
        {
            Javascripts = new List<string>();
            MessageList = new List<SiteMessage>();
        }
    }
}