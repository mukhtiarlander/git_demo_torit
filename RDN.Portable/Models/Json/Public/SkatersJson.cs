using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json.Public
{
    [DataContract]
    public class SkatersJson : DataJson
    {
        [DataMember]
        public List<SkaterJson> Skaters { get; set; }
        public SkatersJson()
        {
            Skaters = new List<SkaterJson>();
        }
    }
}
