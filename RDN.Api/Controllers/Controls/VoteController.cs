using RDN.Library.Cache;
using RDN.Library.Classes.Controls.Voting;
using RDN.Library.Classes.Error;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Voting;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace RDN.Api.Controllers.Controls
{
    public class VoteController : Controller
    {
        //
        // GET: /Vote/

        public ActionResult Polls()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        var league = MemberCache.GetLeagueOfMember(ob.Mid);

                        var polls = VotingFactory.GetPollsV2(league.LeagueId, ob.Mid);
                        polls.LeagueName = league.Name;
                        polls.IsSuccessful = true;
                        return Json(polls, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new PollBase() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PollViewV2()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        var polls = VotingFactory.GetPollV2(edit.CurrentLeagueId, ob.IdOfAnySort, ob.Mid);
                        polls.IsSuccessful = true;
                        return Json(polls, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new VotingClass() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PollToVote()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        var polls = VotingFactory.GetPollV2(edit.CurrentLeagueId, ob.IdOfAnySort, ob.Mid);
                        polls.IsSuccessful = true;
                        return Json(polls, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new VotingClass() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        Regex numberFind = new Regex(@"\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public JsonResult RemoveAnswerFromPoll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        if (VotingFactory.RemoveAnswer(ob.IdOfAnySort))
                            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult SaveResortedOrderOfQuestions(string pId, string newIds)
        //{
        //    try
        //    {
        //        var strings = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(newIds);
        //        var id = new long[strings.Length];
        //        for (int i = 0; i < strings.Length; i++)
        //        {
        //            id[i] = Convert.ToInt64(numberFind.Match(strings[i]).Value);
        //        }
        //        var polls = VotingFactory.ReorderQuestions(Convert.ToInt64(pId), id);
        //        return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult AddAnswerToQuestionToPoll()
        //{
        //    try
        //    {
        //        if (HttpContext.Request.InputStream != null)
        //        {
        //            Stream stream = HttpContext.Request.InputStream;
        //            var ob = Network.LoadObject<MemberPassParams>(ref stream);
        //            var edit = MemberCache.GetMemberDisplay(ob.Mid);
        //            if (ob.Uid == edit.UserId)
        //            {
        //                if (VotingFactory.AddAnswerToQuestionForPoll(ob.IdOfAnySort, ob.TextOfAnySort))
        //                    return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult UpdateQuestionToPoll()
        //{
        //    try
        //    {
        //        if (HttpContext.Request.InputStream != null)
        //        {
        //            Stream stream = HttpContext.Request.InputStream;
        //            var ob = Network.LoadObject<MemberPassParams>(ref stream);
        //            var edit = MemberCache.GetMemberDisplay(ob.Mid);
        //            if (ob.Uid == edit.UserId)
        //            {
        //                var polls = VotingFactory.UpdateQuestionToPoll(ob.IdOfAnySort, ob.TextOfAnySort);

        //                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult UpdateAnswerToPoll()
        //{
        //    try
        //    {
        //        if (HttpContext.Request.InputStream != null)
        //        {
        //            Stream stream = HttpContext.Request.InputStream;
        //            var ob = Network.LoadObject<MemberPassParams>(ref stream);
        //            var edit = MemberCache.GetMemberDisplay(ob.Mid);
        //            if (ob.Uid == edit.UserId)
        //            {
        //                var polls = VotingFactory.UpdateAnswerToPoll(ob.IdOfAnySort, ob.TextOfAnySort);

        //                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult VoteOnPoll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<VotingClass>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        if (VotingFactory.AddVoteV2(edit.CurrentLeagueId, ob.VotingId, ob.Questions, ob.Mid))
                            ob.IsSuccessful = true;
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new VotingClass() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreatePoll(string leagueId)
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<MemberPassParams>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        var league = MemberCache.GetLeagueOfMember(ob.Mid);

                        VotingClass vc = new VotingClass();
                        vc.LeagueName = league.Name;
                        for (int i = 0; i < league.LeagueMembers.Count; i++)
                        {
                            vc.Voters.Add(new MemberDisplayBasic()
                            {
                                MemberId = league.LeagueMembers[i].MemberId,
                                Firstname = league.LeagueMembers[i].Firstname,
                                LastName = league.LeagueMembers[i].LastName,
                                DerbyName = league.LeagueMembers[i].DerbyName
                            });
                        }
                        vc.IsSuccessful = true;
                        return Json(vc, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new VotingClass() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreatePollAdd()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<VotingClass>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {

                        var poll = VotingFactory.AddPoll(ob, ob.Mid);
                        poll.IsSuccessful = true;
                        return Json(poll, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new VotingClass() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendEmailReminderAboutPoll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<VotingClass>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {

                        VotingFactory.SendPollReminder(new Guid(ob.LeagueId), ob.VotingId);
                        return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdatePoll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<VotingClass>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);

                    if (ob.Uid == edit.UserId)
                    {
                        ob.LeagueId = edit.CurrentLeagueId.ToString();
                        ob.IsSuccessful = VotingFactory.UpdatePollMobileAPI(ob);
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new VotingClass() { IsSuccessful = false }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeletePoll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<VotingClass>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        ob.LeagueId = edit.CurrentLeagueId.ToString();
                        ob.IsSuccessful = VotingFactory.DeletePoll(ob);
                        return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClosePoll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<VotingClass>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        ob.LeagueId = edit.CurrentLeagueId.ToString();
                        ob.IsSuccessful = VotingFactory.ClosePoll(ob);
                            return Json(ob, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OpenPoll()
        {
            try
            {
                if (HttpContext.Request.InputStream != null)
                {
                    Stream stream = HttpContext.Request.InputStream;
                    var ob = Network.LoadObject<VotingClass>(ref stream);
                    var edit = MemberCache.GetMemberDisplay(ob.Mid);
                    if (ob.Uid == edit.UserId)
                    {
                        if (VotingFactory.OpenPoll(ob))
                            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
    }
}
