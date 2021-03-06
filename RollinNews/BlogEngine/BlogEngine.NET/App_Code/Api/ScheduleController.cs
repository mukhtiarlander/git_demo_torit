﻿using BlogEngine.Core;
using BlogEngine.Core.Data;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using Newtonsoft.Json;
using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Facebook;
using RDN.Library.Classes.Facebook.Classes;
using RDN.Library.Classes.Facebook.Enum;
using RDN.Library.Classes.Game;
using RDN.Library.Classes.Payment;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.RN.Funds;
using RDN.Library.Classes.Social.Facebook;
using RDN.Library.Classes.Social.Twitter;
using RDN.Portable.Classes.Games;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Linq;
using Common.AutomatedTasks.Library;
using Common.AutomatedTasks.Library.Classes.Frequency;
using RDN.Library.Classes.RN.Communication;
using RDN.Library.Classes.AutomatedTask.Enums;
using Common.AutomatedTasks.Library.Classes.Users;

public class ScheduleController : ApiController
{

    public ScheduleController()
    {
    }


    public IEnumerable<UserTask> Get(int take = 10, int page = 0)
    {
        try
        {
            var tasks = UserTaskFactory.PullActiveTasks(page, take);
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].UserName = Membership.GetUser(tasks[i].UserId).UserName;
            }
            return tasks;
        }
        catch (UnauthorizedAccessException)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }
    [HttpPut]
    public HttpResponseMessage ProcessChecked([FromBody]List<UserTask> items)
    {
        try
        {
            var action = Request.GetRouteData().Values["id"].ToString();
            foreach (var item in items)
            {
                if (item.IsChecked)
                {
                    if (action.ToLower() == "delete")
                    {
                        UserTaskFactory.DeleteActiveTask(item.TaskId);
                    }


                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }

    public HttpResponseMessage Post([FromBody]UserTask task)
    {
        try
        {
            var user = Membership.GetUser(task.UserName);
            UserTaskFactory.Initialize(task.TaskType, (TaskFrequency)task.TaskFrequency).AddUserTask((Guid)user.ProviderUserKey, DateTime.Parse(task.DateCreated));

            return Request.CreateResponse(HttpStatusCode.Created);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }



    public HttpResponseMessage RunReminderEmailsForScheduledPosts(string r)
    {
        try
        {
            var userIds = UserTaskFactory.Initialize((long)UserTaskFactoryType.AuthorPostingEmail, TaskFrequency.Daily).PullUsers();

            Authors.SendAutomatedPostingEmailToAuthors(userIds);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
    [HttpGet]
    public HttpResponseMessage GetRights(string id)
    {
        try
        {
            var result = UserTaskFactory.LoadTypes();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        catch (UnauthorizedAccessException)
        {
            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }
        catch (Exception exception)
        {
            ErrorDatabaseManager.AddException(exception, GetType());
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
