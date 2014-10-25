using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RDN.League.Models.Game
{
    public class RegisterTournamentModel
    {
        [Required(ErrorMessage = "**")]
        [Display(Name = "Name Of Tournamnent: ")]
        public string TournamentName { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "Start of Tournament: ")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "End of Tournament: ")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "Passcode of Tournament: ")]
        public string Passcode { get; set; }


        
    }
}