using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RDN.League.Models.League
{
    public class CreateAccount
    {           
        [Display(Name = "League")]        
        public string League { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm email address")]
        [System.Web.Mvc.Compare("Email", ErrorMessage = "The email address and confirmation email address do not match")]
        [MaxLength(255)]
        public string ConfirmEmail { get; set; }

        [Required]
        [Display(Name = "User name")]
        [StringLength(25, ErrorMessage = "The username must be at least 4 characters long.", MinimumLength = 4)]
        public string UserName { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The firstname must be at least 2 characters long.", MinimumLength = 2)]
        [DataType(DataType.Password)]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "The surname must be at least 2 characters long.", MinimumLength = 2)]
        [DataType(DataType.Password)]
        [Display(Name = "Surname")]
        public string Surname { get; set; }
    }
}