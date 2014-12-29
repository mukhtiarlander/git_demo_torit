using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Portable.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League
{
    public class JobBoard
    {
        public long JobId { get; set; }
        public string JobTitle { get; set; }
        public string HoursPerWeek { get; set; }
        public string ReportsTo { get; set; }
        public DateTime JobEnds { get; set; }//Job Apply Dead Line
        public string JobDesc { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }

        public Guid JobCreator { get; set; }
        public Guid LeagueId { get; set; }

        public int DaysRemaining { get; set; }

        public bool BroadcastJob { get; set; }

        public JobBoard()
        {

        }

        public static bool AddNewJob(RDN.Library.Classes.League.JobBoard jobBoard)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.JobBoard con = new DataModels.League.JobBoard();
                con.JobTitle = jobBoard.JobTitle;
                con.JobDesc = jobBoard.JobDesc;
                con.ReportsTo = jobBoard.ReportsTo;
                con.HoursPerWeek = jobBoard.HoursPerWeek;
                con.JobEnds = jobBoard.JobEnds;
                con.League = dc.Leagues.Where(x => x.LeagueId == jobBoard.LeagueId).FirstOrDefault();
                con.JobCreator = dc.Members.Where(x => x.MemberId == jobBoard.JobCreator).FirstOrDefault();

                dc.JobBoards.Add(con);

                int c = dc.SaveChanges();
                if (jobBoard.BroadcastJob)
                {
                    var mems = LeagueFactory.GetLeagueMembersNotificationSettings(jobBoard.LeagueId);

                    foreach (var member in mems)
                    {
                        SendEmailAboutNewBroadcast(member.UserId, member.DerbyName, con.JobId, jobBoard.LeagueId, jobBoard.JobTitle);
                    }
                }
                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }
        private static void SendEmailAboutNewBroadcast(Guid userId, string derbyName, long jobId, Guid leagueId, string jobName)
        {
            try
            {
                if (userId != new Guid())
                {
                    var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname",derbyName}, 
                                            { "JobName", jobName}, 
                                            { "jobLink",ServerConfig.WEBSITE_INTERNAL_DEFAULT_LOCATION +"/view/job/detail/" + jobId+"/"+ leagueId.ToString().Replace("-","") },
                                        };
                    var user = System.Web.Security.Membership.GetUser((object)userId);
                    if (user != null)
                    {
                        EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL_MESSAGES, ServerConfig.DEFAULT_EMAIL_FROM_NAME, user.UserName, EmailServer.EmailServer.DEFAULT_SUBJECT + " Job Created: " + jobName, emailData, EmailServer.EmailServerLayoutsEnum.RDNJobNewCreated);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }


        public static int GetJobsCount(Guid leagueId)
        {

            try
            {
                var dc = new ManagementContext();
                return dc.JobBoards.Where(x => x.League.LeagueId == leagueId && x.IsClosed == false && x.IsDeleted == false && x.IsFilled == false && x.JobEnds >= DateTime.UtcNow).Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
        public static List<JobBoard> GetJobList(Guid leagueId)
        {
            List<JobBoard> jobList = new List<JobBoard>();
            try
            {
                var dc = new ManagementContext();
                var JobList = dc.JobBoards.Where(x => x.League.LeagueId == leagueId && x.IsClosed == false && x.IsDeleted == false && x.IsFilled == false && x.JobEnds >= DateTime.UtcNow).ToList();

                foreach (var b in JobList)
                {
                    jobList.Add(DisplayJobList(b));
                }
                return jobList;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return jobList;
        }
        public static List<JobBoard> GetJobListOld(Guid leagueId)
        {
            List<JobBoard> jobList = new List<JobBoard>();
            try
            {
                var dc = new ManagementContext();
                var JobList = dc.JobBoards.Where(x => x.League.LeagueId == leagueId && x.IsDeleted == false && x.JobEnds <= DateTime.UtcNow).ToList();

                foreach (var b in JobList)
                {
                    jobList.Add(DisplayJobList(b));
                }
                return jobList;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return jobList;
        }
        private static JobBoard DisplayJobList(DataModels.League.JobBoard jobLists)
        {
            JobBoard bl = new JobBoard();
            bl.JobId = jobLists.JobId;
            bl.HoursPerWeek = jobLists.HoursPerWeek;
            bl.IsClosed = jobLists.IsClosed;
            bl.IsDeleted = jobLists.IsDeleted;
            bl.JobCreator = jobLists.JobCreator.MemberId;
            bl.JobDesc = jobLists.JobDesc;
            bl.JobEnds = jobLists.JobEnds;
            bl.JobTitle = jobLists.JobTitle;
            bl.LeagueId = jobLists.League.LeagueId;
            bl.ReportsTo = jobLists.ReportsTo;
            TimeSpan ts = jobLists.JobEnds - DateTime.UtcNow;
            bl.DaysRemaining = ts.Days;
            return bl;
        }

        /// <summary>
        /// This function used for Edit and View Details
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="leagueId"></param>
        /// <returns></returns>
        public static JobBoard GetData(long jobId, Guid leagueId)//This Function used for "Edit" and "Event View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var jobBoard = dc.JobBoards.Where(x => x.JobId == jobId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (jobBoard != null)
                {
                    return DisplayJobList(jobBoard);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        public static bool UpdateJob(RDN.Library.Classes.League.JobBoard JobToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbJob = dc.JobBoards.Where(x => x.JobId == JobToUpdate.JobId).FirstOrDefault();
                if (dbJob == null)
                    return false;

                dbJob.JobDesc = JobToUpdate.JobDesc;
                dbJob.JobEnds = JobToUpdate.JobEnds;
                dbJob.JobTitle = JobToUpdate.JobTitle;
                dbJob.ReportsTo = JobToUpdate.ReportsTo;
                dbJob.HoursPerWeek = JobToUpdate.HoursPerWeek;

                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool CloseJob(long jobId, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbJobList = dc.JobBoards.Where(x => x.JobId == jobId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (dbJobList == null)
                    return false;
                dbJobList.IsClosed = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteJob(long jobId, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbJobList = dc.JobBoards.Where(x => x.JobId == jobId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (dbJobList == null)
                    return false;
                dbJobList.IsDeleted = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool FilledJob(long jobId, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbJobList = dc.JobBoards.Where(x => x.JobId == jobId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (dbJobList == null)
                    return false;
                dbJobList.IsFilled = true;
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
