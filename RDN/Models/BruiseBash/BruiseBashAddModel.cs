using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RDN.Models.BruiseBash
{
    public class BruiseBashAddModel
    {
        [Display(Name = "Title")]
        public string Title { get; set; }
                           [Display(Name = "Story of Bruise")]
        public string Story { get; set; }
        [Display(Name = "Upload Bruise")]
        public string File { get; set; }
    }
}