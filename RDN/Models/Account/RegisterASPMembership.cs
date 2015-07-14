using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using RDN.Library.Classes.Account.Enums;
using System;
using RDN.Portable.Classes.Account.Enums;

namespace RDN.Models.Account
{
    public class RegisterModel
    {
        public List<string> Errors { get; set; }

        public Guid MemberId { get; set; }

        
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address: ")]
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// this email is used to help verify the email address of a user connecting to their profile
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address: ")]
        [MaxLength(255)]
        public string EmailVerify { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Again: ")]
        [System.Web.Mvc.Compare("Email", ErrorMessage = "The email address and confirmation email address do not match")]
        [MaxLength(255)]
        public string ConfirmEmail { get; set; }

        [StringLength(255, ErrorMessage = "The password must be at least 6 characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //[MaxLength(255)]
        //public string ConfirmPassword { get; set; }

        [Range(0, 2, ErrorMessage = "Must Select a Gender.")]
        [Display(Name = "I am: ")]
        public int Gender { get; set; }

        [Display(Name = "I am: ")]
        public GenderEnum GenderName { get; set; }

        [Range(1, 6, ErrorMessage = "Must Select a Position.")]
        [Display(Name = "I am: ")]
        public int PositionType { get; set; }

        //[Required]
        //[Display(Name = "User name")]
        //[StringLength(25, ErrorMessage = "The username must be at least 4 characters long.", MinimumLength = 4)]
        //public string UserName { get; set; }

        [StringLength(255, ErrorMessage = "The firstname must be at least 2 characters long.", MinimumLength = 2)]
        [Display(Name = "First Name: ")]
        public string Firstname { get; set; }

        /// <summary>
        /// we use this to verify the first name of the user...
        /// </summary>
        [StringLength(255, ErrorMessage = "The firstname must be at least 2 characters long.", MinimumLength = 2)]
        [Display(Name = "First Name: ")]
        public string FirstnameVerify { get; set; }

        [StringLength(255, ErrorMessage = "The derby name must be at least 2 characters long.", MinimumLength = 2)]
        [Display(Name = "Derby Name: ")]
        public string DerbyName { get; set; }


        [Display(Name = "Do You Derby?: ")]
        public bool IsConnectedToRollerDerby { get; set; }

    }
}