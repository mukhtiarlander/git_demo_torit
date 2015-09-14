using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using RDN.Library.DataModels.Context;
using RDN.Utilities.Error;
using RDN.Utilities.Config;
using Common.Site.AppConfig;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Error
{
    public class ErrorDatabaseManager
    {
        public static IList<Expression<Func<object>>> GetEmptyErrorParametersList()
        {
            return new List<Expression<Func<object>>>();
        }

        /// <summary>
        /// Summarizes an exception into an object that then is stored in the database
        /// </summary>
        /// <param name="e">The exception</param>
        /// <param name="type">The type information from the class where this occured. Can be obtained by using this.GetType() or just GetType()</param>
        /// <param name="errorGroup">The group where to put the error. Used for sorting on the error display webpage.</param>
        /// <param name="errorSeverity">The severity of the error. Used for sorting on the error display webpage.</param>
        /// <param name="parameters">Parameters or variables that whose vaules should be saved. Easiest is to get a list from this class, var v = ErrorManager.GetEmptyErrorList(); and then add the data like this: v.Add( () => myImportantVariable ); Variables, arrays, lists and dictionaries work.</param>
        /// <param name="additionalInformation">Additional information that could be useful.</param>        
        public static void AddException(Exception e, Type type, ErrorGroupEnum? errorGroup = null, ErrorSeverityEnum? errorSeverity = null, IList<Expression<Func<object>>> parameters = null, string additionalInformation = null)
        {
            Common.Site.Classes.Exception.Enum.ErrorGroupEnum? errorrGroupEnum = null;
            Common.Site.Classes.Exception.Enum.ErrorSeverityEnum? errorSeverityEnum = null;
            if (errorGroup != null)
            {
                errorrGroupEnum = (Common.Site.Classes.Exception.Enum.ErrorGroupEnum)((int)errorGroup);
            }
            if (errorSeverity != null)
            {
                errorSeverityEnum = (Common.Site.Classes.Exception.Enum.ErrorSeverityEnum)((int)errorSeverity);
            }
            Common.Site.Classes.Exception.ErrorDatabaseManager.AddException(ErrorManagerWeb.GetErrorObject(e, type, HttpContext.Current, errorrGroupEnum, errorSeverityEnum, parameters, additionalInformation));
        }


        /// <summary>
        /// Stores the error object in the database
        /// </summary>
        /// <param name="errorObject">Error object to store</param>
        public static void AddException(ErrorObject errorObject)
        {

            var errorObj = new Common.Site.Classes.Error.ErrorObject()
            {
                AdditionalInformation = errorObject.AdditionalInformation,
                AssemblyName = errorObject.AssemblyName,
                AssemblyVersion = errorObject.AssemblyVersion,
                Created = errorObject.Created,
                ErrorData = errorObject.ErrorData.Select(x => new Common.Site.Classes.Error.ErrorDataDetail()
                {
                    DataType = (Common.Site.Classes.Error.ErrorDataDetail.ErrorDataType)((int)x.DataType),
                    Key = x.Key,
                    Value = x.Value
                }).ToList(),
                ErrorExceptions = errorObject.ErrorExceptions.Select(x => new Common.Site.Classes.Error.ErrorException()
                {
                    Depth = x.Depth,
                    Message = x.Message,
                    MethodName = x.MethodName,
                    StackTrace = x.StackTrace
                }).ToList(),
                ErrorGroup = errorObject.ErrorGroup != null ? (Common.Site.Classes.Exception.Enum.ErrorGroupEnum)((int)errorObject.ErrorGroup) : (Common.Site.Classes.Exception.Enum.ErrorGroupEnum?)null,
                ErrorNameSpace = errorObject.ErrorNameSpace,
                ErrorSeverity = errorObject.ErrorSeverity != null ? (Common.Site.Classes.Exception.Enum.ErrorSeverityEnum)((int)errorObject.ErrorSeverity) : (Common.Site.Classes.Exception.Enum.ErrorSeverityEnum?)null,
                MethodName = errorObject.MethodName,
                NameSpace = errorObject.NameSpace
            };
            Common.Site.Classes.Exception.ErrorDatabaseManager.AddException(errorObj);

        }
        public static void AddException(RDN.Portable.Error.ErrorObject errorObject)
        {
            var errorObj = new Common.Site.Classes.Error.ErrorObject()
                 {
                     AdditionalInformation = errorObject.AdditionalInformation,
                     AssemblyName = errorObject.AssemblyName,
                     AssemblyVersion = errorObject.AssemblyVersion,
                     Created = errorObject.Created,
                     ErrorData = errorObject.ErrorData.Select(x => new Common.Site.Classes.Error.ErrorDataDetail()
                     {
                         DataType = (Common.Site.Classes.Error.ErrorDataDetail.ErrorDataType)((int)x.DataType),
                         Key = x.Key,
                         Value = x.Value
                     }).ToList(),
                     ErrorExceptions = errorObject.ErrorExceptions.Select(x => new Common.Site.Classes.Error.ErrorException()
                     {
                         Depth = x.Depth,
                         Message = x.Message,
                         MethodName = x.MethodName,
                         StackTrace = x.StackTrace
                     }).ToList(),
                     ErrorGroup = errorObject.ErrorGroup != null ? (Common.Site.Classes.Exception.Enum.ErrorGroupEnum)((int)errorObject.ErrorGroup) : (Common.Site.Classes.Exception.Enum.ErrorGroupEnum?)null,
                     ErrorNameSpace = errorObject.ErrorNameSpace,
                     ErrorSeverity = errorObject.ErrorSeverity != null ? (Common.Site.Classes.Exception.Enum.ErrorSeverityEnum)((int)errorObject.ErrorSeverity) : (Common.Site.Classes.Exception.Enum.ErrorSeverityEnum?)null,
                     MethodName = errorObject.MethodName,
                     NameSpace = errorObject.NameSpace

                 };
            Common.Site.Classes.Exception.ErrorDatabaseManager.AddException(errorObj);

        }


        public static List<Classes.Error> GetErrorObjects(int recordsToSkip, int numberOfRecordsToPull)
        {
          var errors = Common.Site.Classes.Exception.ErrorDatabaseManager.GetErrorObjects(recordsToSkip, numberOfRecordsToPull);

          return errors.Select(x => new Classes.Error()
            {
                AdditionalInformation = x.AdditionalInformation,
                AssemblyName = x.AssemblyName,
                AssemblyVersion = x.AssemblyVersion,
                Created = x.Created,
               Data = x.Data.Select(data=>new Error.Data()
               {
                   DataType = (ErrorDataDetail.ErrorDataType)((int)data.DataType),
                   Key = data.Key,
                   Value = data.Value
               }).ToList(),
                ErrorId = x.ErrorId,
                ErrorNameSpace = x.ErrorNameSpace,
                ErrorGroup =
                    x.ErrorGroup != null
                        ? (ErrorGroupEnum) ((int) x.ErrorGroup)
                        : (ErrorGroupEnum?) null,
                ErrorSeverity = x.ErrorSeverity != null ? (ErrorSeverityEnum)((int)x.ErrorSeverity) : (ErrorSeverityEnum?)null,
                MethodName = x.MethodName,
                NameSpace = x.NameSpace,
                Exceptions = x.Exceptions.Select(exc => new Classes.Detail()
                {
                    Depth = exc.Depth,
                    Message = exc.Message,
                    MethodName = exc.MethodName,
                    StackTrace = exc.StackTrace
                }).ToList(),

            }).ToList();

        }

        public static bool DeleteErrorObject(int id)
        {
            return Common.Site.Classes.Exception.ErrorDatabaseManager.DeleteErrorObject(id);

        }
        public static bool DeleteSimilarErrorObjects(int id)
        {
            return Common.Site.Classes.Exception.ErrorDatabaseManager.DeleteSimilarErrorObjects(id);

        }

        public static int GetNumberOfExceptions()
        {
            return Common.Site.Classes.Exception.ErrorDatabaseManager.GetNumberOfExceptions();

        }
        /// <summary>
        /// deletes records.
        /// </summary>
        /// <returns></returns>
        public static int DeleteAllErrorObjects()
        {
            return Common.Site.Classes.Exception.ErrorDatabaseManager.DeleteAllErrorObjects();
        }
    }
}
