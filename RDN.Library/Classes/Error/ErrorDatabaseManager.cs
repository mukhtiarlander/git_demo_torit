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
            AddException(ErrorManagerWeb.GetErrorObject(e, type, HttpContext.Current, errorGroup, errorSeverity, parameters, additionalInformation));
        }


        /// <summary>
        /// Stores the error object in the database
        /// </summary>
        /// <param name="errorObject">Error object to store</param>
        public static void AddException(ErrorObject errorObject)
        {
            try
            {
                ManagementContext dc = ManagementContext.DataContext;

                var databaseError = new DataModels.Exception.Exception
                                        {
                                            AdditionalInformation = errorObject.AdditionalInformation,
                                            AssemblyName = errorObject.AssemblyName,
                                            AssemblyVersion = errorObject.AssemblyVersion,
                                            Created = errorObject.Created,
                                            ErrorNameSpace = errorObject.ErrorNameSpace,
                                            MethodName = errorObject.MethodName,
                                            NameSpace = errorObject.NameSpace,
                                            Group = errorObject.ErrorGroup.HasValue ? (byte?)errorObject.ErrorGroup.Value : null,
                                            Severity = errorObject.ErrorSeverity.HasValue ? (byte?)errorObject.ErrorSeverity.Value : null
                                        };

                if (errorObject.ErrorData != null && errorObject.ErrorData.Count > 0)
                {
                    foreach (var errorDataDetail in errorObject.ErrorData)
                    {
                        var databaseErrorData = new DataModels.Exception.ExceptionData
                        {
                            DataType = (byte)errorDataDetail.DataType,
                            Name = errorDataDetail.Key,
                            Data = errorDataDetail.Value
                        };

                        databaseError.ExceptionData.Add(databaseErrorData);
                    }
                }

                if (errorObject.ErrorExceptions != null)
                    foreach (var exception in errorObject.ErrorExceptions)
                    {
                        var exceptionErrorDetail = new DataModels.Exception.ExceptionDetail();
                        exceptionErrorDetail.Depth = exception.Depth;
                        exceptionErrorDetail.Message = exception.Message;
                        exceptionErrorDetail.MethodName = exception.MethodName;
                        exceptionErrorDetail.StackTrace = exception.StackTrace;
                        databaseError.ExceptionDetails.Add(exceptionErrorDetail);
                    }

                dc.ErrorExceptions.Add(databaseError);
                dc.SaveChanges();
            }
            catch (Exception except)
            { AddException(except, except.GetType()); }
        }
        public static void AddException(RDN.Portable.Error.ErrorObject errorObject)
        {
            try
            {
                var dc = new ManagementContext();
                var databaseError = new DataModels.Exception.Exception
                {
                    AdditionalInformation = errorObject.AdditionalInformation,
                    AssemblyName = errorObject.AssemblyName,
                    AssemblyVersion = errorObject.AssemblyVersion,
                    Created = errorObject.Created,
                    ErrorNameSpace = errorObject.ErrorNameSpace,
                    MethodName = errorObject.MethodName,
                    NameSpace = errorObject.NameSpace,
                    Group = errorObject.ErrorGroup.HasValue ? (byte?)errorObject.ErrorGroup.Value : null,
                    Severity = errorObject.ErrorSeverity.HasValue ? (byte?)errorObject.ErrorSeverity.Value : null
                };

                if (errorObject.ErrorData != null && errorObject.ErrorData.Count > 0)
                {
                    foreach (var errorDataDetail in errorObject.ErrorData)
                    {
                        var databaseErrorData = new DataModels.Exception.ExceptionData
                        {
                            DataType = (byte)errorDataDetail.DataType,
                            Name = errorDataDetail.Key,
                            Data = errorDataDetail.Value
                        };

                        databaseError.ExceptionData.Add(databaseErrorData);
                    }
                }

                if (errorObject.ErrorExceptions != null)
                    foreach (var exception in errorObject.ErrorExceptions)
                    {
                        var exceptionErrorDetail = new DataModels.Exception.ExceptionDetail();
                        exceptionErrorDetail.Depth = exception.Depth;
                        exceptionErrorDetail.Message = exception.Message;
                        exceptionErrorDetail.MethodName = exception.MethodName;
                        exceptionErrorDetail.StackTrace = exception.StackTrace;
                        databaseError.ExceptionDetails.Add(exceptionErrorDetail);
                    }

                dc.ErrorExceptions.Add(databaseError);
                dc.SaveChanges();
            }
            catch (Exception except)
            { AddException(except, except.GetType()); }
        }


        public static List<Classes.Error> GetErrorObjects(int recordsToSkip, int numberOfRecordsToPull)
        {
            var output = new List<Classes.Error>();
            var dc = new ManagementContext();
            var errors =
                dc.ErrorExceptions.Include("ExceptionData").Include("ExceptionDetails").OrderByDescending(x => x.Created)
                    .Skip(recordsToSkip).Take(numberOfRecordsToPull);

            foreach (var error in errors)
            {
                var record = new Classes.Error
                                 {
                                     AdditionalInformation = error.AdditionalInformation,
                                     AssemblyName = error.AssemblyName,
                                     AssemblyVersion = error.AssemblyVersion,
                                     Created = error.Created,
                                     ErrorGroup =
                                         error.Group.HasValue ? (ErrorGroupEnum)error.Group.Value : (ErrorGroupEnum?)null,
                                     ErrorId = error.ExceptionId,
                                     ErrorNameSpace = error.ErrorNameSpace,
                                     ErrorSeverity =
                                         error.Severity.HasValue
                                             ? (ErrorSeverityEnum)error.Severity.Value
                                             : (ErrorSeverityEnum?)null,
                                     MethodName = error.MethodName,
                                     NameSpace = error.NameSpace
                                 };

                foreach (var data in error.ExceptionData)
                {
                    var recordData = new Data
                                         {
                                             DataType = (ErrorDataDetail.ErrorDataType)data.DataType,
                                             Key = data.Name,
                                             Value = data.Data
                                         };
                    record.Data.Add(recordData);
                }

                foreach (var data in error.ExceptionDetails)
                {
                    var recordData = new Classes.Detail
                                         {
                                             Depth = data.Depth,
                                             Message = data.Message,
                                             MethodName = data.MethodName,
                                             StackTrace = data.StackTrace
                                         };
                    record.Exceptions.Add(recordData);
                }

                output.Add(record);
            }
            return output;
        }

        public static bool DeleteErrorObject(int id)
        {
            var dc = new ErrorContext();
            var obj = dc.ErrorExceptions.FirstOrDefault(x => x.ExceptionId.Equals(id));
            if (obj == null) return false;
            dc.ErrorExceptions.Remove(obj);
            var result = dc.SaveChanges();
            return result > 0;

        }
        public static bool DeleteSimilarErrorObjects(int id)
        {
            var dc = new ErrorContext();
            var obj = dc.ErrorExceptions.FirstOrDefault(x => x.ExceptionId.Equals(id));
            if (obj == null) return false;
            var objs = dc.ErrorExceptions.Where(x => x.AdditionalInformation == obj.AdditionalInformation && obj.MethodName == x.MethodName && obj.ErrorNameSpace == x.ErrorNameSpace);

            dc.ErrorExceptions.RemoveRange(objs);
            var result = dc.SaveChanges();
            return result > 0;

        }

        public static int GetNumberOfExceptions()
        {
            var dc = new ErrorContext();
            return dc.ErrorExceptions.Count();

        }
        /// <summary>
        /// deletes records.
        /// </summary>
        /// <returns></returns>
        public static int DeleteAllErrorObjects()
        {
            var dc = new ErrorContext();
            var obj = dc.ErrorExceptions.OrderByDescending(x => x.Created);
            int count = 0;
            foreach (var error in obj)
            {
                dc.ErrorExceptions.Remove(error);
                count++;
                if (count == 200)
                    break;
            }
            var result = dc.SaveChanges();
            return count;
        }
    }
}
