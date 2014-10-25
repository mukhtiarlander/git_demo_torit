using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Scoreboard;

namespace RDN.Library.Classes.Scoreboard
{
    public class Scoreboard
    {

        /// <summary>
        /// inserts and updates the mac address of the scoreboard so that we can track active installs.
        /// </summary>
        /// <param name="id"></param>
        public static ScoreboardInstance insertScoreboardActiveId(string scoreboardMacAddress, string version)
        {
            try
            {
                ManagementContext db = new ManagementContext();
                var instance = (from xx in db.ScoreboardInstance
                                where xx.InstanceMacAddress == scoreboardMacAddress
                                select xx).FirstOrDefault();
                if (instance != null)
                {
                    instance.LastUpdated = DateTime.UtcNow;
                    instance.LoadsCount += 1;
                    if (!String.IsNullOrEmpty(version))
                        instance.Version = version;
                    db.SaveChanges();
                    return instance;
                }
                else
                {
                    ScoreboardInstance ins = new ScoreboardInstance();
                    ins.Created = DateTime.UtcNow;
                    ins.InstanceMacAddress = scoreboardMacAddress;
                    ins.LastUpdated = DateTime.UtcNow;
                    ins.LoadsCount = 1;
                    if (!String.IsNullOrEmpty(version))
                        ins.Version = version;
                    db.ScoreboardInstance.Add(ins);
                    db.SaveChanges();
                    return ins;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        /// <summary>
        /// gets the scoreboard ID.
        /// </summary>
        /// <param name="scoreboardMacAddress"></param>
        /// <returns></returns>
        public static ScoreboardInstance getScoreboardId(string scoreboardMacAddress)
        {
            try
            {
                ManagementContext db = new ManagementContext();
                var instance = (from xx in db.ScoreboardInstance
                                where xx.InstanceMacAddress == scoreboardMacAddress
                                select xx).FirstOrDefault();
                if (instance != null)
                {
                    return instance;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return insertScoreboardActiveId(scoreboardMacAddress, String.Empty);
        }
    }
}
