using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Payment.Classes
{

    [ProtoContract]
    [DataContract]
    public class CurrencyExchange
    {
        [ProtoMember(1)]
        [DataMember]
        public string CurrencyName { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string CurrencyAbbrName { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public decimal CurrencyExchangeRate { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public string CurrencyNameDisplay { get; set; }

    }
}
