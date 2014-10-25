using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace RDN.Library.Extension
{
    public static class ExceptionExt
    {
        public static XmlWriter GetXmlString(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");
            StringWriter sw = new StringWriter();
            XmlWriter xw = XmlWriter.Create(sw);
            using (xw)
            {
                WriteException(xw, "exception", exception);
            }
            return xw;
        }
        private static void WriteException(XmlWriter writer, string name, Exception exception)
        {
            if (exception == null) return;
            writer.WriteStartElement(name);
            writer.WriteElementString("message", exception.Message);
            writer.WriteElementString("source", exception.Source);
            WriteException(writer, "innerException", exception.InnerException);
            writer.WriteEndElement();
        }
    }
}
