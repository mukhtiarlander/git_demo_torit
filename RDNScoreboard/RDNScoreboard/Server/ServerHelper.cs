using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.Static;

using System.Windows;
using Scoreboard.Library.ViewModel.Members;
using Scoreboard.Library.ViewModel.Calculations;
using Scoreboard.Library.ViewModel.Positions.Enums;
using Scoreboard.Library.ViewModel.Jam;
using Scoreboard.Library.MiniServer.Json;
using System.Web.Script.Serialization;
using Scoreboard.Library.MiniServer.Json.Teams;
using Scoreboard.Library.MiniServer.Json.Announcer;
using Scoreboard.Library.MiniServer.Json.Mobile;
using RDN.Utilities.Util;
//using Scoreboard.Library.ViewModel.Calculations;

namespace RDNScoreboard.Server
{
    public class ServerHelper
    {
        public static string LoadMainControlScreen()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                sb.Append("\"fourbyThree\":\"");
                sb.Append(WebServer.Instance.VideoOverlayAddress4x3 + addUrlParameters());
                sb.Append("\",");
                sb.Append("\"sixteenByNine\":\"");
                sb.Append(WebServer.Instance.VideoOverlayAddress16x9 + addUrlParameters());
                sb.Append("\"");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            sb.Append("}");

