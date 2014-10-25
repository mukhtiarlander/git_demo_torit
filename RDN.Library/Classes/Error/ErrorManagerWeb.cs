using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using RDN.Utilities.Error;


namespace RDN.Library.Classes.Error
{
    public class ErrorManagerWeb : ErrorManager
    {
        public static ErrorObject GetErrorObject(Exception e, Type type, HttpContext context, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            if (context != null)
                return GenerateErrorObject(e, type, errorGroup, errorSeverity, GetSessionData(ref context), GetCookieData(ref context), GetUserData(ref context), GetServerVariablesData(ref context), parameters, additionalInformation);
            else
                return GenerateErrorObject(e, type, errorGroup, errorSeverity, null, null, null, null, parameters, additionalInformation);
        }
        /// <summary>
        /// Summarizes an exception into an object that can be sent to the error management system of rdnation
        /// </summary>
        /// <param name="e">The exception</param>
        /// <param name="type">The type information from the class where this occured. Can be obtained by using this.GetType() or just GetType()</param>
        /// <param name="stream">The object will be added to this stream.</param>
        /// <param name="errorGroup">The group where to put the error. Used for sorting on the error display webpage.</param>
        /// <param name="errorSeverity">The severity of the error. Used for sorting on the error display webpage.</param>
        /// <param name="parameters">Parameters or variables that whose vaules should be saved. Easiest is to get a list from this class, var v = ErrorManager.GetEmptyErrorList(); and then add the data like this: v.Add( () => myImportantVariable ); Variables, arrays, lists and dictionaries work.</param>
        /// <param name="additionalInformation">Additional information that could be useful.</param>        
        public static void GetErrorObjectStream(Exception e, Type type, out Stream stream, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
                GetErrorObjectStream(GenerateErrorObject(e, type, errorGroup, errorSeverity, GetSessionData(ref context), GetCookieData(ref context), GetUserData(ref context), GetServerVariablesData(ref context), parameters, additionalInformation), out stream);
            else
                GetErrorObjectStream(GenerateErrorObject(e, type, errorGroup, errorSeverity, null, null, null, null, parameters, additionalInformation), out stream);
        }

        private static Dictionary<string, string> GetSessionData(ref HttpContext context)
        {
            if (context.Session == null) return null;

            var output = new Dictionary<string, string>();
            var sessionKeys = context.Session.Keys;
            for (var i = 0; i < sessionKeys.Count; i++)
            {
                string sessionKey = string.Empty;
                string sessionValue = string.Empty;

                try
                {
                    sessionKey = sessionKeys[i];
                    sessionValue = context.Session[sessionKey].ToString();
                }
                catch (Exception)
                {
                    if (string.IsNullOrEmpty(sessionKey))
                        sessionKey = "Unknown (error)";
                    if (string.IsNullOrEmpty(sessionValue))
                        sessionValue = "Unknown (error)";
                }

                output.Add(sessionKey, sessionValue);
            }
            return output;
        }

        private static Dictionary<string, string> GetCookieData(ref HttpContext context)
        {
            var output = new Dictionary<string, string>();
            
            try
            {
                if (context.Request == null || context.Request.Cookies.Count == 0) return null;    
                var cookieKeys = context.Request.Cookies.Keys;
                for (var i = 0; i < cookieKeys.Count; i++)
                {
                    string cookieKey = string.Empty;
                    string cookieValue = string.Empty;

                    try
                    {
                        cookieKey = cookieKeys[i];
                        // Filter out some chrome cookies like  __qca=P0-1725770340-1330513871437; __unam=7639673-135bf56349f-17eadb1b-27; and also the glimpse information
                        if (cookieKey.StartsWith("__") || cookieKey.ToLower().Contains("glimpse")) continue;
                        cookieValue = context.Session[cookieKey].ToString();
                    }
                    catch (Exception)
                    {
                        if (string.IsNullOrEmpty(cookieKey))
                            cookieKey = "Unknown (error)";
                        if (string.IsNullOrEmpty(cookieValue))
                            cookieValue = "Unknown (error)";
                    }

                    if (!output.Keys.Contains(cookieKey))
                        output.Add(cookieKey, cookieValue);
                }
            }
            catch 
            {
                
            }
            return output;
        }


        private static Dictionary<string, string> GetUserData(ref HttpContext context)
        {
            if (context.User == null) return null;

            var output = new Dictionary<string, string>();
            output.Add("User identity", context.User.Identity.Name ?? string.Empty);
            output.Add("User authenticated", context.User.Identity.IsAuthenticated ? "True" : "False");
            return output;
        }

        private static Dictionary<string, string> GetServerVariablesData(ref HttpContext context)
        {
            var output = new Dictionary<string, string>();
            try
            {
                output.Add("HTTP_HOST", context.Request.ServerVariables["HTTP_HOST"] ?? string.Empty);
                output.Add("PATH_INFO", context.Request.ServerVariables["PATH_INFO"] ?? string.Empty);
                output.Add("REQUEST_METHOD", context.Request.ServerVariables["REQUEST_METHOD"] ?? string.Empty);
                output.Add("QUERY_STRING", context.Request.ServerVariables["QUERY_STRING"] ?? string.Empty);
                output.Add("REMOTE_ADDR", context.Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty);
                output.Add("ALL_RAW", context.Request.ServerVariables["ALL_RAW"] ?? string.Empty);

                output.Add("URL", context.Request.Url.ToString() ?? string.Empty);
                if (context.Request.UrlReferrer != null)
                    output.Add("URL_REFFERER", context.Request.UrlReferrer.ToString() ?? string.Empty);
            }
            catch 
            { }
            return output;
        }
    }
}
