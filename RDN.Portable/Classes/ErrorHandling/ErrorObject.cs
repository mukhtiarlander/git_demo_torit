using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace RDN.Portable.Error
{
    [ProtoContract]
    public sealed class ErrorObject
    {
        [ProtoMember(1)]
        public DateTime Created { get; set; }
        [ProtoMember(2)]
        public string AssemblyName { get; set; }
        [ProtoMember(3)]
        public string AssemblyVersion { get; set; }
        [ProtoMember(4)]
        public string NameSpace { get; set; }
        [ProtoMember(5)]
        public string ErrorNameSpace { get; set; }
        [ProtoMember(6)]
        public string MethodName { get; set; }
        [ProtoMember(7)]
        public string AdditionalInformation { get; set; }
        [ProtoMember(8)]
        public ErrorSeverityEnum? ErrorSeverity { get; set; }
        [ProtoMember(9)]
        public ErrorGroupEnum? ErrorGroup { get; set; }
        [ProtoMember(10)]
        public List<ErrorException> ErrorExceptions { get; set; }
        [ProtoMember(11)]
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
    [ProtoContract]
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
        [ProtoMember(1)]
        public string Key { get; set; }
        [ProtoMember(2)]
        public string Value { get; set; }
        [ProtoMember(3)]
        public ErrorDataType DataType { get; set; }

        public ErrorDataDetail()
        { }
    }

    [ProtoContract]
    public sealed class ErrorException
    {
        [ProtoMember(1)]
        public string MethodName { get; set; }
        [ProtoMember(2)]
        public string Message { get; set; }
        [ProtoMember(3)]
        public string StackTrace { get; set; }
        [ProtoMember(4)]
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
