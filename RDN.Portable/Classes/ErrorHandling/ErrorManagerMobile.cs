using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace RDN.Portable.Error
{
    public class ErrorManagerMobile : ErrorManager
    {
        public ErrorManagerMobile()
        { }

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

     
    }

}
