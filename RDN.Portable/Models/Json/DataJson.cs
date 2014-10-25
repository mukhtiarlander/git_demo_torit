using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json
{
    [DataContract]
  public   class DataJson
    {
         [DataMember]
      public string StartsWith { get; set; }
         [DataMember]
      public int Page { get; set; }
         [DataMember]
      public int Count { get; set; }
      
    }
}
