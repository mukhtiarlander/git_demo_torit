using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RDN.League.Models.User
{
    public class VerifyEmail
    {
        [Required]
        [DisplayName("Verification code")]
        public string EmailVerificationCode { get; set; }
        [Required]
        public string Email { get; set; }
    }
}