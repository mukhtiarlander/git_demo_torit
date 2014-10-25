using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Team
{
    /// <summary>
    /// used to deserialize logos captured from RDNation.com
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class TeamLogo
    {
        public TeamLogo()
        { }

        [ProtoMember(1)]
        [DataMember]
        public Guid TeamLogoId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string TeamName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string ImageUrl { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string ImageUrlThumb { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string SaveLocation { get; set; }
        [ProtoMember(6)]
        [DataMember]
        public string Width { get; set; }
        [ProtoMember(7)]
        [DataMember]
        public string Height { get; set; }
        /// <summary>
        /// if this logo was just uploaded via a saved picture on the persons computer,
        /// we need to upload it to the server, so this will be set to true to make sure its uploaded.
        /// </summary>
        [ProtoMember(8)]
        [DataMember]
        public bool NewlyUploaded { get; set; }
    }
}
