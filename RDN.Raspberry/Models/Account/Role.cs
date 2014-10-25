using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Account
{
    public class Role
    {
        public bool Added { get; set; }
        public string EmailAddress { get; set; }
        public string RoleForMember { get; set; }
    }
}