using RDN.Utilities.Error;
using System;

namespace RDN.Library.Classes.Error
{
    public class Data
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public ErrorDataDetail.ErrorDataType DataType { get; set; }
    }
}
