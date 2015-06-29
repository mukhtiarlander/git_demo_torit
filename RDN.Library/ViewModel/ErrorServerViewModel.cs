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
