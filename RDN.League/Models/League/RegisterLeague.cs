using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Utilities;
using System.Web.Mvc;

namespace RDN.League.Models.League
{
    public class RegisterLeague
    {
        public RegisterLeague()
        {
            var zones = RDN.Library.Classes.Utilities.TimeZone.GetAllCommonTimeZones();
            TimeZones = new List<SelectListItem>();
            foreach (var zone in zones)
            {
                TimeZones.Add(new SelectListItem { Text = zone.Zone.ToString() +" "+ zone.TimeZoneLocation, Value = zone.Zone.ToString("N0") });
            }


        }

        public Guid LeagueId { get; set; }


        [Required(ErrorMessage = "**")]
        [Display(Name = "Name Of League: ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "Email For League: ")]
        [EmailValidation(ErrorMessage = "Not a valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "Phone Number For League: ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "Country: ")]
        public string Country { get; set; }

        [Display(Name = "State: (Alabama or Mississippi) if applicable")]
        public string State { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "City: ")]
        public string City { get; set; }

        [Display(Name = "Federation: ")]
        public string Federation { get; set; }

        [Required(ErrorMessage = "**")]
        [Display(Name = "Time Zone: ")]
        public double TimeZone { get; set; }

        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Federations { get; set; }
        public List<SelectListItem> TimeZones { get; set; }
        /// <summary>
        /// new league was created when set to true.
        /// </summary>
        public bool Created { get; set; }
        /// <summary>
        /// league was only updated when set to true.
        /// </summary>
        public bool Updated{ get; set; }
    }
}