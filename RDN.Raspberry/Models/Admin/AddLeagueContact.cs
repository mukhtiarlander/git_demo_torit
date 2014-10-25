using System.Collections.Generic;
using System.Web.Mvc;

namespace RDN.Raspberry.Models.Admin
{
    public class AddLeagueContact
    {
        public string CountryId { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public string LeagueTypeId { get; set; }
        public List<SelectListItem> LeagueTypes { get; set; }
        public string AssociationId { get; set; }
        public List<SelectListItem> Associations { get; set; }
        public string State { get; set;}
        public string City { get; set; }
        public string Name { get; set; }
        public string HomePage { get; set; }
        public string Facebook { get; set; }
        public string Comments { get; set; }
        public string PrimaryEmails { get; set; }
        public List<string> PrimaryEmailsList { get; set; }
        public string Emails { get; set; }
        public List<string> EmailsList { get; set; }     
   
        public AddLeagueContact()
        {
            PrimaryEmailsList = new List<string>();
            EmailsList = new List<string>();
        }
    }
}