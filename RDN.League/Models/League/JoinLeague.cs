using System.ComponentModel.DataAnnotations;
namespace RDN.League.Models.League
{
    public class JoinLeague
    {
        public string LeagueId { get; set; }
        public string LeagueName { get; set; }

        [Display(Name = "League Join Code: ")]
        public string JoinCode { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}