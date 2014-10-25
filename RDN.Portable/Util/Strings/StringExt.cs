using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RDN.Portable.Util.Strings
{
    public static class StringExt
    {
        /// <summary>
        /// Removes all HTML attributes in the text.
        /// </summary>
        /// <param name="text">HTML Text.</param>
        /// <returns>No HTML Text</returns>
        public static string HtmlDecode(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return text;
            Regex regex = new Regex("<[^>]*>");
            return regex.Replace(text, " ");
        }
    }
}
