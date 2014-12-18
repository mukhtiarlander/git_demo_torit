using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RDN.Utilities.Strings
{
    public static class StringExt
    {
        public static Regex NumberRegex = new Regex(@"\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static bool IsNumber(string numberString)
        {
            return NumberRegex.IsMatch(numberString);
        }

        public static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;
            if (bytes > 1024)
                for (i = 0; (bytes / 1024) > 0; i++, bytes /= 1024)
                    dblSByte = bytes / 1024.0;

            return String.Format("{0:0.#} {1}", dblSByte, Suffix[i]);

        }
        public static string FormatKiloBytes(long bytes)
        {


            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            return String.Format("{0:0.#} kb", (bytes / 1024f)); ;

        }
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static string ConvertTextAreaToHtml(this string text)
        {
            if (!String.IsNullOrEmpty(text))
                return text.Replace(System.Environment.NewLine, "<br/>");
            return text;
        }

        /// <summary>
        /// gets the inititals of a name.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Initials(this String str)
        {
            return String.Join("", str.Split(' ').Select(x => x[0].ToString()).ToArray());
        }
        /// <summary>
        /// removes the crap from strings to make them url friendly.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSearchEngineFriendly(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                text = ToUrlFriendly(text);
                if (text.Length > 100)
                    text = text.Remove(100);
                return text;
            } return string.Empty;
        }
        public static string ToUrlFriendly(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return text.Replace("%", "").Replace(">", "").Replace("<", "").Replace("\"", "").Replace("$", "").Replace(")", "").Replace("(", "").Replace("=", "").Replace("&", "").Replace("*", "").Replace("`", "").Replace("!", "").Replace(".", " ").Replace(":", " ").Replace("?", "").Replace("'", "").Replace("/", " ").Replace(@"\", " ").Replace(",", " ").Replace("+", " ").Trim().Replace("#", "").Replace("´", "").Replace(" ", "-").Replace("--", "-");
            } return string.Empty;
        }
        public static string ToAXDFriendly(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return text.Replace("%", "").Replace(">", "").Replace("<", "").Replace("$", "").Replace(")", "").Replace("(", "").Replace("=", "").Replace("&", "").Replace("*", "").Replace("`", "").Replace("!", "").Replace(":", " ").Replace("?", "").Replace("'", "").Replace("/", " ").Replace(@"\", " ").Replace(",", " ").Trim().Replace("#", "").Replace("´", "").Replace("--", "-");
            } return string.Empty;
        }
        public static string ToInternetFriendly(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return text.Replace("%", "").Replace(">", "").Replace("<", "").Replace("$", "").Replace(")", "").Replace("(", "").Replace("=", "").Replace("&", "").Replace("*", "").Replace("`", "").Replace("!", "").Replace(".", " ").Replace(":", " ").Replace("?", "").Replace("'", "").Replace("/", " ").Replace(@"\", " ").Replace(",", " ").Trim().Replace("#", "").Replace("´", "").Replace("--", "-");
            } return string.Empty;
        }
        public static string ToJavascriptFriendly(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return text.Replace("'", " ");
            } return string.Empty;
        }

        public static string ToExcelFriendly(this string text)
        {
            if (text != null)
            {
                text = text.Replace(">", "").Replace("<", "").Replace("$", "").Replace(")", "").Replace("(", "").Replace("=", "").Replace("&", "").Replace("*", "").Replace("`", "").Replace("!", "").Replace(".", " ").Replace(":", " ").Replace("?", "").Replace("'", "").Replace("/", " ").Replace("\"", " ").Replace(",", " ").Trim().Replace("#", "").Replace(" ", "-").Replace("´", "").Replace("--", "-").Replace("-", " ");
                if (text.Length > 100)
                    text = text.Remove(100);
                return text;
            } return string.Empty;
        }
        /// <summary>
        /// removes file names larger than 100 characters.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToFileNameFriendly(this string text)
        {
            if (text != null)
            {
                text = text.Replace("%", "").Replace(",", "_").Replace(":", "_").Replace("'", " ").Replace("–", "-");
                if (text.Length > 100)
                    text = text.Remove(100);
                return text;
            } return string.Empty;
        }
        /// <summary>
        /// keeps file name same size.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToFileNameFriendlySize(this string text)
        {
            if (text != null)
            {
                text = text.Replace("%", "").Replace(",", "-").Replace(":", "-").Replace("'", " ").Replace("–", "-").Replace(" ", "-");
                return text;
            } return string.Empty;
        }
        /// <summary>
        /// removes the crap from strings to make them url friendly.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToSearchFriendly(this string text)
        {
            if (text != null)
            {
                text = text.Replace("%", "").Replace("$", "").Replace(")", "").Replace("(", "").Replace("=", "").Replace("&", "").Replace("*", "").Replace("`", "").Replace("!", "").Replace(".", " ").Replace("?", "").Replace(":", " ").Replace("'", "").Replace("/", " ").Replace("\"", " ").Replace(",", " ").Replace("´", "").Replace("--", "-").Trim();
                if (text.Length > 100)
                    text = text.Remove(100);
                return text;
            }
            return string.Empty;
        }

    }
}
