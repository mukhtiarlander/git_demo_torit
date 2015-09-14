using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using Common.Site.Classes.Error;
using Common.Site.Classes.Exception.Enum;


namespace RDN.Library.Classes.Error
{
    public class ErrorManagerWeb : ErrorManager
    {
        public static ErrorObject GetErrorObject(Exception e, Type type, HttpContext context, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            //Common.Site.Classes.Exception.Enum.ErrorGroupEnum? errorrGroupEnum = null;
            //Common.Site.Classes.Exception.Enum.ErrorSeverityEnum? errorSeverityEnum = null;
            //if (errorGroup != null)
            //{
            //    errorrGroupEnum = (Common.Site.Classes.Exception.Enum.ErrorGroupEnum)((int)errorGroup);
            //}
            //if (errorSeverity != null)
            //{
            //    errorSeverityEnum = (Common.Site.Classes.Exception.Enum.ErrorSeverityEnum)((int)errorSeverity);
            //}

            return Common.Site.Classes.Exception.ErrorManagerWeb.GetErrorObject(e, type, context, errorGroup, errorSeverity, parameters, additionalInformation);
         
        }
        
    }
}
