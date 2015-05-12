using Common.EmailServer.Library.Database.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Admin
{
   
    public class AdminMessagesModel
    {
        public List<AdminEmailMessage> Items { get; set; }
        public string NewMessage { get; set; }
        
        
    }
}