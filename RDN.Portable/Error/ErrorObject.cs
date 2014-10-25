using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace RDN.Portable.Error
{
        [DataContract]
    public class ErrorObject
    {
        [DataMember]
        
        public DateTime Created { get; set; }
        [DataMember]
        public string AssemblyName { get; set; }
        [DataMember]
        public string AssemblyVersion { get; set; }
        [DataMember]
        public string NameSpace { get; set; }
        [DataMember]
        public string ErrorNameSpace { get; set; }
        [DataMember]
        public string MethodName { get; set; }
        [DataMember]
        public string AdditionalInformation { get; set; }
        [DataMember]
        public ErrorSeverityEnum? ErrorSeverity { get; set; }
        [DataMember]
        public ErrorGroupEnum? ErrorGroup { get; set; }
        [DataMember]
        public List<ErrorException> ErrorExceptions { get; set; }
        [DataMember]
        public List<ErrorDataDetail> ErrorData { get; set; }

        public ErrorObject()
        {
            ErrorData = new List<ErrorDataDetail>();
            ErrorExceptions = new List<ErrorException>();
        }



        internal ErrorObject(Exception e)
        {
            ErrorData = new List<ErrorDataDetail>();
            ErrorExceptions = new List<ErrorException>();

            int traceLevel = 0;
            StoreException(e, traceLevel);
        }

        public void StoreException(Exception e, int traceLevel)
        {
            string targetSiteName = null;
            //if (e.TargetSite != null)
            //    targetSiteName = e.TargetSite.Name;

            ErrorExceptions.Add(new ErrorException(targetSiteName, e.Message, e.StackTrace, traceLevel));

            traceLevel++;
            if (e.InnerException != null)
                StoreException(e.InnerException, traceLevel);
        }


    }
    
    [DataContract]
    public class ErrorDataDetail
    {
        public enum ErrorDataType
        {
            Parameter = 1,
            UserData = 2,
            Session = 3,
            Cookie = 4,
            ServerVariable = 5
        }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public ErrorDataType DataType { get; set; }

        public ErrorDataDetail()
        { }
    }
    [DataContract]
    public sealed class ErrorException
    {
        [DataMember]
        public string MethodName { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string StackTrace { get; set; }
        [DataMember]
        public int Depth { get; set; }

        public ErrorException()
        {
        }

        internal ErrorException(string methodName, string message, string stackTrace, int depth)
        {
            MethodName = methodName;
            Message = message;
            StackTrace = stackTrace;
            Depth = depth;
        }
    }
}
