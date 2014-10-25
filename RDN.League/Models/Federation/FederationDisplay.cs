using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Federation.Enums;
using System.Web.Mvc;

namespace RDN.League.Models.Federation
{
    /// <summary>
    /// displays the federation out to the user.
    /// </summary>
  public   class FederationDisplay
    {
      public Guid FederationId { get; set; }
      public string FederationName { get; set; }
      public string MADEClassRank { get; set; }
      public string MemberType { get; set; }
      public string OwnerType { get; set; }
      [AllowHtml]
      public string FederationComments { get; set; }
    }
}
