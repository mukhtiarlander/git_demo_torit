using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Games.Officiating
{
  public   class RequestsModel
    {
      public List<RequestDA> Requests { get; set; }
      public string UrlToRequest{ get; set; }
    }
}
