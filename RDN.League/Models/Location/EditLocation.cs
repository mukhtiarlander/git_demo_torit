using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Context;
using System.Web.Mvc;

namespace RDN.League.Models.Location
{
    public class EditLocation : RDN.Portable.Classes.Location.Location
    {
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string CityRaw { get; set; }
        public string StateRaw { get; set; }
        [Required]
        public string Zip { get; set; }
        [Required]
        public int Country { get; set; }
        public SelectList Countries { get; set; }
        public string Website { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerType { get; set; }
        public string RedirectTo { get; set; }
    }
}