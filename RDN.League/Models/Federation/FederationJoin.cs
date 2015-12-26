using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Models.Federation
{
    public class FederationJoin
    {

        public SelectList Federations { set; get; }

        [Required(ErrorMessage="Select a Federation to Join")]       
        public string SelectedFederation { get; set; }

        public RDN.Portable.Classes.League.Classes.League League { set; get; }

        public FederationJoin() {
            League = new Portable.Classes.League.Classes.League();
        }
     }
}