using RDN.Portable.Classes.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Models.Json.Games
{
    [DataContract]
    public class GamesJson : DataJson
    {
        [DataMember]
        public List<CurrentGameJson> Games { get; set; }

        [DataMember]
        public List<ManualGame> ManualGames{ get; set; }


        public GamesJson()
        {
            Games = new List<CurrentGameJson>();
        }
    }
}
