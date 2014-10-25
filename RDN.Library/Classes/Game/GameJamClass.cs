using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game.Members;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game;
using Scoreboard.Library.StopWatch;
using Scoreboard.Library.ViewModel;

namespace RDN.Library.Classes.Game
{
    internal class GameJamClass
    {
        public static int insertNewJamIntoDb(Guid gameId, Guid team1Id, Guid team2Id, JamViewModel jam, ManagementContext db, DataModels.Game.Game g)
        {
            int c = 0;
            GameJam jamNew = new GameJam();
            if (jam.Blocker1T1 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team1Id, jam.Blocker1T1.SkaterId, g);
                if (m != null)
                    jamNew.Blocker1Team1 = m;
            }
            if (jam.Blocker1T2 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team2Id, jam.Blocker1T2.SkaterId, g);
                if (m != null)
                    jamNew.Blocker1Team2 = m;
            }
            if (jam.Blocker2T1 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team1Id, jam.Blocker2T1.SkaterId, g);
                if (m != null)
                    jamNew.Blocker2Team1 = m;
            }
            if (jam.Blocker2T2 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team2Id, jam.Blocker2T2.SkaterId, g);
                if (m != null)
                    jamNew.Blocker2Team2 = m;
            }
            if (jam.Blocker3T1 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team1Id, jam.Blocker3T1.SkaterId, g);
                if (m != null)
                    jamNew.Blocker3Team1 = m;
            }
            if (jam.Blocker3T2 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team2Id, jam.Blocker3T2.SkaterId, g);
                if (m != null)
                    jamNew.Blocker3Team2 = m;
            }
            if (jam.Blocker4T1 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team1Id, jam.Blocker4T1.SkaterId, g);
                if (m != null)
                    jamNew.Blocker4Team1 = m;
            }
            if (jam.Blocker4T2 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team2Id, jam.Blocker4T2.SkaterId, g);
                if (m != null)
                    jamNew.Blocker4Team2 = m;
            }
            if (jam.PivotT1 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team1Id, jam.PivotT1.SkaterId, g);
                if (m != null)
                    jamNew.PivotTeam1 = m;
            }
            if (jam.PivotT2 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team2Id, jam.PivotT2.SkaterId, g);
                if (m != null)
                    jamNew.PivotTeam2 = m;
            }
            if (jam.JammerT1 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team1Id, jam.JammerT1.SkaterId, g);
                if (m != null)
                {
                    jamNew.JammerTeam1 = m;
                    jamNew.DidTeam1JammerLoseLeadEligibility = jam.JammerT1.LostLeadJammerEligibility;
                }
            }
            if (jam.JammerT2 != null)
            {
                var m = GameMemberClass.getMemberOfTeamInGame(team2Id, jam.JammerT2.SkaterId, g);
                if (m != null)
                {
                    jamNew.JammerTeam2 = m;
                    jamNew.DidTeam2JammerLoseLeadEligibility = jam.JammerT2.LostLeadJammerEligibility;
                }
            }

            jamNew.JamId = jam.JamId;
            jamNew.CurrentPeriod = jam.CurrentPeriod;
            jamNew.Created = DateTime.UtcNow;
            jamNew.GameTimeElapsedMillisecondsStart = jam.GameTimeElapsedMillisecondsStart;
            jamNew.GameTimeElapsedMillisecondsEnd = jam.GameTimeElapsedMillisecondsEnd;
            jamNew.JamNumber = jam.JamNumber;
            jamNew.TeamLeadingJam = (byte)jam.TeamLeadingJam;
            jamNew.TotalPointsForJamT1 = jam.TotalPointsForJamT1;
            jamNew.TotalPointsForJamT2 = jam.TotalPointsForJamT2;
            jamNew.DidJamEndWithInjury = jam.DidJamEndWithInjury;
            jamNew.DidJamGetCalledByJammerT1 = jam.DidJamGetCalledByJammerT1;
            jamNew.DidJamGetCalledByJammerT2 = jam.DidJamGetCalledByJammerT2;
            g.GameJams.Add(jamNew);
            c += db.SaveChanges();



            foreach (var leadJammer in jam.LeadJammers)
            {
                try
                {
                    GameLeadJammer lead = new GameLeadJammer();
                    lead.Created = DateTime.UtcNow;
                    lead.GameMemberId = leadJammer.Jammer.SkaterId;
                    lead.GameTimeInMilliseconds = leadJammer.GameTimeInMilliseconds;
                    lead.GameJam = jamNew;
                    lead.JamTimeInMilliseconds = lead.JamTimeInMilliseconds;
                    jamNew.LeadJammers.Add(lead);
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            db.SaveChanges();
            if (jam.JamClock != null && jam.JamClock.StartTime != new DateTime())
            {
                try
                {
                    GameStopwatch stop = new GameStopwatch();
                    stop.Game = g;
                    stop.Created = DateTime.UtcNow;
                    stop.IsClockAtZero = jam.JamClock.IsClockAtZero == true ? 1 : 0; ;
                    stop.IsRunning = jam.JamClock.IsRunning == true ? 1 : 0;
                    stop.Length = jam.JamClock.TimerLength;
                    stop.JamNumber = jam.JamNumber;
                    stop.JamId = jam.JamId;
                    stop.StartDateTime = jam.JamClock.StartTime;
                    stop.StopwatchForId = gameId;
                    stop.TimeElapsed = jam.JamClock.TimeElapsed;
                    stop.TimeRemaining = jam.JamClock.TimeRemaining;
                    stop.Type = (int)StopWatchTypeEnum.JamClock;
                    db.GameStopWatch.Add(stop);
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            g.GameJams.Add(jamNew);
            c += db.SaveChanges();


            AddJamPasses(jam, db, g, jamNew);
            return c;
        }
        /// <summary>
        /// adds all the jam passes
        /// </summary>
        /// <param name="jam"></param>
        /// <param name="db"></param>
        /// <param name="g"></param>
        /// <param name="jamNew"></param>
        /// <returns></returns>
        private static int AddJamPasses(JamViewModel jam, ManagementContext db, DataModels.Game.Game g, GameJam jamNew)
        {
            int c = 0;
            try
            {
                foreach (var pass in jam.JamPasses)
                {
                    try
                    {
                        GameJamPasses p = new GameJamPasses();
                        p.GameJam = jamNew;
                        p.GamePassId = pass.PassId;
                        p.JamTimeMilliseconds = pass.JamTimeMilliseconds;
                        p.PassNumber = pass.PassNumber;
                        p.PointsScoredForPass = pass.PointsScoredForPass;
                        p.SkaterWhoPassed = GameMemberClass.getMemberOfTeamInGame(pass.SkaterWhoPassed.SkaterId, g);
                        p.TeamNumberEnum = (byte)pass.Team;
                        if (p.SkaterWhoPassed != null)
                            jamNew.JamPasses.Add(p);

                        //moved to down below because this was throwing an error...
                        //Collection was modified; enumeration operation may not execute.
                        //testing it out..  See if it will throw another.
                        // c += db.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
                c += db.SaveChanges();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            } return c;
        }
        public static int updateJamToDb(GameViewModel game, ManagementContext db, JamViewModel jam, DataModels.Game.Game gDb)
        {
            int c = 0;
            try
            {


                var findJam = gDb.GameJams.Where(x => x.JamId == jam.JamId).FirstOrDefault();

                if (findJam == null)
                    GameJamClass.insertNewJamIntoDb(game.GameId, game.Team1.TeamId, game.Team2.TeamId, jam, db, gDb);
                else
                {
                    findJam.Game = findJam.Game;
                    findJam.CurrentPeriod = jam.CurrentPeriod;
                    findJam.DidJamEndWithInjury = jam.DidJamEndWithInjury;
                    findJam.DidJamGetCalledByJammerT1 = jam.DidJamGetCalledByJammerT1;
                    findJam.DidJamGetCalledByJammerT2 = jam.DidJamGetCalledByJammerT2;
                    findJam.TotalPointsForJamT1 = jam.TotalPointsForJamT1;
                    findJam.TotalPointsForJamT2 = jam.TotalPointsForJamT2;
                    findJam.GameTimeElapsedMillisecondsEnd = jam.GameTimeElapsedMillisecondsEnd;
                    findJam.GameTimeElapsedMillisecondsStart = jam.GameTimeElapsedMillisecondsStart;
                    c += db.SaveChanges();
                    try
                    {
                        for (int i = 0; i < jam.LeadJammers.Count; i++)
                        {
                            try
                            {
                                var j = findJam.LeadJammers.Where(x => x.GameJamLeadId == jam.LeadJammers[i].GameLeadJamId).FirstOrDefault();
                                if (j == null)
                                {
                                    GameLeadJammer l = new GameLeadJammer();
                                    l.GameJam = findJam;
                                    l.GameMemberId = jam.LeadJammers[i].Jammer.SkaterId;
                                    l.GameTimeInMilliseconds = jam.LeadJammers[i].GameTimeInMilliseconds;
                                    l.JamTimeInMilliseconds = jam.LeadJammers[i].JamTimeInMilliseconds;
                                    l.GameJamLeadId = jam.LeadJammers[i].GameLeadJamId;
                                    findJam.LeadJammers.Add(l);
                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }
                        c += db.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                    try
                    {
                        for (int i = 0; i < jam.JamPasses.Count; i++)
                        {
                            try
                            {
                                var pass = findJam.JamPasses.Where(x => x.GamePassId == jam.JamPasses[i].PassId).FirstOrDefault();
                                if (pass == null)
                                {
                                    GameJamPasses p = new GameJamPasses();
                                    p.GameJam = findJam;
                                    p.GamePassId = jam.JamPasses[i].PassId;
                                    p.JamTimeMilliseconds = jam.JamPasses[i].JamTimeMilliseconds;
                                    p.PassNumber = jam.JamPasses[i].PassNumber;
                                    p.PointsScoredForPass = jam.JamPasses[i].PointsScoredForPass;
                                    p.SkaterWhoPassed = GameMemberClass.getMemberOfTeamInGame(jam.JamPasses[i].SkaterWhoPassed.SkaterId, gDb);
                                    p.TeamNumberEnum = (byte)jam.JamPasses[i].Team;
                                    findJam.JamPasses.Add(p);
                                }
                                else
                                {
                                    pass.PointsScoredForPass = jam.JamPasses[i].PointsScoredForPass;
                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }
                        c += db.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                    if ((jam.Blocker1T1 != null && findJam.Blocker1Team1 == null) || (jam.Blocker1T1 != null && findJam.Blocker1Team1.GameMemberId != jam.Blocker1T1.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team1.TeamId, jam.Blocker1T1.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker1Team1 = m;
                    }
                    if ((jam.Blocker1T2 != null && findJam.Blocker1Team2 == null) || (jam.Blocker1T2 != null && findJam.Blocker1Team2.GameMemberId != jam.Blocker1T2.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team2.TeamId, jam.Blocker1T2.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker1Team2 = m;
                    }
                    if ((jam.Blocker2T1 != null && findJam.Blocker2Team1 == null) || (jam.Blocker2T1 != null && findJam.Blocker2Team1.GameMemberId != jam.Blocker2T1.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team1.TeamId, jam.Blocker2T1.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker2Team1 = m;
                    }
                    if ((jam.Blocker2T2 != null && findJam.Blocker2Team2 == null) || (jam.Blocker2T2 != null && findJam.Blocker2Team2.GameMemberId != jam.Blocker2T2.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team2.TeamId, jam.Blocker2T2.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker2Team2 = m;
                    }
                    if ((jam.Blocker3T1 != null && findJam.Blocker3Team1 == null) || (jam.Blocker3T1 != null && findJam.Blocker3Team1.GameMemberId != jam.Blocker3T1.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team1.TeamId, jam.Blocker3T1.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker3Team1 = m;
                    }
                    if ((jam.Blocker3T2 != null && findJam.Blocker3Team2 == null) || (jam.Blocker3T2 != null && findJam.Blocker3Team2.GameMemberId != jam.Blocker3T2.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team2.TeamId, jam.Blocker3T2.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker3Team2 = m;
                    }
                    if ((jam.Blocker4T1 != null && findJam.Blocker4Team1 == null) || (jam.Blocker4T1 != null && findJam.Blocker4Team1.GameMemberId != jam.Blocker4T1.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team1.TeamId, jam.Blocker4T1.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker4Team1 = m;
                    }
                    if ((jam.Blocker4T2 != null && findJam.Blocker4Team2 == null) || (jam.Blocker4T2 != null && findJam.Blocker4Team2.GameMemberId != jam.Blocker4T2.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team2.TeamId, jam.Blocker4T2.SkaterId, gDb);
                        if (m != null)
                            findJam.Blocker4Team2 = m;
                    }
                    if ((jam.PivotT1 != null && findJam.PivotTeam1 == null) || (jam.PivotT1 != null && findJam.PivotTeam1.GameMemberId != jam.PivotT1.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team1.TeamId, jam.PivotT1.SkaterId, gDb);
                        if (m != null)
                            findJam.PivotTeam1 = m;
                    }
                    if ((jam.PivotT2 != null && findJam.PivotTeam2 == null) || (jam.PivotT2 != null && findJam.PivotTeam2.GameMemberId != jam.PivotT2.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team2.TeamId, jam.PivotT2.SkaterId, gDb);
                        if (m != null)
                            findJam.PivotTeam2 = m;
                    }
                    if ((jam.JammerT1 != null && findJam.JammerTeam1 == null) || (jam.JammerT1 != null && findJam.JammerTeam1.GameMemberId != jam.JammerT1.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team1.TeamId, jam.JammerT1.SkaterId, gDb);
                        if (m != null)
                        {
                                                        findJam.JammerTeam1 = m;
                            findJam.DidTeam1JammerLoseLeadEligibility = jam.JammerT1.LostLeadJammerEligibility;
                        }
                    }
                    if ((jam.JammerT2 != null && findJam.JammerTeam2 == null) || (jam.JammerT2 != null && findJam.JammerTeam2.GameMemberId != jam.JammerT2.SkaterId))
                    {
                        var m = GameMemberClass.getMemberOfTeamInGame(game.Team2.TeamId, jam.JammerT2.SkaterId, gDb);
                        if (m != null)
                        {
                            findJam.JammerTeam2 = m;
                            findJam.DidTeam2JammerLoseLeadEligibility = jam.JammerT2.LostLeadJammerEligibility;
                        }
                    }

                    c += db.SaveChanges();
                }
                var findClock = (from xx in db.GameStopWatch
                                 where xx.StopwatchForId == game.GameId
                                 where xx.JamId == jam.JamId
                                 select xx).FirstOrDefault();
                if (findClock != null && jam.JamClock != null)
                {
                    findClock.IsClockAtZero = jam.JamClock.IsClockAtZero == true ? 1 : 0;
                    findClock.IsRunning = jam.JamClock.IsRunning == true ? 1 : 0;
                    findClock.Length = jam.JamClock.TimerLength;
                    if (jam.JamClock.StartTime != new DateTime())
                        findClock.StartDateTime = jam.JamClock.StartTime;
                    findClock.TimeElapsed = jam.JamClock.TimeElapsed;
                    findClock.TimeRemaining = jam.JamClock.TimeRemaining;
                    findClock.LastModified = DateTime.UtcNow;
                    findClock.Game = gDb;
                    db.SaveChanges();
                }
                else if (jam.JamClock != null && jam.JamClock.StartTime != new DateTime())
                {
                    GameStopwatch stop = new GameStopwatch();
                    stop.Created = DateTime.UtcNow;
                    stop.IsClockAtZero = jam.JamClock.IsClockAtZero == true ? 1 : 0;
                    stop.IsRunning = jam.JamClock.IsRunning == true ? 1 : 0;
                    stop.Length = jam.JamClock.TimerLength;
                    if (jam.JamClock.StartTime != new DateTime())
                        stop.StartDateTime = jam.JamClock.StartTime;
                    stop.StopwatchForId = game.GameId;
                    stop.TimeElapsed = jam.JamClock.TimeElapsed;
                    stop.TimeRemaining = jam.JamClock.TimeRemaining;
                    stop.JamNumber = jam.JamNumber;
                    stop.JamId = jam.JamId;
                    stop.Type = (int)StopWatchTypeEnum.JamClock;
                    stop.Game = gDb;
                    db.GameStopWatch.Add(stop);
                    db.SaveChanges();
                }
                c += db.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return c;
        }


    }
}
