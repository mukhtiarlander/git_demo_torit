using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Admin
{
    public class TestEmailLayout
    {
        public string Email { get; set; }
        public string LayoutId { get; set; }
        public string Xml { get; set; }        
        public string Error { get; set; }
        public bool Sent { get; set; }
    }
}