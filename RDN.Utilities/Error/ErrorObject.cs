using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RDN.Utilities.Error
{
    [Serializable]
    public sealed class ErrorObject
    {
        public DateTime Created { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyVersion { get; set; }
        public string NameSpace { get; set; }
        public string ErrorNameSpace { get; set; }
        public string MethodName { get; set; }
        public string AdditionalInformation { get; set; }
        public ErrorSeverityEnum? ErrorSeverity { get; set; }
        public ErrorGroupEnum? ErrorGroup { get; set; }

        public List<ErrorException> ErrorExceptions { get; set; }
        public List<ErrorDataDetail> ErrorData { get; set; }

        internal ErrorObject()
        {
            ErrorData = new List<ErrorDataDetail>();
            ErrorExceptions = new List<ErrorException>();
        }

        internal ErrorObject(Exception e)
        {
            ErrorData = new List<ErrorDataDetail>();
            ErrorExceptions = new List<ErrorException>();

            int traceLevel = 0;
            StoreException(e,  traceLevel);
        }
              
        public void StoreException(Exception e,  int traceLevel)
        {
            string targetSiteName = null;
            if (e.TargetSite != null)
                targetSiteName = e.TargetSite.Name;

            ErrorExceptions.Add(new ErrorException(targetSiteName, e.Message, e.StackTrace, traceLevel));

            traceLevel++;
            if (e.InnerException != null)
                StoreException(e.InnerException,  traceLevel);
        }

       
    }
    [Serializable]
    public sealed class ErrorDataDetail
    {
        public enum ErrorDataType
        {
            Parameter = 1,
            UserData = 2,
            Session = 3,
            Cookie = 4,
            ServerVariable = 5
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public ErrorDataType DataType { get; set; }
    }

    [Serializable]
    public sealed class ErrorException
    {
        public string MethodName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int Depth { get; set; }

        internal ErrorException()
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
