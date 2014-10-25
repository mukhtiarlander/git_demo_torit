using RDN.League.Controllers;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.League.TaskList;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Portable.Classes.Federation.Enums;
using RDN.Portable.Classes.League.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.DBUpdate.Migrations
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif

    public class TaskController : BaseController
    {
        #region Task List
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult TaskList()
        {
            return View();
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult TaskList(TaskListDA taskList)
        {

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                taskList.TaskListForLeague = league.LeagueId;
                taskList.ListAddByMember = memId;

                bool execute = RDN.Library.Classes.League.TaskList.TaskListDA.Add_New_List(taskList);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Redirect(Url.Content("~/tasks/league/list?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        
        public ActionResult ViewLists()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "List information updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New List Successfully Added.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Deleted.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Used.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Closed.";
                    this.AddMessage(message);
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);


                var itemLists = RDN.Library.Classes.League.TaskList.TaskListDA.GetTaskList(league.LeagueId);
                return View(itemLists);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult UpdateTaskList(long id, string leagueId)
        {
            try
            {
                
                var Data = RDN.Library.Classes.League.TaskList.TaskListDA.GetData(id, new Guid(leagueId));//.TaskDA.GetData(id);

                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif


        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult UpdateTaskList(TaskListDA oTaskList)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                oTaskList.ListAddByMember = (Guid)memId;
                bool execute = RDN.Library.Classes.League.TaskList.TaskListDA.UpdateTaskListInfo(oTaskList);

                return Redirect(Url.Content("~/tasks/league/list?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }



        #endregion Task List


        #region Task
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult Task(long id,string listTitle)
        {
            TaskDA oDA = null;
            try
            {

                IEnumerable<TaskStatusEnum> status = Enum.GetValues(typeof(TaskStatusEnum))
                                                          .Cast<TaskStatusEnum>();
                 var list = (from a in status
                             select new SelectListItem
                             {
                                 Text = RDN.Portable.Util.Enums.EnumExt.ToFreindlyName(a),
                                 Value = ((int)a).ToString()
                             });

                 ViewBag.status = new SelectList(list, "Value", "Text");

                     
                    oDA = new TaskDA();
                    oDA.TaskListId = id;
                    oDA.TaskListTitle = listTitle;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View(oDA);
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]

        public ActionResult Task(TaskDA oTask)
        {

            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                oTask.TaskAddByMember = memId;
                

                bool execute = RDN.Library.Classes.League.TaskList.TaskDA.Add_New_Task(oTask);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            //tasks/league/list/view/7/TK
            return Redirect(Url.Content("~/tasks/league/list/view/" + oTask.TaskListId + "/" + oTask.TaskListTitle + "?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]

        public ActionResult ViewTasks(long id, string listTitle)
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Task updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New Task Successfully Added.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Deleted.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Used.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Closed.";
                    this.AddMessage(message);
                }
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(memId);
                if (league != null)
                    SetCulture(league.CultureSelected);

                ViewBag.id = id;
                ViewBag.ListTitle = listTitle;
                var tasks = RDN.Library.Classes.League.TaskList.TaskDA.GetTaskList(id);
                return View(tasks);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif

        public ActionResult ViewTaskDetail(long id, string listTitle)
        {
            try
            {

                var Data = TaskDA.GetData(id);
                if (!String.IsNullOrEmpty(Data.Notes))
                {
                    Data.Notes = Data.Notes.Replace(Environment.NewLine, "<br/>");
                }
                ViewBag.ListTitle = listTitle;
                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult DeleteTaskList(long id, string listTitle)
        {
            try
            {

                bool execute = TaskListDA.DeleteList(id);


                return Redirect(Url.Content("~/tasks/league/list" + "?u=" + SiteMessagesEnum.de));

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult DeleteTask(long id, long taskListId, string listTitle)
        {
            try
            {

                bool execute = TaskDA.DeleteTask(id);
                return Redirect(Url.Content("~/tasks/league/list/view/" + taskListId + "/" + listTitle +  "?u=" + SiteMessagesEnum.de));


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult UpdateTask(long id,string taskTitle)
        {
            try
            {
                //var memId = RDN.Library.Classes.Account.User.GetMemberId();
                //if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                //{
                //    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                //}

                IEnumerable<TaskStatusEnum> status = Enum.GetValues(typeof(TaskStatusEnum))
                                                          .Cast<TaskStatusEnum>();
                var list = (from a in status
                            select new SelectListItem
                            {
                                Text = RDN.Utilities.Enums.EnumExt.ToFreindlyName(a),
                                Value = ((int)a).ToString()
                            });

                ViewBag.status = new SelectList(list, "Value", "Text");
                var Data = RDN.Library.Classes.League.TaskList.TaskDA.GetData(id);

                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif


        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult UpdateTask(TaskDA oTask)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                oTask.TaskAddByMember =(Guid) memId;
                bool execute = RDN.Library.Classes.League.TaskList.TaskDA.UpdateTaskInfo(oTask);

                ViewBag.ListTitle = oTask.TaskListTitle;
                return Redirect(Url.Content("~/tasks/league/list/view/" + oTask.TaskListId + "/" + oTask.TaskListTitle +  "?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }

        #endregion Task

    }
}
