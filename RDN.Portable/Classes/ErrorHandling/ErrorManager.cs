using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Error
{

    public class ErrorManager
    {
        internal static readonly int ErrorVersion = 1;

        public static List<Expression<Func<object>>> GetEmptyErrorList(params Expression<Func<object>>[] input)
        {
            return new List<Expression<Func<object>>>(input);
        }



        #region Get Error object

        /// <summary>
        /// Summarizes an exception into an object that can be sent to the error management system of 
        /// </summary>
        /// <param name="e">The exception</param>
        /// <param name="type">The type information from the class where this occured. Can be obtained by using this.GetType() or just GetType()</param>
        /// <param name="errorGroup">The group where to put the error. Used for sorting on the error display webpage.</param>
        /// <param name="errorSeverity">The severity of the error. Used for sorting on the error display webpage.</param>
        /// <param name="parameters">Parameters or variables that whose vaules should be saved. Easiest is to get a list from this class, var v = ErrorManager.GetEmptyErrorList(); and then add the data like this: v.Add( () => myImportantVariable ); Variables, arrays, lists and dictionaries work.</param>
        /// <param name="additionalInformation">Additional information that could be useful.</param>
        /// <returns>A manageable object</returns>
        public static ErrorObject GetErrorObject(Exception e, Type type, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            return GenerateErrorObject(e, type, errorGroup, errorSeverity, null, null, null, null, parameters, additionalInformation);
        }

        #endregion

        #region Get Error object in a stream

        /// <summary>
        /// Summarizes an exception into an object that can be sent to the error management system of 
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
            GetErrorObjectStream(GenerateErrorObject(e, type, errorGroup, errorSeverity, null, null, null, null, parameters, additionalInformation), out stream);
        }

        /// <summary>
        /// Summarizes an exception object into the specified stream
        /// </summary>
        /// <param name="errorObject">The error object</param>
        /// <param name="stream">The object will be added to this stream.</param>        
        public static void GetErrorObjectStream(ErrorObject errorObject, out Stream stream)
        {
            try
            {
                stream = new MemoryStream();
                Serializer.Serialize(stream, errorObject);
            }
            catch
            {
                stream = null;
            }
        }

        #endregion

        #region Save error object to disk

        /// <summary>
        /// Summarizes an exception into an object that can be sent to the error management system of 
        /// </summary>
        /// <param name="e">The exception</param>
        /// <param name="type">The type information from the class where this occured. Can be obtained by using this.GetType() or just GetType()</param>
        /// <param name="path">Path to the final destination. Remember to add a fully qualified filename. Example: c:\myfolder\myfile.data</param>
        /// <param name="errorGroup">The group where to put the error. Used for sorting on the error display webpage.</param>
        /// <param name="errorSeverity">The severity of the error. Used for sorting on the error display webpage.</param>
        /// <param name="parameters">Parameters or variables that whose vaules should be saved. Easiest is to get a list from this class, var v = ErrorManager.GetEmptyErrorList(); and then add the data like this: v.Add( () => myImportantVariable ); Variables, arrays, lists and dictionaries work.</param>
        /// <param name="additionalInformation">Additional information that could be useful.</param>        
        public static Stream SaveErrorObject(Exception e, Type type, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            return SaveErrorObject(GenerateErrorObject(e, type, errorGroup, errorSeverity, null, null, null, null, parameters, additionalInformation));
        }

        protected static Stream SaveErrorObject(ErrorObject errorObject)
        {
            Stream stream = new MemoryStream();
            Serializer.Serialize(stream, errorObject);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }



        #endregion

        #region Load error object


        ///// <summary>
        ///// Loads a file into a stream containing the error object
        ///// </summary>
        ///// <param name="stream">Stream to add the error object to.</param>
        ///// <param name="path">Path to the error object</param>
        //public static void LoadErrorObject(out Stream stream, string path)
        //{
        //    // Get the file into a new stream
        //    stream = new FileStream(path, FileMode.Open, FileAccess.Read);

        //}

        /// <summary>
        /// Loads an error object from a stream into an object. Closes the stream afterwards.
        /// </summary>
        /// <param name="stream">The stream containing the error object</param>
        /// <returns>The error object</returns>
        public static ErrorObject LoadErrorObjectMobile(ref Stream stream)
        {
            stream.Position = 0;
            ErrorObject output = Serializer.Deserialize<ErrorObject>(stream);
            stream.Dispose();
            return output;
        }

        #endregion

        #region View plaintext, debug
        private static void ViewObjectDebug(Exception e, Type type, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, Dictionary<string, string> sessionData = null, Dictionary<string, string> userData = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            //CultureInfo info = new CultureInfo("en");
            //Thread.CurrentThread.CurrentCulture = info;
            //Thread.CurrentThread.CurrentUICulture = info;

            //// Gets the assembly information
            //var assembly = type.Assembly;
            //var assemblyInfo = assembly.GetName();
            //WriteData.WriteLine(" - Error handling - ");
            //WriteData.WriteLine("Occured", DateTime.UtcNow.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
            //WriteData.WriteLine("Assembly name", assemblyInfo.Name);
            //WriteData.WriteLine("Version", assemblyInfo.Version.ToString());
            //WriteData.WriteLine("Group",ErrorGroupEnum.HasValue ? errorGroup.Value.ToString() : string.Empty);
            //WriteData.WriteLine("Severity", errorSeverity.HasValue ? errorSeverity.Value.ToString() : string.Empty);

            //WriteData.WriteEmptyLine();

            //// WriteLine namespace and method name
            //WriteData.WriteLine("Namespace", type.Namespace);
            //WriteData.WriteLine("Error namespace", e.Source);
            //if (type.Name.IndexOf('`') > 0)
            //    WriteData.WriteLine("Method", type.Name.Substring(0, type.Name.IndexOf('`')));
            //else
            //    WriteData.WriteLine("Method", type.Name);

            //WriteData.WriteEmptyLine();

            //// Parameters and variables
            //WriteData.WriteLine(" - Parameters/Variables - ");
            //if (parameters != null && parameters.Any())
            //{
            //    foreach (var parameter in parameters)
            //    {
            //        // Get the parameter name
            //        string parameterName;
            //        var expression = parameter.Body as UnaryExpression;
            //        if (expression != null)
            //        {
            //            var expression2 = (MemberExpression)expression.Operand;
            //            parameterName = expression2.Member.Name;
            //        }
            //        else
            //        {
            //            var expression2 = (MemberExpression)parameter.Body;
            //            parameterName = expression2.Member.Name;
            //        }

            //        // Get the parameter value by compiling it and retrieving the value
            //        var parameterValue = parameter.Compile().Invoke().ToString();

            //        WriteData.WriteLine(parameterName, parameterValue);
            //    }
            //}

            //WriteData.WriteEmptyLine();

            //// Trace
            //int traceLevel = 1;
            //WriteInnerExceptions(e, ref traceLevel);

            //WriteData.WriteEmptyLine();

            //// Session and user data
            //WriteData.WriteLine("- Sessions -");
            //foreach (var sessionInfo in sessionData)
            //    WriteData.WriteLine(sessionInfo.Key, sessionInfo.Value);

            //WriteData.WriteEmptyLine();
            //WriteData.WriteLine("- User -");
            //foreach (var userInfo in userData)
            //    WriteData.WriteLine(userInfo.Key, userInfo.Value);

            //// Additional information
            //WriteData.WriteLine("Additional information", additionalInformation);
        }
        #endregion

        protected static ErrorObject GenerateErrorObject(Exception e, Type type = null, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IDictionary<string, string> sessionData = null, IDictionary<string, string> cookieData = null, IDictionary<string, string> userData = null, IDictionary<string, string> serverVariablesData = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            var output = new ErrorObject(e)
            {
                ErrorGroup = errorGroup,
                ErrorSeverity = errorSeverity,
                AdditionalInformation = additionalInformation
            };

            #region Parameter data
            // Do we have any parameter or variable ?
            if (parameters != null && parameters.Count > 0)
            {
                var parameterValue = new StringBuilder();
                foreach (var parameter in parameters)
                {
                    parameterValue.Clear(); // Clear old values
                    var detail = new ErrorDataDetail { Key = string.Empty, Value = string.Empty, DataType = ErrorDataDetail.ErrorDataType.Parameter };

                    // Get the name and value seperatly in case one of them fails.
                    try
                    {
                        // Get the parameter name                        
                        var expression = parameter.Body as UnaryExpression;
                        if (expression != null)
                        {
                            var expression2 = (MemberExpression)expression.Operand;
                            detail.Key = expression2.Member.Name;
                        }
                        else
                        {
                            var expression2 = (MemberExpression)parameter.Body;
                            detail.Key = expression2.Member.Name;
                        }
                    }
                    catch (Exception)
                    {
                        detail.Key = "Unknown (error)";
                    }

                    try
                    {
                        // Get the parameter value by compiling it and retrieving the value
                        var invokedData = parameter.Compile().Invoke();

                        // Check if its a dictionary
                        var collection = invokedData as IDictionary;
                        if (collection != null)
                        {
                            // The keys in the collection
                            var keys = collection.Keys;
                            // Loop through the keys and write key/value
                            foreach (var key in keys)
                                parameterValue.Append(string.Format("{0}={1}, ", key, collection[key]));
                            // If we have added something to the value, then remove the ,{space} at the end of the string
                            detail.Value = parameterValue.Length > 0 ? parameterValue.ToString().Substring(0, parameterValue.Length - 2) : parameterValue.ToString();
                            output.ErrorData.Add(detail);
                            continue;
                        }

                        // Check if its a list
                        var list = invokedData as IList;
                        if (list != null)
                        {
                            // Loop through the data and write it
                            foreach (var colData in list)
                                parameterValue.Append(string.Format("{0}, ", colData));
                            // If we have added something to the value, then remove the ,{space} at the end of the string
                            detail.Value = parameterValue.Length > 0 ? parameterValue.ToString().Substring(0, parameterValue.Length - 2) : parameterValue.ToString();
                            output.ErrorData.Add(detail);
                            continue;
                        }

                        if (invokedData == null || String.IsNullOrEmpty(invokedData.ToString()))
                        {
                            // Save the value
                            detail.Value = string.Empty;
                            output.ErrorData.Add(detail);
                        }
                        else
                        {
                            // Save the value
                            detail.Value = invokedData.ToString();
                            output.ErrorData.Add(detail);
                        }
                    }
                    catch (Exception)
                    {
                        detail.Value = "Unknown (error)";
                        output.ErrorData.Add(detail);
                    }
                }
            }
            #endregion

            if (sessionData != null && sessionData.Count > 0)
            {
                foreach (var session in sessionData)
                {
                    output.ErrorData.Add(new ErrorDataDetail { Key = session.Key, Value = session.Value, DataType = ErrorDataDetail.ErrorDataType.Session });
                }
            }

            if (userData != null && userData.Count > 0)
            {
                foreach (var user in userData)
                {
                    output.ErrorData.Add(new ErrorDataDetail { Key = user.Key, Value = user.Value, DataType = ErrorDataDetail.ErrorDataType.UserData });
                }
            }

            if (cookieData != null && cookieData.Count > 0)
            {
                foreach (var cookie in cookieData)
                {
                    output.ErrorData.Add(new ErrorDataDetail { Key = cookie.Key, Value = cookie.Value, DataType = ErrorDataDetail.ErrorDataType.Cookie });
                }
            }

            if (serverVariablesData != null && serverVariablesData.Count > 0)
            {
                foreach (var serverVariable in serverVariablesData)
                {
                    output.ErrorData.Add(new ErrorDataDetail { Key = serverVariable.Key, Value = serverVariable.Value, DataType = ErrorDataDetail.ErrorDataType.ServerVariable });
                }
            }

            var assembly = type.Assembly;
            //var assemblyInfo = assembly.GetName();
            //output.AssemblyName = assemblyInfo.Name;
            //output.AssemblyVersion = assemblyInfo.Version.ToString();
            output.Created = DateTime.UtcNow;
            output.NameSpace = type.Namespace;
            //output.ErrorNameSpace = e.Source;
            output.MethodName = type.Name.IndexOf('`') > 0 ? type.Name.Substring(0, type.Name.IndexOf('`')) : type.Name;

            return output;
        }





    }


}
