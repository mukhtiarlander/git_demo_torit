using System;
namespace RDN.Library.Classes.Error.Classes
{
    [Obsolete("User RDN.Core.*")]
    public class Detail
    {
        public string MethodName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int Depth { get; set; }
    }
}
