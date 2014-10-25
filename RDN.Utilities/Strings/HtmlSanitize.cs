using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RDN.Utilities.Strings
{
    /// <summary>
    /// Filters HTML to the valid html tags set (with only the attributes specified)
    /// 
    /// Thanks to http://refactormycode.com/codes/333-sanitize-html for the original
    /// </summary>
    public static class HtmlSanitize
    {
        private const string HTML_TAG_PATTERN = @"(?'tag_start'</?)(?'tag'\w+)((\s+(?'attr'(?'attr_name'\w+)(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+)))?)+\s*|\s*)(?'tag_end'/?>)";

        private static Regex carriageReplacement = new Regex(@"[\r\n]+", RegexOptions.Compiled);
        public static string ReplaceCarriageReturnsWithBreaks(string source)
        {
            return carriageReplacement.Replace(source, "<br />");
        }

        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        /// <summary>
        /// A dictionary of allowed tags and their respectived allowed attributes.  If no
        /// attributes are provided, all attributes will be stripped from the allowed tag
        /// </summary>
        public static Dictionary<string, List<string>> ValidHtmlTags = new Dictionary<string, List<string>> {
            { "p", new List<string>() },
            { "strong", new List<string>() }, 
            { "ul", new List<string>() }, 
            { "li", new List<string>() }, 
            { "a", new List<string> { "href", "target" } }
    };

        /// <summary>
        /// Extension filters your HTML to the whitelist specified in the ValidHtmlTags dictionary
        /// </summary>
        public static string FilterHtmlToWhitelist(this string text)
        {
            Regex htmlTagExpression = new Regex(HTML_TAG_PATTERN, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return htmlTagExpression.Replace(text, m =>
            {
                if (!ValidHtmlTags.ContainsKey(m.Groups["tag"].Value))
                    return String.Empty;

                StringBuilder generatedTag = new StringBuilder(m.Length);

                Group tagStart = m.Groups["tag_start"];
                Group tagEnd = m.Groups["tag_end"];
                Group tag = m.Groups["tag"];
                Group tagAttributes = m.Groups["attr"];

                generatedTag.Append(tagStart.Success ? tagStart.Value : "<");
                generatedTag.Append(tag.Value);

                foreach (Capture attr in tagAttributes.Captures)
                {
                    int indexOfEquals = attr.Value.IndexOf('=');

                    // don't proceed any futurer if there is no equal sign or just an equal sign
                    if (indexOfEquals < 1)
                        continue;

                    string attrName = attr.Value.Substring(0, indexOfEquals);

                    // check to see if the attribute name is allowed and write attribute if it is
                    if (ValidHtmlTags[tag.Value].Contains(attrName))
                    {
                        generatedTag.Append(' ');
                        generatedTag.Append(attr.Value);
                    }
                }

                generatedTag.Append(tagEnd.Success ? tagEnd.Value : ">");

                return generatedTag.ToString();
            });
        }
    }
}
