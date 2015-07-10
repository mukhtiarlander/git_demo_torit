using System;
using System.Collections.Generic;
using System.Linq;
using RDN.Library.DataModels;
using Scoreboard.Library.ViewModel;
using System.Text;
using Scoreboard.Library.Util;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Error.Classes;
using RDN.Library.DataModels.Scoreboard;

namespace RDN.Library.ViewModel
{
    public class ErrorServerViewModel
    {

        //public static void commitScoreboardException(Exception e, string additionalInfo)
        //{
        //    RDNDataContext db = new RDNDataContext();
        //    RDN_Errors_Log UEL = new RDN_Errors_Log();
        //    //UEL.Application_Id = Applications.Instance.ApplicationId;
        //    UEL.Date_Time = DateTime.UtcNow;
        //    UEL.Error_Message = e.Message;
        //    UEL.Error_Source = e.Source;
        //    UEL.Error_Target = e.TargetSite != null ? e.TargetSite.ToString() : null;
        //    UEL.Error_Trace = e.StackTrace;
        //    UEL.Last_Exception = e.InnerException != null ? e.InnerException.ToString() : null;
        //    if (e.InnerException != null)
        //        UEL.Trace_Error = e.InnerException.StackTrace;
        //    UEL.Additional_Info = additionalInfo;

        //    db.RDN_Errors_Logs.InsertOnSubmit(UEL);
        //    db.SubmitChanges();

        //    try
        //    {
        //        RDN.Library.Util.Email.SendEmail(false, "info@rdnation.com", "Error", Xml.SerializeToString(e));
        //    }
        //    catch
        //    {

        //    }
        //}

        //public static void commitScoreboardException(ErrorViewModel e, string additionalInfo)
        //{
        //    RDNDataContext db = new RDNDataContext();
        //    RDN_Errors_Log UEL = new RDN_Errors_Log();
        //    //UEL.Application_Id = Applications.Instance.ApplicationId;
        //    UEL.Date_Time = DateTime.UtcNow;
        //    UEL.Error_Message = e.ExceptionMessege;
        //    UEL.Error_Source = e.ExceptionSource;
        //    UEL.Error_Target = e.Error_Target;
        //    UEL.Error_Trace = e.ExceptionStack;
        //    UEL.Last_Exception = e.Last_Exception;
        //    UEL.Load_Date = e.DateTime.ToString();
        //    UEL.Version = e.Version;
        //    UEL.Scoreboard_Id = e.ScoreboardId;
        //    UEL.Game_Id = e.GameId;
        //    UEL.Additional_Info = additionalInfo + e.Log;

        //    db.RDN_Errors_Logs.InsertOnSubmit(UEL);
        //    db.SubmitChanges();

        //    try
        //    {
        //        RDN.Library.Util.Email.SendEmail(false, "info@rdnation.com", "Error", Xml.SerializeToString(e));
        //    }
        //    catch
        //    {

        //    }
        //}

        public static List<Error> GetExceptions(int recordsToSkip, int numberOfRecordsToPull)
        {
            return ErrorDatabaseManager.GetErrorObjects(recordsToSkip, numberOfRecordsToPull);
        }

        public static int GetNumberOfExceptions()
        {
            return ErrorDatabaseManager.GetNumberOfExceptions();
        }

        public static void DeleteError(int errorId)
        {
            ErrorDatabaseManager.DeleteErrorObject(errorId);
        }

        public static List<ScoreboardFeedback> GetFeedbackItems(int recordsToSkip, int numberOfRecordsToPull)
        {

            var dc = new ManagementContext();
            return dc.ScoreboardFeedback.Where(x => x.IsArchived == false).OrderByDescending(x => x.Created).Select(x => x).Skip(recordsToSkip).Take(numberOfRecordsToPull).ToList();
        }

        public static int GetNumberOfFeedbackItems()
        {
            var dc = new ManagementContext();
            return dc.ScoreboardFeedback.Where(x => x.IsArchived == false).Count();
        }

        public static void DeleteFeedbackItem(int itemId)
        {
            var dc = new ManagementContext();
            var item = dc.ScoreboardFeedback.FirstOrDefault(x => x.FeedBackId.Equals(itemId));
            if (item == null) return;
            dc.ScoreboardFeedback.Remove(item);
            dc.SaveChanges();
        }
        public static void ArchiveFeedbackItem(int itemId)
        {
            var dc = new ManagementContext();
            var item = dc.ScoreboardFeedback.FirstOrDefault(x => x.FeedBackId.Equals(itemId));
            if (item == null)
                return;
            item.IsArchived = true;
            dc.SaveChanges();
        }
    }
}
