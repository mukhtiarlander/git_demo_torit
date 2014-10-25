using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RDN.Models.Account
{
    public class SignUpBeta
    {
        public bool SignedUp { get; set; }
        [Display(Name = "Email Address: ")]
        public string Email { get; set; }
    }
}