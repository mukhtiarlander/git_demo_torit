using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Utilities.Error;


namespace RDN.Library.Classes.Error.Classes
{
    public class Error
    {
        public int ErrorId { get; set; }
        public DateTime Created { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyVersion { get; set; }
        public string NameSpace { get; set; }
        public string ErrorNameSpace { get; set; }
        public string MethodName { get; set; }
        public string AdditionalInformation { get; set; }
        public ErrorSeverityEnum? ErrorSeverity { get; set; }
        public ErrorGroupEnum? ErrorGroup { get; set; }

        public List<Detail> Exceptions { get; set; }
        public List<Data> Data { get; set; }

        public Error()
        {
            Exceptions = new List<Detail>();
            Data = new List<Data>();
        }
    }
}
