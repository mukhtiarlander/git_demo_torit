using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer.Routing;
using HttpServer;
using HttpServer.Headers;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Static.Enums;
using RDN.Utilities.Util;
using Scoreboard.Library.Util;
using RDN.Utilities.Strings;

namespace RDNScoreboard.Server
{
    public class JsonRouter : IRouter
    {
        private string _fromUrl;
        public string FromUrl { get { return _fromUrl; } }


        public JsonRouter(string fromUrl)
        {
            _fromUrl = FromUrl;

        }
        public virtual ProcessingResult Process(RequestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            IRequest request = context.Request;
            IResponse response = context.Response;

            try
            {
                byte[] buffer;
                switch (request.Uri.AbsolutePath)
                {
                    case "/fullAnnouncerPage":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GetAnnouncerPage());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/deletepenalty":
                        if (request.QueryString["penaltyId"] != null)
                        {
                            var pId = new Guid(request.QueryString["penaltyId"]);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.RemovePenalty(pId));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/getpenaltiesformember":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPenalties(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/addpenalty":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["pid"] != null && request.QueryString["mm"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"]);
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            int penaltyId = Convert.ToInt32(request.QueryString["pid"]);
                            int minorMajor = Convert.ToInt32(request.QueryString["mm"]);
                            ServerHelper.AddPenalty(pId, teamNumber, jamNumber, penaltyId, minorMajor, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPenalties(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/getallpenaltytypes":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GetAllPenaltyTypes());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/getRuleSet":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GetRuleSet());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/GrabMobileUpdate":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GrabMobileUpdate());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/GrabOverlayUpdate":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GrabGameUpdate());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/GetTeam1Members":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GetTeam1Members());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/GetTeam2Members":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GetTeam2Members());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/GetTeamNames":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GetTeamNames());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/getJamNumber":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.GetJamNumber());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/addAssist":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.AddAssist(pId, teamNumber, jamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetAssists(pId, jamNumber, teamNumber, jamId));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/removeAssist":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.RemoveAssist(pId, teamNumber);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetAssists(pId, jamNumber, teamNumber, jamId));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/addBlock":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.AddBlock(pId, teamNumber, jamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetBlocks(pId, teamNumber, jamId));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/removeBlock":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.RemoveBlock(pId, teamNumber);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetBlocks(pId, teamNumber, jamId));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/scoringLoaded":
                        ServerHelper.ScoringLoaded();
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes("{ \"result\": true}");
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/addScore": //used from the jammer screens
                        if (request.QueryString["playerId"] != null && request.QueryString["points"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            int points = Convert.ToInt32(request.QueryString["points"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.AddScore(pId, teamNumber, jamId, jamNumber, points);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetScore(pId, jamId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/removeScore":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.RemoveScore(pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetScore(pId, jamId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/setBlocker1":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.SetBlocker(1, pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPlayerPositions(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/setBlocker2":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.SetBlocker(2, pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPlayerPositions(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/setBlocker3":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.SetBlocker(3, pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPlayerPositions(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/setBlocker4":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.SetBlocker(4, pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPlayerPositions(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/setPBox":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.SetPenaltyBox(pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPlayerPositions(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/setPivot":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.SetPivot(pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPlayerPositions(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/setJammer":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            var pId = new Guid(request.QueryString["playerId"]);
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.SetJammer(pId, teamNumber, jamId);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetPlayerPositions(pId, teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/startStopJam":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.StartStopJam());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/startOfficialTimeOut":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.StartOfficialTimeOut());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/addToScore": //used from the control screen.
                        if (request.QueryString["t"] != null && request.QueryString["points"] != null)
                        {
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            int points = Convert.ToInt32(request.QueryString["points"].ToString());
                            if (teamNumber == 1)
                                GameViewModel.Instance.AddScoreToGame(points, TeamNumberEnum.Team1);
                            else if (teamNumber == 2)
                                GameViewModel.Instance.AddScoreToGame(points, TeamNumberEnum.Team2);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetScore(teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/savePeriodTime": //used from the control screen.
                        if (!String.IsNullOrEmpty(request.QueryString["t"]))
                        {
                            string time = request.QueryString["t"].ToString();
                            if (Clocks.CLOCK_CHECK.IsMatch(time))
                                GameViewModel.Instance.PeriodClock.changeSecondsOfClock(Convert.ToInt32(Clocks.convertTimeDisplayToSeconds(Clocks.CLOCK_CHECK.Match(time).Value)));
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GrabMobileUpdate());
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/saveJamTime": //used from the control screen.
                        if (!String.IsNullOrEmpty(request.QueryString["t"]))
                        {
                            string time = request.QueryString["t"].ToString();
                            if (Clocks.CLOCK_CHECK.IsMatch(time))
                                GameViewModel.Instance.CurrentJam.JamClock.changeSecondsOfClock(Convert.ToInt32(Clocks.convertTimeDisplayToSeconds(Clocks.CLOCK_CHECK.Match(time).Value)));
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GrabMobileUpdate());
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/saveJamNumber": //used from the control screen.
                        if (!String.IsNullOrEmpty(request.QueryString["n"]))
                        {
                            string number = request.QueryString["n"].ToString();
                            if (StringExt.IsNumber(number))
                                GameViewModel.Instance.CurrentJam.JamNumber = Convert.ToInt32(StringExt.NumberRegex.Match(number).Value);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GrabMobileUpdate());
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/subtractFromScore":
                        if (request.QueryString["t"] != null)
                        {
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            ServerHelper.RemoveScore(teamNumber);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetScore(teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/takeTimeOut":
                        if (request.QueryString["t"] != null)
                        {
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.TakeTimeOut(teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/loadMainScreen":
                        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                        response.Connection.Type = ConnectionType.KeepAlive;
                        buffer = Encoding.UTF8.GetBytes(ServerHelper.LoadMainControlScreen());
                        response.Body.Write(buffer, 0, buffer.Length);
                        return ProcessingResult.SendResponse;
                    case "/addJamPass":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            var pId = new Guid(request.QueryString["playerId"]);
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            ServerHelper.AddJamPass(teamNumber, pId, jamId, jamNumber);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.GetScore(teamNumber));
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    case "/isLead":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            var pId = new Guid(request.QueryString["playerId"]);
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.IsLead(teamNumber, pId, jamId, jamNumber).ToString());
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    //case "/calledJam":
                    //    if (request.QueryString["playerId"] != null && request.QueryString["t"] != null)
                    //    {
                    //        int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                    //        var pId = new Guid(request.QueryString["playerId"]);
                    //        response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                    //        response.Connection.Type = ConnectionType.KeepAlive;
                    //        buffer = Encoding.UTF8.GetBytes(ServerHelper.CalledJam(teamNumber, pId));
                    //        response.Body.Write(buffer, 0, buffer.Length);
                    //        return ProcessingResult.SendResponse;
                    //    }
                    //    break;
                    case "/lostLead":
                        if (request.QueryString["playerId"] != null && request.QueryString["t"] != null && request.QueryString["jamNumber"] != null && request.QueryString["jamId"] != null)
                        {
                            int teamNumber = Convert.ToInt32(request.QueryString["t"].ToString());
                            var pId = new Guid(request.QueryString["playerId"]);
                            int jamNumber = Convert.ToInt32(request.QueryString["jamNumber"]);
                            Guid jamId = new Guid(request.QueryString["jamId"]);
                            response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                            response.Connection.Type = ConnectionType.KeepAlive;
                            buffer = Encoding.UTF8.GetBytes(ServerHelper.LostLeadJammerEligibility(teamNumber, pId, jamId, jamNumber).ToString());
                            response.Body.Write(buffer, 0, buffer.Length);
                            return ProcessingResult.SendResponse;
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: request.Uri.OriginalString + Logger.Instance.getLoggedMessages());
                response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
                response.Connection.Type = ConnectionType.KeepAlive;
                byte[] buffer11 = Encoding.UTF8.GetBytes("");
                response.Body.Write(buffer11, 0, buffer11.Length);
                return ProcessingResult.SendResponse;
            }
            return ProcessingResult.Continue;
        }
    }
}
