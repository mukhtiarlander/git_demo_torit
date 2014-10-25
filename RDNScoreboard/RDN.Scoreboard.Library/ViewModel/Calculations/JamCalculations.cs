using RDN.Utilities.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.ViewModel.Positions.Enums;
using Scoreboard.Library.Static.Enums;

namespace Scoreboard.Library.ViewModel.Calculations
{
    public class JamCalculations
    {
        private static decimal CalculatePointsPerJam(decimal points, decimal jams)
        {
            return points / jams;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skaterId"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        public static decimal jammerPointsPerJam(Guid skaterId, TeamNumberEnum team)
        {
            try
            {

                decimal jams = jamCount(team, GamePositionEnum.J, skaterId);
                decimal points = pointsFor(team, GamePositionEnum.J, skaterId);

                if (jams > 0)
                    return CalculatePointsPerJam(points, jams);
                else
                    return 0;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return -1;
        }
        public static decimal blockerPointsPerJam(Guid skaterId, TeamNumberEnum team, GamePositionEnum position)
        {
            try
            {
                decimal jams = jamCount(team, position, skaterId);
                decimal points = pointsFor(team, position, skaterId);
                decimal pointsAgainst = pointsConceded(team, position, skaterId);
                if (jams == 0)
                    return 0;
                else
                    return ((points - pointsAgainst) / jams);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static int jamCount(TeamNumberEnum team, GamePositionEnum position, Guid skaterId)
        {
            try
            {
                if (GameViewModel.Instance.Jams != null)
                {
                    if (position == GamePositionEnum.J && team == TeamNumberEnum.Team1)
                        return GameViewModel.Instance.Jams.Where(x => x.JammerT1 != null && x.JammerT1.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.J && team == TeamNumberEnum.Team2)
                        return GameViewModel.Instance.Jams.Where(x => x.JammerT2 != null && x.JammerT2.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.P && team == TeamNumberEnum.Team1)
                        return GameViewModel.Instance.Jams.Where(x => x.PivotT1 != null && x.PivotT1.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.P && team == TeamNumberEnum.Team2)
                        return GameViewModel.Instance.Jams.Where(x => x.PivotT2 != null && x.PivotT2.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B1 && team == TeamNumberEnum.Team1)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker1T1 != null && x.Blocker1T1.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B1 && team == TeamNumberEnum.Team2)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker1T2 != null && x.Blocker1T2.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B2 && team == TeamNumberEnum.Team1)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker2T1 != null && x.Blocker2T1.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B2 && team == TeamNumberEnum.Team2)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker2T2 != null && x.Blocker2T2.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B3 && team == TeamNumberEnum.Team1)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker3T1 != null && x.Blocker3T1.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B3 && team == TeamNumberEnum.Team2)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker3T2 != null && x.Blocker3T2.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B4 && team == TeamNumberEnum.Team1)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker4T1 != null && x.Blocker4T1.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B4 && team == TeamNumberEnum.Team2)
                        return GameViewModel.Instance.Jams.Where(x => x.Blocker4T2 != null && x.Blocker4T2.SkaterId == skaterId).Count();
                    else if (position == GamePositionEnum.B)
                        return jamCount(team, GamePositionEnum.B1, skaterId) + jamCount(team, GamePositionEnum.B2, skaterId) + jamCount(team, GamePositionEnum.B3, skaterId) + jamCount(team, GamePositionEnum.B4, skaterId);
                    else if (position == GamePositionEnum.L)
                        return jamCount(team, GamePositionEnum.B1, skaterId) + jamCount(team, GamePositionEnum.B2, skaterId) + jamCount(team, GamePositionEnum.B3, skaterId) + jamCount(team, GamePositionEnum.B4, skaterId) + jamCount(team, GamePositionEnum.P, skaterId);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static int pointsFor(TeamNumberEnum team, GamePositionEnum position, Guid skaterId)
        {
            try
            {
                if (position == GamePositionEnum.J && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.JammerT1 != null && x.JammerT1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.J && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.JammerT2 != null && x.JammerT2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                }
                else if (position == GamePositionEnum.P && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.PivotT1 != null && x.PivotT1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.P && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.PivotT2 != null && x.PivotT2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                }
                else if (position == GamePositionEnum.B1 && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker1T1 != null && x.Blocker1T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.B1 && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker1T2 != null && x.Blocker1T2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                }
                else if (position == GamePositionEnum.B2 && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker2T1 != null && x.Blocker2T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.B2 && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker2T2 != null && x.Blocker2T2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                }
                else if (position == GamePositionEnum.B3 && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker3T1 != null && x.Blocker3T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.B3 && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker3T1 != null && x.Blocker3T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.B4 && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker4T1 != null && x.Blocker4T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.B4 && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker4T1 != null && x.Blocker4T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.B)
                    return JamCalculations.pointsFor(team, GamePositionEnum.B1, skaterId) + JamCalculations.pointsFor(team, GamePositionEnum.B2, skaterId) + JamCalculations.pointsFor(team, GamePositionEnum.B3, skaterId) + JamCalculations.pointsFor(team, GamePositionEnum.B4, skaterId);
                else if (position == GamePositionEnum.L)
                    return JamCalculations.pointsFor(team, GamePositionEnum.B1, skaterId) + JamCalculations.pointsFor(team, GamePositionEnum.B2, skaterId) + JamCalculations.pointsFor(team, GamePositionEnum.B3, skaterId) + JamCalculations.pointsFor(team, GamePositionEnum.B4, skaterId) + JamCalculations.pointsFor(team, GamePositionEnum.P, skaterId);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static int pointsConceded(TeamNumberEnum team, GamePositionEnum position, Guid skaterId)
        {
            try
            {
                if (position == GamePositionEnum.J && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.JammerT1 != null && x.JammerT1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.J && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.JammerT2 != null && x.JammerT2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                }
                else if (position == GamePositionEnum.P && team == TeamNumberEnum.Team1)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.PivotT1 != null && x.PivotT1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                }
                else if (position == GamePositionEnum.P && team == TeamNumberEnum.Team2)
                {
                    return GameViewModel.Instance.Jams.Where(x => x.PivotT2 != null && x.PivotT2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                }
                else if (position == GamePositionEnum.B1 && team == TeamNumberEnum.Team1)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker1T1 != null && x.Blocker1T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                else if (position == GamePositionEnum.B1 && team == TeamNumberEnum.Team2)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker1T2 != null && x.Blocker1T2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                else if (position == GamePositionEnum.B2 && team == TeamNumberEnum.Team1)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker2T1 != null && x.Blocker2T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                else if (position == GamePositionEnum.B2 && team == TeamNumberEnum.Team2)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker2T2 != null && x.Blocker2T2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                else if (position == GamePositionEnum.B3 && team == TeamNumberEnum.Team1)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker3T1 != null && x.Blocker3T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                else if (position == GamePositionEnum.B3 && team == TeamNumberEnum.Team2)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker3T2 != null && x.Blocker3T2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                else if (position == GamePositionEnum.B4 && team == TeamNumberEnum.Team1)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker4T1 != null && x.Blocker4T1.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT1);
                else if (position == GamePositionEnum.B4 && team == TeamNumberEnum.Team2)
                    return GameViewModel.Instance.Jams.Where(x => x.Blocker4T2 != null && x.Blocker4T2.SkaterId == skaterId).Sum(x => x.TotalPointsForJamT2);
                else if (position == GamePositionEnum.B)
                    return JamCalculations.pointsConceded(team, GamePositionEnum.B1, skaterId) + JamCalculations.pointsConceded(team, GamePositionEnum.B2, skaterId) + JamCalculations.pointsConceded(team, GamePositionEnum.B3, skaterId) + JamCalculations.pointsConceded(team, GamePositionEnum.B4, skaterId);
                else if (position == GamePositionEnum.L)
                    return JamCalculations.pointsConceded(team, GamePositionEnum.B1, skaterId) + JamCalculations.pointsConceded(team, GamePositionEnum.B2, skaterId) + JamCalculations.pointsConceded(team, GamePositionEnum.B3, skaterId) + JamCalculations.pointsConceded(team, GamePositionEnum.B4, skaterId) + JamCalculations.pointsConceded(team, GamePositionEnum.P, skaterId);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static decimal trackMinutes(TeamNumberEnum team, GamePositionEnum position, Guid skaterId)
        {
            try
            {
                decimal minutes = 0;
                if (team == TeamNumberEnum.Team1 && position == GamePositionEnum.J)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.JammerT1 != null && x.JammerT1.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team2 && position == GamePositionEnum.J)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.JammerT2 != null && x.JammerT2.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team1 && position == GamePositionEnum.P)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.PivotT1 != null && x.PivotT1.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team2 && position == GamePositionEnum.P)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.PivotT2 != null && x.PivotT2.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team1 && position == GamePositionEnum.B1)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker1T1 != null && x.Blocker1T1.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team2 && position == GamePositionEnum.B1)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker1T2 != null && x.Blocker1T2.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team1 && position == GamePositionEnum.B2)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker2T1 != null && x.Blocker2T1.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team2 && position == GamePositionEnum.B2)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker2T2 != null && x.Blocker2T2.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team1 && position == GamePositionEnum.B1)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker1T1 != null && x.Blocker1T1.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team2 && position == GamePositionEnum.B1)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker1T2 != null && x.Blocker1T2.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team1 && position == GamePositionEnum.B3)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker3T1 != null && x.Blocker3T1.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team2 && position == GamePositionEnum.B3)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker3T2 != null && x.Blocker3T2.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team1 && position == GamePositionEnum.B4)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker4T1 != null && x.Blocker4T1.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (team == TeamNumberEnum.Team2 && position == GamePositionEnum.B4)
                {
                    Guid[] jams;
                    long[] timeStart;
                    jams = GameViewModel.Instance.Jams.Where(x => x.Blocker4T2 != null && x.Blocker4T2.SkaterId == skaterId).Select(x => x.JamId).ToArray();
                    foreach (Guid jamID in jams)
                    {
                        timeStart = GameViewModel.Instance.Jams.Where(x => x.JamId == jamID).Select(x => x.JamClock.TimeElapsed).ToArray();
                        minutes += timeStart[0];
                    }
                    return (minutes / 60000);
                }
                else if (position == GamePositionEnum.B)
                    return trackMinutes(team, GamePositionEnum.B1, skaterId) + trackMinutes(team, GamePositionEnum.B2, skaterId) + trackMinutes(team, GamePositionEnum.B3, skaterId) + trackMinutes(team, GamePositionEnum.B4, skaterId);
                else if (position == GamePositionEnum.L)
                    return trackMinutes(team, GamePositionEnum.P, skaterId) + trackMinutes(team, GamePositionEnum.B, skaterId);
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static decimal jammerPointsPerMinute(Guid skaterId, GamePositionEnum position, TeamNumberEnum team)
        {
            try
            {
                decimal minutes = trackMinutes(team, position, skaterId);
                decimal points = pointsFor(team, position, skaterId);
                if (minutes > 0)
                    return points / minutes;
                else
                    return 0;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return -1;
        }
        public static int leadJams(TeamNumberEnum teamOneOrTwo, GamePositionEnum position, Guid skaterId)
        {
            try
            {
                if (teamOneOrTwo == TeamNumberEnum.Team1 && position == GamePositionEnum.J)
                    return GameViewModel.Instance.Jams.Where(x => x.JammerT1 != null && x.JammerT1.SkaterId == skaterId && x.TeamLeadingJam == Static.Enums.TeamNumberEnum.Team1).Count();
                else if (teamOneOrTwo == TeamNumberEnum.Team2 && position == GamePositionEnum.J)
                    return GameViewModel.Instance.Jams.Where(x => x.JammerT2 != null && x.JammerT2.SkaterId == skaterId && x.TeamLeadingJam == Static.Enums.TeamNumberEnum.Team2).Count();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static decimal leadJamPc(TeamNumberEnum teamOneOrTwo, GamePositionEnum position, Guid skaterId)
        {
            try
            {
                decimal LJpc;
                int jams = jamCount(teamOneOrTwo, position, skaterId);
                int leads = leadJams(teamOneOrTwo, position, skaterId);
                LJpc = leads * 100 / jams;
                return LJpc;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static int rosterJamCount(TeamNumberEnum teamOneOrTwo, GamePositionEnum position, int rosterNum)
        {
            try
            {
                Guid[] rostah = { };
                if (teamOneOrTwo == TeamNumberEnum.Team1)
                {
                    rostah = GameViewModel.Instance.Team1.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    return jamCount(teamOneOrTwo, position, rostah[rosterNum - 1]);
                }
                else if (teamOneOrTwo == TeamNumberEnum.Team2)
                {
                    rostah = GameViewModel.Instance.Team2.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    return jamCount(teamOneOrTwo, position, rostah[rosterNum - 1]);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return 0;
        }
        public static string rosterPoints(TeamNumberEnum teamOneOrTwo, GamePositionEnum position, int rosterNum)
        {
            try
            {
                Guid[] rostah = { };
                int scoah = 0;
                if (teamOneOrTwo == TeamNumberEnum.Team1)
                {
                    rostah = GameViewModel.Instance.Team1.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    scoah = pointsFor(teamOneOrTwo, position, rostah[rosterNum - 1]);
                }
                else if (teamOneOrTwo == TeamNumberEnum.Team2)
                {
                    rostah = GameViewModel.Instance.Team2.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    scoah = pointsFor(teamOneOrTwo, position, rostah[rosterNum - 1]);
                }
                if (scoah == 0)
                    return null;
                else
                    return scoah.ToString();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return null;
        }
        public static string rosterPointsPerMinute(TeamNumberEnum team, GamePositionEnum position, int rosterNum)
        {
            try
            {
                Guid[] rostah = { };
                decimal scoah = 0;
                if (team == TeamNumberEnum.Team1)
                {
                    rostah = GameViewModel.Instance.Team1.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    scoah = jammerPointsPerMinute(rostah[rosterNum - 1], position, team);
                }
                else if (team == TeamNumberEnum.Team2)
                {
                    rostah = GameViewModel.Instance.Team2.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    scoah = jammerPointsPerMinute(rostah[rosterNum - 1], position, team);
                }
                return scoah.ToString("N2");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return null;
        }
        public static string rosterPlusMinusPerJam(TeamNumberEnum teamOneOrTwo, GamePositionEnum position, int rosterNum)
        {
            try
            {
                Guid[] rostah = { };
                decimal scoah = 0;
                if (teamOneOrTwo == TeamNumberEnum.Team1)
                {
                    rostah = GameViewModel.Instance.Team1.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    scoah = blockerPointsPerJam(rostah[rosterNum - 1], teamOneOrTwo, position);
                }
                else if (teamOneOrTwo == TeamNumberEnum.Team2)
                {
                    rostah = GameViewModel.Instance.Team2.TeamMembers.OrderBy(x => x.SkaterName).Select(x => x.SkaterId).ToArray();
                    scoah = blockerPointsPerJam(rostah[rosterNum - 1], teamOneOrTwo, position);
                }
                return scoah.ToString("N2");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return null;
        }
        public static string penaltyCountAnnouncer(TeamNumberEnum teamOneOrTwo, Guid skater)
        {
            try
            {
                int pens = 0;
                if (teamOneOrTwo == TeamNumberEnum.Team1)
                    pens = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyAgainstMember != null && x.PenaltyAgainstMember.SkaterId == skater).Count();
                else if (teamOneOrTwo == TeamNumberEnum.Team2)
                    pens = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyAgainstMember != null && x.PenaltyAgainstMember.SkaterId == skater).Count();
                return pens.ToString();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), ErrorGroupEnum.UI);
            }
            return null;
        }
    }
}
