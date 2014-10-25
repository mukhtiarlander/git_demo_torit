using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.ContactCard
{
    [ProtoContract]
    [DataContract]
   public  class Im
    {
       [ProtoMember(1)]
       [DataMember]
       public string Name { get; set; }
    }
}
