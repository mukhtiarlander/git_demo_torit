using RDN.Library.DataModels.Admin.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Admin
{
   
    public class AdminMessagesModel
    {
      public   List<AdminEmailMessages> Items { get; set; }
        public string NewMessage { get; set; }
        
        
    }
}