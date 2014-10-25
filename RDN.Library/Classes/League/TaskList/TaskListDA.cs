using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League.TaskList
{
    public class TaskListDA
    {

        public long ListId { get; set; }
        public string ListName { get; set; }
        public DateTime? EndDate { get; set; }
        public string AssignedTo { get; set; }

        
        public Guid TaskListForLeague { get; set; }
        public Guid ListAddByMember { get; set; }

        public TaskListDA()
        {

        }



        public static bool Add_New_List(RDN.Library.Classes.League.TaskList.TaskListDA NewTaskList)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.Task.TaskList con = new DataModels.League.Task.TaskList();
                con.AssignedTo = NewTaskList.AssignedTo;
                con.EndDate = NewTaskList.EndDate;
                con.ListAddByMember = dc.Members.Where(x => x.MemberId == NewTaskList.ListAddByMember).FirstOrDefault();
                con.ListName = NewTaskList.ListName;
                con.TaskListForLeague = dc.Leagues.Where(x => x.LeagueId == NewTaskList.TaskListForLeague).FirstOrDefault();

                dc.TaskLists.Add(con);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }

        /// <summary>
        /// This function used for Edit and View Details
        /// </summary>
        /// <param name="ListId"></param>
        /// <param name="TaskListForLeague"></param>
        /// <returns>Task List details</returns>
        public static TaskListDA GetData(long ListId, Guid TaskListForLeague)//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var taskList = dc.TaskLists.Where(x => x.ListId == ListId && x.TaskListForLeague.LeagueId == TaskListForLeague).FirstOrDefault();
                if (taskList != null)
                {
                    return DisplayTaskList(taskList);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static TaskListDA DisplayTaskList(DataModels.League.Task.TaskList oTaskList)
        {
            TaskListDA bl = new TaskListDA();
            bl.AssignedTo = oTaskList.AssignedTo;
            bl.EndDate = oTaskList.EndDate;
            bl.ListAddByMember = oTaskList.ListAddByMember.MemberId;
            bl.ListId = oTaskList.ListId;
            bl.ListName = oTaskList.ListName;
            bl.TaskListForLeague = oTaskList.TaskListForLeague.LeagueId;

            return bl;
        }

        public static List<TaskListDA> GetTaskList(Guid leagueId)
        {
            List<TaskListDA> taskLists = new List<TaskListDA>();
            try
            {
                var dc = new ManagementContext();
                var taskList = dc.TaskLists.Where(x => x.TaskListForLeague.LeagueId == leagueId && x.IsDeleted == false).ToList();

                foreach (var b in taskList)
                {
                    taskLists.Add(DisplayTaskList(b));
                }
                return taskLists;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return taskLists;
        }

        public static bool UpdateTaskListInfo(RDN.Library.Classes.League.TaskList.TaskListDA ListToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbList = dc.TaskLists.Where(x => x.ListId == ListToUpdate.ListId).FirstOrDefault();
                if (dbList == null)
                    return false;
                dbList.AssignedTo = ListToUpdate.AssignedTo;
                dbList.EndDate = ListToUpdate.EndDate;
                dbList.ListAddByMember.MemberId = ListToUpdate.ListAddByMember;
                dbList.ListName = ListToUpdate.ListName;
                dbList.TaskListForLeague.LeagueId = ListToUpdate.TaskListForLeague;

                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool DeleteList(long listId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbList = dc.TaskLists.Where(x => x.ListId == listId).FirstOrDefault();
                if (dbList == null)
                    return false;
                dbList.IsDeleted = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


    }
}