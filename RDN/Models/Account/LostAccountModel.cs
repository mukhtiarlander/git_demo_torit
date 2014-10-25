using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Models.Account
{
    /// <summary>
    /// A model for those users that lost their email or password.
    /// </summary>
    public class LostAccountModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool CanChangePassword { get; set; }

        public string ConfirmationMessage { get; set; }

        public string VerificationCode { get; set; }
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [MinLength(6)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm")]
        public string NewPasswordConfirm { get; set; }
    }
}