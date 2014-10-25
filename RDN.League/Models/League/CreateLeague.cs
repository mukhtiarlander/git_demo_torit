using System.Collections.Generic;
using System.Web.Mvc;

namespace RDN.League.Models.League
{
    public class CreateLeague
    {
        public CreateLeague() { }
        public CreateLeague(string leagueName, string contactEmail, string contactPhone, string city, string state, string country)
        {
            this.LeagueName = leagueName;
            this.ContactEmail = contactEmail;
            this.ContactPhone = contactPhone;
            this.City = city;
            this.State = state;
            this.Country = country;
        }


        public string LeagueName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string AdditionalInformation { get; set; }
        public string Federation { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Federations { get; set; }
    }
}