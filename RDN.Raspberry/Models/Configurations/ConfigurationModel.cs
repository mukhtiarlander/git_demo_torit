using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Configurations
{
    public class ConfigurationModel
    {
        public List<Common.Site.Classes.Configations.SiteConfiguration> Items { get; set; }

        [StringLength(50), Required]
        public string Key { get; set; }
        [StringLength(400), Required]
        public string Value { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Discription { set; get; }
    }
}