            return sb.ToString();
        }
        public static string addUrlParameters()
        {
            string parameters = String.Empty;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TopTeam1Color))
                parameters += "&t1color=" + PolicyViewModel.Instance.VideoOverlay.TopTeam1Color;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color))
                parameters += "&b1color=" + PolicyViewModel.Instance.VideoOverlay.BottomTeam1Color;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.FontTeam1Color))
                parameters += "&f1color=" + PolicyViewModel.Instance.VideoOverlay.FontTeam1Color;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TopTeam2Color))
                parameters += "&t2color=" + PolicyViewModel.Instance.VideoOverlay.TopTeam2Color;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color))
                parameters += "&b2color=" + PolicyViewModel.Instance.VideoOverlay.BottomTeam2Color;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.FontTeam2Color))
                parameters += "&f2color=" + PolicyViewModel.Instance.VideoOverlay.FontTeam2Color;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.OverlayColor))
                parameters += "&modcolor=" + PolicyViewModel.Instance.VideoOverlay.OverlayColor;
            if (PolicyViewModel.Instance.VideoOverlay.IsOverlayTransparent)
                parameters += "&modtrans=.4";
            if (!PolicyViewModel.Instance.VideoOverlay.LogoOnOff)
                parameters += "&logo=off";
            if (PolicyViewModel.Instance.VideoOverlay.ScoresOnTop)
                parameters += "&mod=top";
            if (PolicyViewModel.Instance.VideoOverlay.IsBottomRowOn)
                parameters += "&brow=on";
            if (PolicyViewModel.Instance.VideoOverlay.ShowJamScore)
                parameters += "&sjam=on";
            if (PolicyViewModel.Instance.VideoOverlay.TurnOffTimeOuts)
                parameters += "&sto=off";
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TimeOutColor))
                parameters += "&tocolor=" + PolicyViewModel.Instance.VideoOverlay.TimeOutColor;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.GreenScreen))
                parameters += "&gscreen=" + PolicyViewModel.Instance.VideoOverlay.GreenScreen;
            if (!string.IsNullOrEmpty(PolicyViewModel.Instance.VideoOverlay.TextColor))
                parameters += "&tecolor=" + PolicyViewModel.Instance.VideoOverlay.TextColor;

            parameters += "&tsize=" + PolicyViewModel.Instance.VideoOverlay.TextSizePosition;
            parameters += "&vertical=" + PolicyViewModel.Instance.VideoOverlay.VerticalPosition;
            return parameters;
        }

        public static void AddPenalty(Guid playerId, int teamNumber, int jamNumber, int penaltyId, int minorMajor, Guid jamId)
        {
            try
            {
                TeamMembersViewModel skater = null;

                if (teamNumber == 1)
                {
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.PenaltiesForTeam1.Add(new PenaltyViewModel((PenaltiesEnum)Enum.Parse(typeof(PenaltiesEnum), penaltyId.ToString()), skater, jamNumber, jamId, (PenaltyScaleEnum)Enum.Parse(typeof(PenaltyScaleEnum), minorMajor.ToString())));
                }
                else if (teamNumber == 2)
                {
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.PenaltiesForTeam2.Add(new PenaltyViewModel((PenaltiesEnum)Enum.Parse(typeof(PenaltiesEnum), penaltyId.ToString()), skater, jamNumber, jamId, (PenaltyScaleEnum)Enum.Parse(typeof(PenaltyScaleEnum), minorMajor.ToString())));
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public static string RemovePenalty(Guid penaltyId)
        {
            var pens = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyId == penaltyId).FirstOrDefault();
            if (pens != null)
            {
                GameViewModel.Instance.PenaltiesForTeam1.Remove(pens);
            }
            else
            {
                pens = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyId == penaltyId).FirstOrDefault();
                GameViewModel.Instance.PenaltiesForTeam2.Remove(pens);
            }
            return "{\"result\": \"True\"}";
        }

        public static string GetPenalties(Guid playerId, int teamNumber)
        {
            StringBuilder sb = new StringBuilder();
            List<PenaltyViewModel> pens = new List<PenaltyViewModel>();
            sb.Append("{");
            try
            {
                if (teamNumber == 1 && GameViewModel.Instance.PenaltiesForTeam1 != null)
                    pens = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyAgainstMember != null).Where(x => x.PenaltyAgainstMember.SkaterId != null).Where(x => x.PenaltyAgainstMember.SkaterId == playerId).ToList();
                else if (teamNumber == 2 && GameViewModel.Instance.PenaltiesForTeam2 != null)
                    pens = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyAgainstMember != null).Where(x => x.PenaltyAgainstMember.SkaterId != null).Where(x => x.PenaltyAgainstMember.SkaterId == playerId).ToList();
                if (pens.Count > 0)
                {
                    sb.Append("\"penalties\": [");
                    for (int i = 0; i < pens.Count; i++)
                    {
                        sb.Append("{\"id\":\"");
                        sb.Append(pens[i].PenaltyId);
                        sb.Append("\",");
                        sb.Append("\"name\":\"");
                        sb.Append(RDN.Utilities.Enums.EnumExt.ToFreindlyName(pens[i].PenaltyType));
                        if ((i + 1) == pens.Count)
                            sb.Append("\"}");
                        else
                            sb.Append("\"},");
                    }
                    sb.Append("]");
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            sb.Append("}");
            return sb.ToString();
        }

        public static string GetAnnouncerPage()
        {
            AnnouncerJson an = new AnnouncerJson();

            try
            {
                if (GameViewModel.Instance.CurrentJam != null)
                {
                    if (GameViewModel.Instance.Team1 != null)
                        an.team1Name = GameViewModel.Instance.Team1.TeamName;

                    if (GameViewModel.Instance.Team2 != null)
                        an.team2Name = GameViewModel.Instance.Team2.TeamName;

                    if (GameViewModel.Instance.CurrentJam.JammerT1 != null)
                        an.JammerT1 = CalculateJammerStats(an, TeamNumberEnum.Team1, GameViewModel.Instance.CurrentJam.JammerT1.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.PivotT1 != null)
                        an.PivotT1 = CalculateBlockerStats(an, TeamNumberEnum.Team1, GamePositionEnum.P, GameViewModel.Instance.CurrentJam.PivotT1.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.Blocker1T1 != null)
                        an.Blocker1T1 = CalculateBlockerStats(an, TeamNumberEnum.Team1, GamePositionEnum.B1, GameViewModel.Instance.CurrentJam.Blocker1T1.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.Blocker2T1 != null)
                        an.Blocker2T1 = CalculateBlockerStats(an, TeamNumberEnum.Team1, GamePositionEnum.B2, GameViewModel.Instance.CurrentJam.Blocker2T1.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.Blocker3T1 != null)
                        an.Blocker3T1 = CalculateBlockerStats(an, TeamNumberEnum.Team1, GamePositionEnum.B3, GameViewModel.Instance.CurrentJam.Blocker3T1.SkaterId);
                    if (GameViewModel.Instance.CurrentJam.Blocker4T1 != null)
                        an.Blocker4T1 = CalculateBlockerStats(an, TeamNumberEnum.Team1, GamePositionEnum.B4, GameViewModel.Instance.CurrentJam.Blocker4T1.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.JammerT2 != null)
                        an.JammerT2 = CalculateJammerStats(an, TeamNumberEnum.Team2, GameViewModel.Instance.CurrentJam.JammerT2.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.PivotT2 != null)
                        an.PivotT2 = CalculateBlockerStats(an, TeamNumberEnum.Team2, GamePositionEnum.P, GameViewModel.Instance.CurrentJam.PivotT2.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.Blocker1T2 != null)
                        an.Blocker1T2 = CalculateBlockerStats(an, TeamNumberEnum.Team2, GamePositionEnum.B1, GameViewModel.Instance.CurrentJam.Blocker1T2.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.Blocker2T2 != null)
                        an.Blocker2T2 = CalculateBlockerStats(an, TeamNumberEnum.Team2, GamePositionEnum.B2, GameViewModel.Instance.CurrentJam.Blocker2T2.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.Blocker3T2 != null)
                        an.Blocker3T2 = CalculateBlockerStats(an, TeamNumberEnum.Team2, GamePositionEnum.B3, GameViewModel.Instance.CurrentJam.Blocker3T2.SkaterId);

                    if (GameViewModel.Instance.CurrentJam.Blocker4T2 != null)
                        an.Blocker1T2 = CalculateBlockerStats(an, TeamNumberEnum.Team2, GamePositionEnum.B4, GameViewModel.Instance.CurrentJam.Blocker4T2.SkaterId);

                    //two for loops to set out the rosters

                    var rostah = GameViewModel.Instance.Team1.TeamMembers.OrderBy(x => x.SkaterName);
                    an.PlayerStatsT1 = new List<LivePlayerStat>();
                    foreach (var member in rostah)
                    {
                        var p = CalculateLivePlayerStats(member, TeamNumberEnum.Team1);
                        an.PlayerStatsT1.Add(p);
                    }

                    rostah = GameViewModel.Instance.Team2.TeamMembers.OrderBy(x => x.SkaterName);
                    an.PlayerStatsT2 = new List<LivePlayerStat>();
                    foreach (var member in rostah)
                    {
                        {
                            var p = CalculateLivePlayerStats(member, TeamNumberEnum.Team2);
                            an.PlayerStatsT2.Add(p);
                        }
                    }
                    an.currentJam = GameViewModel.Instance.CurrentJam.JamNumber;
                    an.currentJamId = GameViewModel.Instance.CurrentJam.JamId;
                    an.totalJams = GameViewModel.Instance.Jams.Count + 1;
                }
                else
                {
                    if (GameViewModel.Instance.CurrentJam != null)
                    {
                        an.currentJam = 0;
                        an.currentJamId = new Guid();
                        an.totalJams = 0;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            JavaScriptSerializer s = new JavaScriptSerializer();

            return s.Serialize(an);
        }

        private static LivePlayerStat CalculateLivePlayerStats(TeamMembersViewModel member, TeamNumberEnum team)
        {
            var p = new LivePlayerStat();
            try
            {

                p.rosterName = member.SkaterName;
                p.rosterNumber = member.SkaterNumber;
                int jams = JamCalculations.jamCount(team, GamePositionEnum.J, member.SkaterId);
                if (jams > 0)
                {
                    p.rosterJammerJams = jams;
                    p.rosterJammerPts = JamCalculations.pointsFor(team, GamePositionEnum.J, member.SkaterId);
                }
                int jam = JamCalculations.jamCount(team, GamePositionEnum.L, member.SkaterId);
                if (jam > 0)
                {
                    p.rosterBlockerJams = jams;
                    p.rosterBlockerPointsPer = JamCalculations.blockerPointsPerJam(member.SkaterId, team, GamePositionEnum.L).ToString("N1");
                }
                if (jam > 0 || jams > 0)
                {
                    p.rosterPens = JamCalculations.penaltyCountAnnouncer(team, member.SkaterId);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return p;
        }

        private static MemberForAnnouncerJson CalculateBlockerStats(AnnouncerJson an, TeamNumberEnum team, GamePositionEnum position, Guid skaterId)
        {
            var blocker = new MemberForAnnouncerJson();
            try
            {
                blocker.Name = NameFunctions.skaterName(team, position);
                blocker.Number = NameFunctions.skaterNumber(team, position);
                blocker.Jams = JamCalculations.jamCount(team, GamePositionEnum.L, skaterId);
                int pointsFor = JamCalculations.pointsFor(team, GamePositionEnum.L, skaterId);
                blocker.PointsPerJam = JamCalculations.blockerPointsPerJam(skaterId, team, GamePositionEnum.L).ToString("N2");
                blocker.Points = pointsFor;
                blocker.PointsPerMinute = "0";
                blocker.LeadJamPc = "0";
            }

            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return blocker;
        }

        private static MemberForAnnouncerJson CalculateJammerStats(AnnouncerJson an, TeamNumberEnum team, Guid skaterId)
        {
            var jammer = new MemberForAnnouncerJson();
            try
            {
                jammer.Name = NameFunctions.skaterName(team, GamePositionEnum.J);
                jammer.Number = NameFunctions.skaterNumber(team, GamePositionEnum.J);
                jammer.PointsPerJam = JamCalculations.jammerPointsPerJam(skaterId, team).ToString("N2");
                jammer.Jams = JamCalculations.jamCount(team, GamePositionEnum.J, skaterId);
                jammer.Points = JamCalculations.pointsFor(team, GamePositionEnum.J, skaterId);
                jammer.PointsPerMinute = JamCalculations.jammerPointsPerMinute(skaterId, GamePositionEnum.J, team).ToString("N2");
                jammer.LeadJamPc = JamCalculations.leadJams(team, GamePositionEnum.J, skaterId).ToString("N1");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return jammer;
        }

        public static string GetJamNumber()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            try
            {
                if (GameViewModel.Instance.CurrentJam != null)
                {
                    sb.Append("\"currentJam\":\"");
                    sb.Append(GameViewModel.Instance.CurrentJam.JamNumber);
                    sb.Append("\",");
                    sb.Append("\"currentJamId\":\"");
                    sb.Append(GameViewModel.Instance.CurrentJam.JamId);
                    sb.Append("\",");
                    sb.Append("\"totalJams\":");
                    sb.Append(GameViewModel.Instance.Jams.Count);
                    sb.Append("");
                }
                else
                {
                    if (GameViewModel.Instance.CurrentJam != null)
                    {
                        sb.Append("\"currentJam\":\"");
                        sb.Append(0);
                        sb.Append("\",");
                        sb.Append("\"currentJamId\":\"");
                        sb.Append(new Guid());
                        sb.Append("\",");
                        sb.Append("\"totalJams\":");
                        sb.Append(0);
                        sb.Append("");
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            sb.Append("}");

            return sb.ToString();
        }

        public static string GetBlocks(Guid playerId, int teamNumber, Guid jamId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                if (teamNumber == 1)
                {
                    sb.Append("\"totalBlocks\":");
                    sb.Append(GameViewModel.Instance.BlocksForTeam1.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == playerId).Count());
                    sb.Append(",");
                    sb.Append("\"blocksForJam\":");
                    sb.Append(GameViewModel.Instance.BlocksForTeam1.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == playerId).Where(x => x.JamId == jamId).Count());
                }
                else if (teamNumber == 2)
                {
                    sb.Append("\"totalBlocks\":");
                    sb.Append(GameViewModel.Instance.BlocksForTeam2.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == playerId).Count());
                    sb.Append(",");
                    sb.Append("\"blocksForJam\":");
                    sb.Append(GameViewModel.Instance.BlocksForTeam2.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == playerId).Where(x => x.JamId == jamId).Count());
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            sb.Append("}");

            return sb.ToString();
        }

        public static string GetScore(int teamNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                if (teamNumber == 1)
                {
                    sb.Append("\"totalScores\":");
                    sb.Append(GameViewModel.Instance.CurrentTeam1Score);
                }
                else if (teamNumber == 2)
                {
                    sb.Append("\"totalScores\":");
                    sb.Append(GameViewModel.Instance.CurrentTeam1Score);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

            sb.Append("}");

            return sb.ToString();
        }
        public static string GetScore(Guid playerId, Guid jamId, int teamNumber)
        {
            ScoresJson sc = new ScoresJson();

            try
            {
                if (teamNumber == 1)
                {
                    sc.totalScores = GameViewModel.Instance.ScoresTeam1.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == playerId).Count();
                    sc.scoreForJam = GameViewModel.Instance.ScoresTeam1.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == playerId).Where(x => x.JamId == jamId).Count();
                }
                else if (teamNumber == 2)
                {
                    sc.totalScores = GameViewModel.Instance.ScoresTeam2.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == playerId).Count();
                    sc.scoreForJam = GameViewModel.Instance.ScoresTeam2.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == playerId).Where(x => x.JamId == jamId).Count();
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

            JavaScriptSerializer s = new JavaScriptSerializer();

            return s.Serialize(sc);
        }

        public static string GetAssists(Guid playerId, int jamNumber, int teamNumber, Guid jamId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                if (teamNumber == 1)
                {
                    sb.Append("\"totalAssists\":");
                    sb.Append(GameViewModel.Instance.AssistsForTeam1.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == playerId).Count());
                    sb.Append(",");
                    sb.Append("\"assistsForJam\":");
                    sb.Append(GameViewModel.Instance.AssistsForTeam1.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == playerId).Where(x => x.JamId == jamId).Count());
                }
                else if (teamNumber == 2)
                {
                    sb.Append("\"totalAssists\":");
                    sb.Append(GameViewModel.Instance.AssistsForTeam2.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == playerId).Count());
                    sb.Append(",");
                    sb.Append("\"assistsForJam\":");
                    sb.Append(GameViewModel.Instance.AssistsForTeam2.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == playerId).Where(x => x.JamId == jamId).Count());
                }

                sb.Append("}");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return sb.ToString();
        }

        public static void AddAssist(Guid playerId, int teamNumber, int jamNumber, Guid jamId)
        {
            try
            {
                TeamMembersViewModel skater = null;

                if (teamNumber == 1)
                {
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.AssistsForTeam1.Add(new AssistViewModel(skater, jamNumber, jamId));
                }
                else if (teamNumber == 2)
                {
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.AssistsForTeam2.Add(new AssistViewModel(skater, jamNumber, jamId));
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public static void RemoveAssist(Guid playerId, int teamNumber)
        {
            try
            {
                if (teamNumber == 1)
                {
                    var ass = GameViewModel.Instance.AssistsForTeam1.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == playerId).LastOrDefault();
                    GameViewModel.Instance.AssistsForTeam1.Remove(ass);
                }
                else if (teamNumber == 2)
                {
                    var ass = GameViewModel.Instance.AssistsForTeam2.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == playerId).LastOrDefault();
                    GameViewModel.Instance.AssistsForTeam2.Remove(ass);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }


        public static void AddBlock(Guid playerId, int teamNumber, int jamNumber, Guid jamId)
        {
            try
            {
                TeamMembersViewModel skater = null;

                if (teamNumber == 1)
                {
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.BlocksForTeam1.Add(new BlockViewModel(skater, jamNumber, jamId));
                }
                else if (teamNumber == 2)
                {
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.BlocksForTeam2.Add(new BlockViewModel(skater, jamNumber, jamId));
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public static void RemoveBlock(Guid playerId, int teamNumber)
        {
            try
            {
                if (teamNumber == 1)
                {
                    var ass = GameViewModel.Instance.BlocksForTeam1.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == playerId).LastOrDefault();
                    GameViewModel.Instance.BlocksForTeam1.Remove(ass);
                }
                else if (teamNumber == 2)
                {
                    var ass = GameViewModel.Instance.BlocksForTeam2.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == playerId).LastOrDefault();
                    GameViewModel.Instance.BlocksForTeam2.Remove(ass);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public static void ScoringLoaded()
        {
            try
            {
                GameViewModel.Instance.UsingServerScoring = true;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        /// <summary>
        /// this add score gets controls from the line up page.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="teamNumber"></param>
        /// <param name="jamId"></param>
        /// <param name="jamNumber"></param>
        public static void AddScore(Guid playerId, int teamNumber, Guid jamId, int jamNumber, int points)
        {
            try
            {
                TeamMembersViewModel skater = null;
                if (teamNumber == 1)
                {
                    if (PolicyViewModel.Instance.LineUpTrackerControlsScore)
                    {
                        GameViewModel.Instance.CurrentTeam1Score += points;
                        GameViewModel.Instance.CurrentTeam1JamScore += points;
                        //add the points to the totoal points for the jam.
                    }
                    if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId && GameViewModel.Instance.CurrentJam.TotalPointsForJamT1 + points > -1)
                        GameViewModel.Instance.CurrentJam.TotalPointsForJamT1 += points;
                    else if (GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault() != null)
                    {
                        GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault().TotalPointsForJamT1 += points;
                    }
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.ScoresTeam1.Add(new ScoreViewModel(new TeamMembersViewModel() { SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, IsPivot = skater.IsPivot, IsJammer = skater.IsJammer, IsLeadJammer = skater.IsLeadJammer, LostLeadJammerEligibility = skater.LostLeadJammerEligibility, SkaterName = skater.SkaterName, SkaterNumber = skater.SkaterNumber }, points, jamId, jamNumber));
                }
                else if (teamNumber == 2)
                {
                    if (PolicyViewModel.Instance.LineUpTrackerControlsScore)
                    {
                        GameViewModel.Instance.CurrentTeam2Score += points;
                        GameViewModel.Instance.CurrentTeam2JamScore += points;

                    }
                    if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId && GameViewModel.Instance.CurrentJam.TotalPointsForJamT2 + points > -1)
                        GameViewModel.Instance.CurrentJam.TotalPointsForJamT2 += points;
                    else if (GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault() != null)
                    {
                        GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault().TotalPointsForJamT2 += points;
                    }
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    GameViewModel.Instance.ScoresTeam2.Add(new ScoreViewModel(new TeamMembersViewModel() { SkaterId = skater.SkaterId, SkaterLinkId = skater.SkaterLinkId, IsPivot = skater.IsPivot, IsJammer = skater.IsJammer, IsLeadJammer = skater.IsLeadJammer, LostLeadJammerEligibility = skater.LostLeadJammerEligibility, SkaterName = skater.SkaterName, SkaterNumber = skater.SkaterNumber }, points, jamId, jamNumber));
                }
                if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId)
                    GameViewModel.Instance.CurrentJam.UpdateLatestJamPassScore(playerId, points);
                else
                {
                    var jam = GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault();
                    if (jam != null)
                        jam.UpdateLatestJamPassScore(playerId, points);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public static void RemoveScore(Guid playerId, int teamNumber, Guid jamId)
        {
            try
            {
                if (teamNumber == 1)
                {
                    var ass = GameViewModel.Instance.ScoresTeam1.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == playerId && x.JamId == jamId).LastOrDefault();
                    GameViewModel.Instance.ScoresTeam1.Remove(ass);
                    GameViewModel.Instance.CurrentTeam1Score += -1;
                    GameViewModel.Instance.CurrentTeam1JamScore += -1;
                    if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId && GameViewModel.Instance.CurrentJam.TotalPointsForJamT1 + -1 > -1)
                        GameViewModel.Instance.CurrentJam.TotalPointsForJamT1 += -1;
                    else if (GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault() != null)
                    {
                        GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault().TotalPointsForJamT1 += -1;
                    }
                }
                else if (teamNumber == 2)
                {
                    var ass = GameViewModel.Instance.ScoresTeam2.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == playerId && x.JamId == jamId).LastOrDefault();
                    GameViewModel.Instance.ScoresTeam2.Remove(ass);
                    GameViewModel.Instance.CurrentTeam2Score += -1;
                    GameViewModel.Instance.CurrentTeam2JamScore += -1;
                    if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId && GameViewModel.Instance.CurrentJam.TotalPointsForJamT2 + -1 > -1)
                        GameViewModel.Instance.CurrentJam.TotalPointsForJamT2 += -1;
                    else if (GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault() != null)
                    {
                        GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault().TotalPointsForJamT2 += -1;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public static void RemoveScore(int teamNumber)
        {
            try
            {
                if (teamNumber == 1)
                {
                    if (GameViewModel.Instance.ScoresTeam1 != null && GameViewModel.Instance.ScoresTeam1.Count > 0)
                        GameViewModel.Instance.ScoresTeam1.RemoveAt(GameViewModel.Instance.ScoresTeam1.Count - 1);
                    GameViewModel.Instance.CurrentTeam1Score += -1;
                    GameViewModel.Instance.CurrentTeam1JamScore += -1;
                    if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.TotalPointsForJamT1 + -1 > -1)
                        GameViewModel.Instance.CurrentJam.TotalPointsForJamT1 += -1;
                }
                else if (teamNumber == 2)
                {
                    if (GameViewModel.Instance.ScoresTeam2 != null && GameViewModel.Instance.ScoresTeam2.Count > 0)
                        GameViewModel.Instance.ScoresTeam2.RemoveAt(GameViewModel.Instance.ScoresTeam2.Count - 1);
                    GameViewModel.Instance.CurrentTeam2Score += -1;
                    GameViewModel.Instance.CurrentTeam2JamScore += -1;
                    if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.TotalPointsForJamT2 + -1 > -1)
                        GameViewModel.Instance.CurrentJam.TotalPointsForJamT2 += -1;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public static void SetBlocker(int blockerNumber, Guid playerId, int teamNumber, Guid jamId)
        {
            try
            {
                TeamMembersViewModel skater = null;
                TeamNumberEnum team = TeamNumberEnum.Team1;

                if (teamNumber == 1)
                {
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team1;
                }
                else if (teamNumber == 2)
                {
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team2;
                }

                if (GameViewModel.Instance.CurrentJam.JamId == jamId)
                {
                    if (blockerNumber == 1)
                        GameViewModel.Instance.CurrentJam.setBlocker1(skater, team);
                    else if (blockerNumber == 2)
                        GameViewModel.Instance.CurrentJam.setBlocker2(skater, team);
                    else if (blockerNumber == 3)
                        GameViewModel.Instance.CurrentJam.setBlocker3(skater, team);
                    else if (blockerNumber == 4)
                        GameViewModel.Instance.CurrentJam.setBlocker4(skater, team);
                }
                else
                {
                    Logger.Instance.logMessage("settings Blocker for jam: " + jamId, LoggerEnum.message);
                    var jam = GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault();
                    if (jam != null)
                    {
                        if (blockerNumber == 1)
                            jam.setBlocker1(skater, team);
                        else if (blockerNumber == 2)
                            jam.setBlocker2(skater, team);
                        else if (blockerNumber == 3)
                            jam.setBlocker3(skater, team);
                        else if (blockerNumber == 4)
                            jam.setBlocker4(skater, team);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }
        public static void SetPenaltyBox(Guid playerId, int teamNumber, Guid jamId)
        {
            try
            {
                TeamMembersViewModel skater = null;
                TeamNumberEnum team = TeamNumberEnum.Team1;

                if (teamNumber == 1)
                {
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team1;
                }
                else if (teamNumber == 2)
                {
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team2;
                }

                if (GameViewModel.Instance.CurrentJam.JamId == jamId)
                {
                    GameViewModel.Instance.sendSkaterToPenaltyBox(skater, team);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        public static void SetPivot(Guid playerId, int teamNumber, Guid jamId)
        {
            try
            {
                TeamMembersViewModel skater = null;
                TeamNumberEnum team = TeamNumberEnum.Team1;

                if (teamNumber == 1)
                {
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team1;
                }
                else if (teamNumber == 2)
                {
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team2;
                }

                if (GameViewModel.Instance.CurrentJam.JamId == jamId)
                {
                    GameViewModel.Instance.CurrentJam.setPivot(skater, team);
                }
                else
                {
                    Logger.Instance.logMessage("settings pivot for jam: " + jamId, LoggerEnum.message);
                    var jam = GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault();
                    if (jam != null)
                        jam.setPivot(skater, team);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }
        public static void SetJammer(Guid playerId, int teamNumber, Guid jamId)
        {
            try
            {
                TeamMembersViewModel skater = null;
                TeamNumberEnum team = TeamNumberEnum.Team1;

                if (teamNumber == 1)
                {
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team1;
                }
                else if (teamNumber == 2)
                {
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                    team = TeamNumberEnum.Team2;
                }

                if (GameViewModel.Instance.CurrentJam.JamId == jamId)
                {
                    GameViewModel.Instance.CurrentJam.setJammer(skater, team);
                }
                else
                {
                    Logger.Instance.logMessage("settings jammer for jam: " + jamId, LoggerEnum.message);
                    var jam = GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault();
                    if (jam != null)
                        jam.setJammer(skater, team);
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
        }

        public static string GetPlayerPositions(Guid playerId, int teamNumber)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                TeamMembersViewModel skater = new TeamMembersViewModel();
                if (teamNumber == 1 && GameViewModel.Instance.Team1.TeamMembers.Count > 0)
                    skater = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();
                else if (teamNumber == 2 && GameViewModel.Instance.Team2.TeamMembers.Count > 0)
                    skater = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == playerId).FirstOrDefault();

                sb.Append("\"isJammer\":\"");
                sb.Append(skater.IsJammer);
                sb.Append("\",");
                sb.Append("\"isPivot\":\"");
                sb.Append(skater.IsPivot);
                sb.Append("\",");
                sb.Append("\"isPBox\":\"");
                sb.Append(skater.IsInBox);
                sb.Append("\",");
                sb.Append("\"isBlocker1\":\"");
                sb.Append(skater.IsBlocker1);
                sb.Append("\",");
                sb.Append("\"isBlocker2\":\"");
                sb.Append(skater.IsBlocker2);
                sb.Append("\",");
                sb.Append("\"isBlocker3\":\"");
                sb.Append(skater.IsBlocker3);
                sb.Append("\",");
                sb.Append("\"isBlocker4\":\"");
                sb.Append(skater.IsBlocker4);
                sb.Append("\"");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            sb.Append("}");

            return sb.ToString();
        }
        public static string GetTeamNames()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                if (GameViewModel.Instance.Team1.TeamName != null)
                {
                    sb.Append("\"team1Name\":\"");
                    sb.Append(GameViewModel.Instance.Team1.TeamName);
                    sb.Append("\",");
                }
                if (GameViewModel.Instance.Team2.TeamName != null)
                {
                    sb.Append("\"team2Name\":\"");
                    sb.Append(GameViewModel.Instance.Team2.TeamName);
                    sb.Append("\",");
                }
                sb.Append("\"team1Id\":\"");
                sb.Append(GameViewModel.Instance.Team1.TeamId);
                sb.Append("\",");
                sb.Append("\"team2Id\":\"");
                sb.Append(GameViewModel.Instance.Team2.TeamId);
                sb.Append("\"");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            sb.Append("}");

            return sb.ToString();
        }

        public static string GetTeam1Members()
        {

            TeamJson team = new TeamJson();

            try
            {
                team.teamId = GameViewModel.Instance.Team1.TeamId.ToString();

                if (GameViewModel.Instance.Team1.TeamName != null)
                {
                    team.teamName = GameViewModel.Instance.Team1.TeamName;
                }

                if (GameViewModel.Instance.CurrentJam != null)
                {
                    team.currentJam = GameViewModel.Instance.CurrentJam.JamNumber;
                    team.currentJamId = GameViewModel.Instance.CurrentJam.JamId.ToString();
                    team.totalJams = GameViewModel.Instance.Jams.Count;
                }

                for (int i = 0; i < GameViewModel.Instance.Team1.TeamMembers.Count; i++)
                {
                    try
                    {
                        TeamMemberJson member = new TeamMemberJson();
                        member.memberName = GameViewModel.Instance.Team1.TeamMembers[i].SkaterName;
                        member.memberId = GameViewModel.Instance.Team1.TeamMembers[i].SkaterId.ToString();
                        member.memberNumber = GameViewModel.Instance.Team1.TeamMembers[i].SkaterNumber;

                        var blocks = GameViewModel.Instance.BlocksForTeam1.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId).Count();
                        member.totalBlocks = blocks;

                        if (GameViewModel.Instance.CurrentJam != null)
                        {

                            if (GameViewModel.Instance.BlocksForTeam1 != null)
                            {
                                var mem = GameViewModel.Instance.BlocksForTeam1.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.blocksForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Count();
                                else
                                    member.blocksForJam = 0;
                            }
                            else
                                member.blocksForJam = 0;


                            if (GameViewModel.Instance.AssistsForTeam1 != null)
                            {
                                var mem = GameViewModel.Instance.AssistsForTeam1.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.assistsForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Count();
                                else
                                    member.assistsForJam = 0;
                            }
                            else
                                member.assistsForJam = 0;


                            if (GameViewModel.Instance.PenaltiesForTeam1 != null)
                            {
                                var mem = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyAgainstMember != null).Where(x => x.PenaltyAgainstMember.SkaterId != null).Where(x => x.PenaltyAgainstMember.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.penaltiesForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Count();
                                else
                                    member.penaltiesForJam = 0;
                            }
                            else
                                member.penaltiesForJam = 0;


                            if (GameViewModel.Instance.ScoresTeam1 != null)
                            {
                                var mem = GameViewModel.Instance.ScoresTeam1.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.scoreForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Sum(x => x.Points);
                                else
                                    member.scoreForJam = 0;
                            }
                            else
                                member.scoreForJam = 0;
                        }
                        member.totalAssists = GameViewModel.Instance.AssistsForTeam1.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId).Count();
                        member.totalPenalties = GameViewModel.Instance.PenaltiesForTeam1.Where(x => x.PenaltyAgainstMember != null).Where(x => x.PenaltyAgainstMember.SkaterId != null).Where(x => x.PenaltyAgainstMember.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId).Count();
                        member.totalScores = GameViewModel.Instance.ScoresTeam1.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == GameViewModel.Instance.Team1.TeamMembers[i].SkaterId).Sum(x => x.Points);
                        member.isJammer = GameViewModel.Instance.Team1.TeamMembers[i].IsJammer;
                        member.isPivot = GameViewModel.Instance.Team1.TeamMembers[i].IsPivot;
                        member.isBlocker1 = GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker1;
                        member.isBlocker2 = GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker2;
                        member.isBlocker3 = GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker3;
                        member.isBlocker4 = GameViewModel.Instance.Team1.TeamMembers[i].IsBlocker4;
                        member.linedUp = !GameViewModel.Instance.Team1.TeamMembers[i].IsBenched;
                        member.isPBox = GameViewModel.Instance.Team1.TeamMembers[i].IsInBox;
                        member.lostLead = GameViewModel.Instance.Team1.TeamMembers[i].LostLeadJammerEligibility;
                        member.isLead = GameViewModel.Instance.Team1.TeamMembers[i].IsLeadJammer;
                        team.members.Add(member);
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: GameViewModel.Instance.Team1.TeamMembers[i].SkaterId + ":" + Logger.Instance.getLoggedMessages());
                    }
                }
                team.gameName = GameViewModel.Instance.GameName;

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

            JavaScriptSerializer s = new JavaScriptSerializer();

            return s.Serialize(team);
        }
        public static string GetTeam2Members()
        {

            TeamJson team = new TeamJson();

            try
            {
                team.teamId = GameViewModel.Instance.Team2.TeamId.ToString();

                if (GameViewModel.Instance.Team2.TeamName != null)
                {
                    team.teamName = GameViewModel.Instance.Team2.TeamName;
                }

                if (GameViewModel.Instance.CurrentJam != null)
                {
                    team.currentJam = GameViewModel.Instance.CurrentJam.JamNumber;
                    team.currentJamId = GameViewModel.Instance.CurrentJam.JamId.ToString();
                    team.totalJams = GameViewModel.Instance.Jams.Count;
                }

                for (int i = 0; i < GameViewModel.Instance.Team2.TeamMembers.Count; i++)
                {
                    try
                    {
                        TeamMemberJson member = new TeamMemberJson();
                        member.memberName = GameViewModel.Instance.Team2.TeamMembers[i].SkaterName;
                        member.memberId = GameViewModel.Instance.Team2.TeamMembers[i].SkaterId.ToString();
                        member.memberNumber = GameViewModel.Instance.Team2.TeamMembers[i].SkaterNumber;

                        var blocks = GameViewModel.Instance.BlocksForTeam2.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId).Count();
                        member.totalBlocks = blocks;

                        if (GameViewModel.Instance.CurrentJam != null)
                        {

                            if (GameViewModel.Instance.BlocksForTeam2 != null)
                            {
                                var mem = GameViewModel.Instance.BlocksForTeam2.Where(x => x.PlayerWhoBlocked != null).Where(x => x.PlayerWhoBlocked.SkaterId != null).Where(x => x.PlayerWhoBlocked.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.blocksForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Count();
                                else
                                    member.blocksForJam = 0;
                            }
                            else
                                member.blocksForJam = 0;


                            if (GameViewModel.Instance.AssistsForTeam2 != null)
                            {
                                var mem = GameViewModel.Instance.AssistsForTeam2.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.assistsForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Count();
                                else
                                    member.assistsForJam = 0;
                            }
                            else
                                member.assistsForJam = 0;


                            if (GameViewModel.Instance.PenaltiesForTeam2 != null)
                            {
                                var mem = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyAgainstMember != null).Where(x => x.PenaltyAgainstMember.SkaterId != null).Where(x => x.PenaltyAgainstMember.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.penaltiesForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Count();
                                else
                                    member.penaltiesForJam = 0;
                            }
                            else
                                member.penaltiesForJam = 0;


                            if (GameViewModel.Instance.ScoresTeam2 != null)
                            {
                                var mem = GameViewModel.Instance.ScoresTeam2.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId);
                                if (mem != null && mem.Count() > 0)
                                    member.scoreForJam = mem.Where(x => x.JamId == GameViewModel.Instance.CurrentJam.JamId).Sum(x => x.Points);
                                else
                                    member.scoreForJam = 0;
                            }
                            else
                                member.scoreForJam = 0;
                        }
                        member.totalAssists = GameViewModel.Instance.AssistsForTeam2.Where(x => x.PlayerWhoAssisted != null).Where(x => x.PlayerWhoAssisted.SkaterId != null).Where(x => x.PlayerWhoAssisted.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId).Count();
                        member.totalPenalties = GameViewModel.Instance.PenaltiesForTeam2.Where(x => x.PenaltyAgainstMember != null).Where(x => x.PenaltyAgainstMember.SkaterId != null).Where(x => x.PenaltyAgainstMember.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId).Count();
                        member.totalScores = GameViewModel.Instance.ScoresTeam2.Where(x => x.PlayerWhoScored != null).Where(x => x.PlayerWhoScored.SkaterId != null).Where(x => x.PlayerWhoScored.SkaterId == GameViewModel.Instance.Team2.TeamMembers[i].SkaterId).Sum(x => x.Points);
                        member.isJammer = GameViewModel.Instance.Team2.TeamMembers[i].IsJammer;
                        member.isPivot = GameViewModel.Instance.Team2.TeamMembers[i].IsPivot;
                        member.isBlocker1 = GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker1;
                        member.isBlocker2 = GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker2;
                        member.isBlocker3 = GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker3;
                        member.isBlocker4 = GameViewModel.Instance.Team2.TeamMembers[i].IsBlocker4;
                        member.linedUp = !GameViewModel.Instance.Team2.TeamMembers[i].IsBenched;
                        member.isPBox = GameViewModel.Instance.Team2.TeamMembers[i].IsInBox;
                        team.members.Add(member);
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: GameViewModel.Instance.Team2.TeamMembers[i].SkaterId + ":" + Logger.Instance.getLoggedMessages());
                    }
                }
                team.gameName = GameViewModel.Instance.GameName;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }

            System.Web.Script.Serialization.JavaScriptSerializer s = new System.Web.Script.Serialization.JavaScriptSerializer();

            return s.Serialize(team);
        }
        public static string GetAllPenaltyTypes()
        {
            List<EnumListItem> pens = new List<EnumListItem>();
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                if (PolicyViewModel.Instance != null && (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA))
                {
                    pens = (from Scoreboard.Library.Static.Enums.PenaltiesWFTDAEnum d in Enum.GetValues(typeof(Scoreboard.Library.Static.Enums.PenaltiesWFTDAEnum))
                            select new EnumListItem { Id = (int)d, Name = RDN.Utilities.Enums.EnumExt.ToFreindlyName(d) }).OrderBy(x => x.Name).ToList();
                }
                else if (PolicyViewModel.Instance != null && (PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.WFTDA_2010))
                {
                    pens = (from Scoreboard.Library.Static.Enums.PenaltiesWFTDA2010Enum d in Enum.GetValues(typeof(Scoreboard.Library.Static.Enums.PenaltiesWFTDA2010Enum))
                            select new EnumListItem { Id = (int)d, Name = RDN.Utilities.Enums.EnumExt.ToFreindlyName(d) }).OrderBy(x => x.Name).ToList();
                }
                else if (PolicyViewModel.Instance != null && PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.MADE || GameViewModel.Instance.Policy.GameSelectionType == GameTypeEnum.MADE_COED)
                {
                    pens = (from Scoreboard.Library.Static.Enums.PenaltiesMADEEnum d in Enum.GetValues(typeof(Scoreboard.Library.Static.Enums.PenaltiesMADEEnum))
                            select new EnumListItem { Id = (int)d, Name = RDN.Utilities.Enums.EnumExt.ToFreindlyName(d) }).OrderBy(x => x.Name).ToList();
                }
                else if (PolicyViewModel.Instance != null && PolicyViewModel.Instance.GameSelectionType == GameTypeEnum.RDCL)
                {
                    pens = (from Scoreboard.Library.Static.Enums.PenaltiesRDCLEnum d in Enum.GetValues(typeof(Scoreboard.Library.Static.Enums.PenaltiesRDCLEnum))
                            select new EnumListItem { Id = (int)d, Name = RDN.Utilities.Enums.EnumExt.ToFreindlyName(d) }).OrderBy(x => x.Name).ToList();
                }
                else
                {
                    pens = (from Scoreboard.Library.Static.Enums.PenaltiesEnum d in Enum.GetValues(typeof(Scoreboard.Library.Static.Enums.PenaltiesEnum))
                            select new EnumListItem { Id = (int)d, Name = RDN.Utilities.Enums.EnumExt.ToFreindlyName(d) }).OrderBy(x => x.Name).ToList();
                }
                if (pens.Count > 0)
                {
                    sb.Append("\"penaltyTypes\": [");
                    for (int i = 0; i < pens.Count; i++)
                    {
                        sb.Append("{\"id\":");
                        sb.Append(pens[i].Id);
                        sb.Append(",");
                        sb.Append("\"name\":\"");
                        sb.Append(pens[i].Name);
                        if ((i + 1) == pens.Count)
                            sb.Append("\"}");
                        else
                            sb.Append("\"},");
                    }
                    sb.Append("]");
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            sb.Append("}");

            return sb.ToString();

        }

        public static string GrabMobileUpdate()
        {
            MobileJson json = new MobileJson();
            try
            {
                if (GameViewModel.Instance != null)
                {
                    if (GameViewModel.Instance.CurrentJam != null)
                    {
                        json.JamNumber = GameViewModel.Instance.CurrentJam.JamNumber;
                        if (GameViewModel.Instance.CurrentJam.JamClock != null)
                        {
                            json.JamTime = GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining;
                            json.IsJamRunning = GameViewModel.Instance.CurrentJam.JamClock.IsRunning;
                        }
                    }
                    json.PeriodNumber = GameViewModel.Instance.CurrentPeriod;

                    if (GameViewModel.Instance.Team1 != null)
                        json.team1Name = GameViewModel.Instance.Team1.TeamName;
                    if (GameViewModel.Instance.Team2 != null)
                        json.team2Name = GameViewModel.Instance.Team2.TeamName;

                    json.Team1Score = GameViewModel.Instance.CurrentTeam1Score;
                    json.Team2Score = GameViewModel.Instance.CurrentTeam2Score;
                    json.Team1JamScore = GameViewModel.Instance.CurrentTeam1JamScore;
                    json.Team2JamScore = GameViewModel.Instance.CurrentTeam2JamScore;

                    if (GameViewModel.Instance.CurrentTimeOutClock != null)
                    {
                        json.TimeOutTime = GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining;
                        json.IsTimeOutRunning = GameViewModel.Instance.CurrentTimeOutClock.IsRunning;
                    }
                    if (GameViewModel.Instance.PeriodClock != null)
                    {
                        json.PeriodTime = GameViewModel.Instance.PeriodClock.TimeRemaining;
                        json.IsPeriodRunning = GameViewModel.Instance.PeriodClock.IsRunning;
                    }
                    if (GameViewModel.Instance.CurrentLineUpClock != null)
                        json.LineUpTime = GameViewModel.Instance.CurrentLineUpClock.TimeRemaining;
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(json);
        }

        /// <summary>
        /// produces a game update for the video overlay.
        /// </summary>
        /// <returns></returns>
        public static string GrabGameUpdate()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            try
            {
                sb.Append("\"periodNumber\":\"P");
                sb.Append(GameViewModel.Instance.CurrentPeriod);
                sb.Append("\",");
                sb.Append("\"team1Score\":\"");
                sb.Append(GameViewModel.Instance.CurrentTeam1Score);
                sb.Append("\",");
                sb.Append("\"team1JamScore\":\"");
                sb.Append(GameViewModel.Instance.CurrentTeam1JamScore);
                sb.Append("\",");
                sb.Append("\"team1TimeOut\":\"");
                sb.Append(GameViewModel.Instance.Team1.TimeOutsLeft);
                sb.Append("\",");
                if (GameViewModel.Instance.Team1.TeamName != null)
                {
                    sb.Append("\"team1Name\":\"");
                    sb.Append(GameViewModel.Instance.Team1.TeamName);
                    sb.Append("\",");
                }
                sb.Append("\"team2Score\":\"");
                sb.Append(GameViewModel.Instance.CurrentTeam2Score);
                sb.Append("\",");
                sb.Append("\"team2JamScore\":\"");
                sb.Append(GameViewModel.Instance.CurrentTeam2JamScore);
                sb.Append("\",");
                sb.Append("\"team2TimeOut\":\"");
                sb.Append(GameViewModel.Instance.Team2.TimeOutsLeft);
                sb.Append("\",");
                if (GameViewModel.Instance.Team2.TeamName != null)
                {
                    sb.Append("\"team2Name\":\"");
                    sb.Append(GameViewModel.Instance.Team2.TeamName);
                    sb.Append("\",");
                }
                if (GameViewModel.Instance.PeriodClock != null)
                {
                    sb.Append("\"periodClock\":\"");
                    sb.Append(GameViewModel.Instance.PeriodClock.TimeRemaining);
                    sb.Append("\",");
                }
                if (GameViewModel.Instance.CurrentJam != null)
                {
                    sb.Append("\"jamNumber\":\"J");
                    sb.Append(GameViewModel.Instance.CurrentJam.JamNumber);
                    sb.Append("\",");
                    sb.Append("\"jamId\":\"");
                    sb.Append(GameViewModel.Instance.CurrentJam.JamId);
                    sb.Append("\",");
                    sb.Append("\"jamClock\":\"");
                    sb.Append(GameViewModel.Instance.CurrentJam.JamClock.TimeRemaining);
                    sb.Append("\",");
                    sb.Append("\"isJamRunning\":\"");
                    sb.Append(GameViewModel.Instance.CurrentJam.JamClock.IsRunning);
                    sb.Append("\",");
                    sb.Append("\"activeTeam1Jammer\":\"");
                    if (GameViewModel.Instance.CurrentJam.JammerT1 != null)
                        sb.Append(GameViewModel.Instance.CurrentJam.JammerT1.SkaterNumber + " " + GameViewModel.Instance.CurrentJam.JammerT1.SkaterName);
                    sb.Append("\",");
                    sb.Append("\"activeTeam2Jammer\":\"");
                    if (GameViewModel.Instance.CurrentJam.JammerT2 != null)
                        sb.Append(GameViewModel.Instance.CurrentJam.JammerT2.SkaterNumber + " " + GameViewModel.Instance.CurrentJam.JammerT2.SkaterName);
                    sb.Append("\",");
                    if (GameViewModel.Instance.CurrentLineUpClock != null)
                    {
                        sb.Append("\"lineUpClock\":\"");
                        sb.Append(GameViewModel.Instance.CurrentLineUpClock.TimeRemaining);
                        sb.Append("\",");
                        sb.Append("\"isLineUpRunning\":\"");
                        sb.Append(GameViewModel.Instance.CurrentLineUpClock.IsRunning);
                        sb.Append("\",");
                    }
                }
                sb.Append("\"isTimeOutRunning\":\"");
                sb.Append(GameViewModel.Instance.CurrentlyInTimeOut);
                sb.Append("\",");
                if (GameViewModel.Instance.CurrentTimeOutClock != null)
                {
                    sb.Append("\"timeOutClock\":\"");
                    sb.Append(GameViewModel.Instance.CurrentTimeOutClock.TimeRemaining);
                    sb.Append("\",");
                }
                if (GameViewModel.Instance.IntermissionClock != null)
                {
                    sb.Append("\"intermissionClock\":\"");
                    sb.Append(GameViewModel.Instance.IntermissionClock.TimeRemaining);
                    sb.Append("\",");
                    sb.Append("\"isIntermissionRunning\":\"");
                    sb.Append(GameViewModel.Instance.IntermissionClock.IsRunning);
                    sb.Append("\",");
                    sb.Append("\"intermissionName\":\"");
                    sb.Append(GameViewModel.Instance.NameOfIntermission);
                    sb.Append("\",");
                }
                if (GameViewModel.Instance.CurrentJam != null)
                {
                    if (GameViewModel.Instance.CurrentJam.JammerT1 != null)
                    {
                        sb.Append("\"leadJT1\":\"");
                        sb.Append(GameViewModel.Instance.CurrentJam.JammerT1.IsLeadJammer);
                        sb.Append("\",");
                    }
                    if (GameViewModel.Instance.CurrentJam.JammerT2 != null)
                    {
                        sb.Append("\"leadJT2\":\"");
                        sb.Append(GameViewModel.Instance.CurrentJam.JammerT2.IsLeadJammer);
                        sb.Append("\",");
                    }
                    if (GameViewModel.Instance.CurrentJam.PivotT1 != null)
                    {
                        sb.Append("\"leadPT1\":\"");
                        sb.Append(GameViewModel.Instance.CurrentJam.PivotT1.IsLeadJammer);
                        sb.Append("\",");
                    }
                    if (GameViewModel.Instance.CurrentJam.PivotT2 != null)
                    {
                        sb.Append("\"leadPT2\":\"");
                        sb.Append(GameViewModel.Instance.CurrentJam.PivotT2.IsLeadJammer);
                        sb.Append("\",");
                    }
                }
                sb.Append("\"gameName\":\"");
                sb.Append(GameViewModel.Instance.GameName);
                sb.Append("\",");
                sb.Append("\"gameStarted\":\"");
                sb.Append(GameViewModel.Instance.HasGameStarted);
                sb.Append("\"");
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
            sb.Append("}");

            return sb.ToString();
        }
        public static string StartStopJam()
        {
            try
            {
                if (GameViewModel.Instance.HasGameStarted && GameViewModel.Instance.CurrentJam.JamClock.IsRunning)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        Window owner = System.Windows.Application.Current.MainWindow;
                        ((MainWindow)owner).stopJamBtn_Click(null, null);
                    }));
                    return "{\"result\": \"stopped\"}";
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        Window owner = System.Windows.Application.Current.MainWindow;
                        if(owner!= null)
                        ((MainWindow)owner).startJamWithButtonClick(false);
                    }));
                    return "{\"result\": \"started\"}";
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return "{\"result\": \"stopped\"}";
        }
        public static string GetRuleSet()
        {
            try
            {
                return "{\"ruleSet\": \"" + PolicyViewModel.Instance.GameSelectionType.ToString() + "\"}";
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return "{\"result\": \"false\"}";
        }
        public static string StartOfficialTimeOut()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Window owner = System.Windows.Application.Current.MainWindow;
                    ((MainWindow)owner).timeOutBtn_Click(null, null);
                }));
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return "{\"result\": \"True\"}";
        }
        public static string TakeTimeOut(int teamNumber)
        {
            try
            {
                if (teamNumber == 1)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
        {
            Window owner = System.Windows.Application.Current.MainWindow;
            ((MainWindow)owner).timeOutTeam1Btn_Click(null, null);
        }));
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        Window owner = System.Windows.Application.Current.MainWindow;
                        ((MainWindow)owner).timeOutTeam2Btn_Click(null, null);
                    }));
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return "{\"result\": \"True\"}";

        }
        public static Guid AddJamPass(int team, Guid skaterId, Guid jamId, int jamNumber)
        {
            try
            {
                JamViewModel jam = null;
                if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId)
                    jam = GameViewModel.Instance.CurrentJam;
                else
                    jam = GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault();
                if (jam != null)
                {
                    if (team == 1)
                    {
                        var skaterWhoPassed = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterId).FirstOrDefault();
                        //we don't want to move the entire skater object so we need to create a new skater here.
                        var tempSkater = new TeamMembersViewModel() { SkaterId = skaterWhoPassed.SkaterId, SkaterName = skaterWhoPassed.SkaterName, SkaterNumber = skaterWhoPassed.SkaterNumber };
                        JamPass pass = new JamPass(jamNumber, jam.JamClock.TimeElapsed, tempSkater, jamId, TeamNumberEnum.Team1);
                        jam.AddJamPass(pass);
                        return pass.PassId;
                    }
                    else if (team == 2)
                    {
                        var skaterWhoPassed = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterId).FirstOrDefault();
                        //we don't want to move the entire skater object so we need to create a new skater here.
                        var tempSkater = new TeamMembersViewModel() { SkaterId = skaterWhoPassed.SkaterId, SkaterName = skaterWhoPassed.SkaterName, SkaterNumber = skaterWhoPassed.SkaterNumber };
                        JamPass pass = new JamPass(jamNumber, jam.JamClock.TimeElapsed, tempSkater, jamId, TeamNumberEnum.Team2);
                        jam.AddJamPass(pass);
                        return pass.PassId;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return new Guid();
        }

        /// <summary>
        /// sets is lead jammer.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="skaterId"></param>
        /// <returns></returns>
        public static bool IsLead(int team, Guid skaterId, Guid jamId, int jamNumber)
        {
            try
            {
                JamViewModel jam = null;
                if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId)
                    jam = GameViewModel.Instance.CurrentJam;
                else
                    jam = GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault();
                if (jam != null)
                {
                    if (team == 1)
                    {
                        var skaterWhoPassed = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterId).FirstOrDefault();
                        jam.setLeadJammer(skaterWhoPassed, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team1);
                        return true;
                    }
                    else if (team == 2)
                    {
                        var skaterWhoPassed = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterId).FirstOrDefault();
                        jam.setLeadJammer(skaterWhoPassed, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team2);
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return false;
        }

        /// <summary>
        /// jammer lost lead eligibity
        /// </summary>
        /// <param name="team"></param>
        /// <param name="skaterId"></param>
        /// <returns></returns>
        public static bool LostLeadJammerEligibility(int team, Guid skaterId, Guid jamId, int jamNumber)
        {
            try
            {
                JamViewModel jam = null;
                if (GameViewModel.Instance.CurrentJam != null && GameViewModel.Instance.CurrentJam.JamId == jamId)
                    jam = GameViewModel.Instance.CurrentJam;
                else
                    jam = GameViewModel.Instance.Jams.Where(x => x.JamId == jamId).FirstOrDefault();
                if (jam != null)
                {
                    if (team == 1)
                    {
                        var skaterWhoPassed = GameViewModel.Instance.Team1.TeamMembers.Where(x => x.SkaterId == skaterId).FirstOrDefault();
                        jam.setLostLeadJammerEligibility(skaterWhoPassed, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team1);
                        return true;
                    }
                    else if (team == 2)
                    {
                        var skaterWhoPassed = GameViewModel.Instance.Team2.TeamMembers.Where(x => x.SkaterId == skaterId).FirstOrDefault();
                        jam.setLostLeadJammerEligibility(skaterWhoPassed, GameViewModel.Instance.ElapsedTimeGameClockMilliSeconds, TeamNumberEnum.Team2);
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: Logger.Instance.getLoggedMessages());
            }
            return true;
        }

    }
}
