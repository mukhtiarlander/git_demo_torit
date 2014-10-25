using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.Raspberry.Models.Utilities
{
    public static class StringExtensionMethods
    {
        public static string ToMvcDisplayRegardlessOfNull(this string input, string defaultValueIfNull = null)
        {
            if (defaultValueIfNull == null)
                return input ?? string.Empty;
            else
                return input ?? defaultValueIfNull;
        }
    }
}