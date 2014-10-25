using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.League.Classes
{
    [ProtoContract]
    [DataContract]
  public   class LeagueGroupModel:LeagueGroupBasic
    {
        [ProtoMember(101)]
        [DataMember]
        public bool IsChecked{ get; set; }
    }
}
