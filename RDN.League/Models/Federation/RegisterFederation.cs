using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Utilities;

namespace RDN.League.Models.Federation
{
    public class RegisterFederation
    {
        [Required(ErrorMessage = "**")]
        [Display(Name = "Name Of Federation: ")]
        public string FederationName { get; set; }
         
        [Required(ErrorMessage = "**")]
        [Display(Name = "Email For Federation: ")]
        [EmailValidation(ErrorMessage = "Not a valid Email Address")]
        public string FederationEmail { get; set; }

        [Required(ErrorMessage="**")]
        [Display(Name = "Phone Number For Federation: ")]
        public string FederationPhoneNumber { get; set; }

        public bool FederationCreated { get; set; }

        //[Required]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; set; }

        //[Display(Name = "Remember me?")]
        //public bool RememberMe { get; set; }

    }
}