using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League.TaskList
{
    public class TaskDA
    {

        public long TaskId { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public int Completed { get; set; }
        public int StatusId { get; set; }
        public string Notes { get; set; }
        public string TaskListTitle { get; set; }

        public long TaskListId { get; set; }
        public Guid TaskAddByMember { get; set; }

        public TaskDA()
        {

        }
        
 
        public static bool Add_New_Task(RDN.Library.Classes.League.TaskList.TaskDA NewTask)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.Task.Task con = new DataModels.League.Task.Task();
                con.Completed = NewTask.Completed;
                con.Title = NewTask.Title;
                con.DeadLine = NewTask.DeadLine;
                con.EndDate = NewTask.EndDate;
                con.Notes = NewTask.Notes;
                con.StartDate = NewTask.StartDate;
                con.StatusId = NewTask.StatusId;
                con.TaskAddByMember = dc.Members.Where(x => x.MemberId == NewTask.TaskAddByMember).FirstOrDefault();
                con.TaskBelongsTo = dc.TaskLists.Where(x => x.ListId == NewTask.TaskListId).FirstOrDefault();
                

                dc.Tasks.Add(con);

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
        /// <param name="taskId"></param>
        /// <param name=""></param>
        /// <returns>Task List details</returns>
        public static TaskDA GetData(long taskId)//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var taskList = dc.Tasks.Where(x => x.TaskId == taskId).FirstOrDefault();
                if (taskList != null)
                {
                    return DisplayTask(taskList);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static TaskDA DisplayTask(DataModels.League.Task.Task oTask)
        {
            TaskDA bl = new TaskDA();
            bl.Completed = oTask.Completed;
            bl.Title = oTask.Title;
            bl.DeadLine = oTask.DeadLine;
            bl.EndDate = oTask.EndDate;
            bl.Notes = oTask.Notes;
            bl.StartDate = oTask.StartDate;
            bl.StatusId = oTask.StatusId;
            bl.TaskAddByMember = oTask.TaskAddByMember.MemberId;
            bl.TaskId = oTask.TaskId;
            bl.TaskListId = oTask.TaskBelongsTo.ListId;
            bl.TaskListTitle = oTask.TaskBelongsTo.ListName;
            return bl;
        }

        public static List<TaskDA> GetTaskList(long taskListID)
        {
            List<TaskDA> taskLists = new List<TaskDA>();
            try
            {
                var dc = new ManagementContext();
                var taskList = dc.Tasks.Where(x => x.TaskBelongsTo.ListId == taskListID && x.IsDeleted == false).ToList();

                foreach (var b in taskList)
                {
                    taskLists.Add(DisplayTask(b));
                }
                return taskLists;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return taskLists;
        }

        public static bool UpdateTaskInfo(RDN.Library.Classes.League.TaskList.TaskDA TaskToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbList = dc.Tasks.Where(x => x.TaskId == TaskToUpdate.TaskId).FirstOrDefault();
                if (dbList == null)
                    return false;

                dbList.Completed = TaskToUpdate.Completed;
                dbList.Title = TaskToUpdate.Title;
                dbList.DeadLine = TaskToUpdate.DeadLine;
                dbList.EndDate = TaskToUpdate.EndDate;
                dbList.Notes = TaskToUpdate.Notes;
                dbList.StartDate = TaskToUpdate.StartDate;
                dbList.StatusId = TaskToUpdate.StatusId;
                dbList.TaskAddByMember.MemberId = TaskToUpdate.TaskAddByMember;
                dbList.TaskBelongsTo.ListId = TaskToUpdate.TaskListId;
                
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool DeleteTask(long taskId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbTask = dc.Tasks.Where(x => x.TaskId == taskId).FirstOrDefault();
                if (dbTask == null)
                    return false;
                dbTask.IsDeleted = true;
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
