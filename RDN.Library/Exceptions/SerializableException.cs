using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace RDN.Library.Exceptions
{
    [XmlRoot("exception"), XmlType("exception")]
    public class SerializableException
    {
        [XmlElement("message")]
        public string Message { get; set; }

        [XmlElement("innerException")]
        public SerializableException InnerException { get; set; }
    }
}
