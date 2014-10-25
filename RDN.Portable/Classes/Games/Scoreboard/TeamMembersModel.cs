using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Games.Scoreboard
{
    [ProtoContract]
    [DataContract]
    public class TeamMembersModel
    {
        [ProtoMember(1)]
        [DataMember]
        public Guid SkaterLinkId { get; set; }

        public TeamMembersModel(Guid skaterLinkId)
        {
            this.SkaterLinkId = skaterLinkId;
        }
    }
}
