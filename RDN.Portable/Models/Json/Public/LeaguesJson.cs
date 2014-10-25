using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json.Public
{
    [DataContract]
  public   class LeaguesJson : DataJson
    {
        [DataMember]
        public List<LeagueJsonDataTable> Leagues { get; set; }
        public LeaguesJson()
        {
            Leagues = new List<LeagueJsonDataTable>();
        }
    }
}
