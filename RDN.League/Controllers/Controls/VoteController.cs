using RDN.League.Models.Filters;
using RDN.Library.Cache;
using RDN.Library.Classes.Controls.Voting;
using RDN.Library.Classes.Error;
using RDN.Library.Util;
using RDN.Library.Util.Enum;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Voting;
using RDN.Portable.Classes.Voting.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Controllers.UI
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class VoteController : BaseController
    {
        Regex numberFind = new Regex(@"\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public JsonResult RemoveAnswerFromPoll(string answerId)
        {
            try
            {
                var polls = VotingFactory.RemoveAnswer(Convert.ToInt64(answerId));
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public JsonResult SaveResortedOrderOfQuestions(string pId, string newIds)
        {
            try
            {
                var strings = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(newIds);
                var id = new long[strings.Length];
                for (int i = 0; i < strings.Length; i++)
                {
                    id[i] = Convert.ToInt64(numberFind.Match(strings[i]).Value);
                }
                var polls = VotingFactory.ReorderQuestions(Convert.ToInt64(pId), id);
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public JsonResult AddAnswerToQuestionToPoll(string questionId, string text)
        {
            try
            {
                var polls = VotingFactory.AddAnswerToQuestionForPoll(Convert.ToInt64(questionId), text);
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public JsonResult UpdateQuestionToPoll(string questionId, string text)
        {
            try
            {
                var polls = VotingFactory.UpdateQuestionToPoll(Convert.ToInt64(questionId), text);

                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public JsonResult UpdateAnswerToPoll(string answerId, string text)
        {
            try
            {
                var polls = VotingFactory.UpdateAnswerToPoll(Convert.ToInt64(answerId), text);

                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult PollViewV2(string leagueId, string pollid)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(leagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var polls = VotingFactory.GetPollV2(new Guid(leagueId), Convert.ToInt64(pollid), mem);


                if (!String.IsNullOrEmpty(polls.Description))
                {
                    polls.Description = polls.Description.Replace(Environment.NewLine, "<br/>");
                }

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.su.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully updated the poll.";
                    this.AddMessage(message);
                }

                return View(polls);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult PollEditAdmin(string leagueId, string pollid)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(leagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var polls = VotingFactory.GetPollV2(new Guid(leagueId), Convert.ToInt64(pollid), mem);

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.su.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully updated the poll.";
                    this.AddMessage(message);
                }

                return View(polls);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult Polls(string leagueId)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(leagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var polls = VotingFactory.GetPollsV2(new Guid(leagueId), mem);

                polls.Polls = polls.Polls.OrderByDescending(x => x.Created).ToList();
                polls.LeagueName = league.Name;

                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sv.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "You successfully voted!";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sag.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "You added a poll.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.pc.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Closed The Poll.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Successfully Deleted The Poll.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Poll is Closed, no more votes are being recorded.";
                    this.AddMessage(message);
                }
                else if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.na.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "You don't have access to that particular poll.";
                    this.AddMessage(message);
                }

                return View(polls);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult PollToVote(string leagueId, string pollid)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(leagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var polls = VotingFactory.GetPollV2(new Guid(leagueId), Convert.ToInt64(pollid), mem);
                if (polls == null)
                    return Redirect(Url.Content("~/poll/" + leagueId + "?u=" + SiteMessagesEnum.na));

                if (polls.IsClosed)
                    return Redirect(Url.Content("~/poll/" + leagueId + "?u=" + SiteMessagesEnum.cl));
                return View(polls);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult PollToVoteV2(string leagueId, string pollid)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(leagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var polls = VotingFactory.GetPollV2(new Guid(leagueId), Convert.ToInt64(pollid), mem);

                if (polls == null)
                    return Redirect(Url.Content("~/poll/" + leagueId + "?u=" + SiteMessagesEnum.na));

                if (polls.IsClosed)
                    return Redirect(Url.Content("~/poll/" + leagueId + "?u=" + SiteMessagesEnum.cl));

                return View(polls);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult VoteOnPoll(VotingClass vote)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(vote.LeagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                long answerId = Convert.ToInt64(Request.Form["AnswerType"]);


                var polls = VotingFactory.AddVote(new Guid(vote.LeagueId), Convert.ToInt64(vote.VotingId), mem, answerId, vote.Comment);
                if (polls == true)
                    return Redirect(Url.Content("~/poll/" + vote.LeagueId + "?u=" + SiteMessagesEnum.sv));
                else
                    return Redirect(Url.Content("~/poll/" + vote.LeagueId + "?u=" + SiteMessagesEnum.cl));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true)]
        public ActionResult VoteOnPollV2(VotingClass vote)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(vote.LeagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                var poll = VotingFactory.GetPollV2(league.LeagueId, vote.VotingId, mem);

                List<VotingQuestionClass> questions = new List<VotingQuestionClass>();
                foreach (var question in poll.Questions)
                {
                    VotingQuestionClass qu = new VotingQuestionClass();
                    qu.QuestionId = question.QuestionId;
                    var v = new VotesClass();
                    if (question.QuestionType == QuestionTypeEnum.Single)
                    {
                        if (!String.IsNullOrEmpty(HttpContext.Request.Form["answer-" + question.QuestionId]))
                        {
                            v.AnswerIds.Add(Convert.ToInt64(HttpContext.Request.Form["answer-" + question.QuestionId].ToString()));
                        }
                    }
                    else
                    {
                        foreach (var answer in question.Answers)
                        {
                            if (!String.IsNullOrEmpty(HttpContext.Request.Form["answer-" + question.QuestionId + "-" + answer.AnswerId]))
                            {
                                v.AnswerIds.Add(answer.AnswerId);
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(HttpContext.Request.Form["comment-" + question.QuestionId]))
                        v.OtherText = HttpContext.Request.Form["comment-" + question.QuestionId].ToString();
                    qu.Votes.Add(v);
                    questions.Add(qu);

                }

                var polls = VotingFactory.AddVoteV2(new Guid(vote.LeagueId), vote.VotingId, questions, mem);
                return Redirect(Url.Content("~/poll/" + vote.LeagueId + "?u=" + SiteMessagesEnum.sv));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult CreatePoll(string leagueId)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(leagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

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


                VotingQuestionClass q = new VotingQuestionClass();
                q.Answers.Add(new VotingAnswersClass());
                q.Answers.Add(new VotingAnswersClass());
                q.Answers.Add(new VotingAnswersClass());
                q.Answers.Add(new VotingAnswersClass());
                vc.Questions.Add(q);


                vc.LeagueId = leagueId;
                return View(vc);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [ValidateInput(false)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult CreatePollAdd(VotingClass voting)
        {
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                var league = MemberCache.GetLeagueOfMember(mem);
                if (new Guid(voting.LeagueId) != league.LeagueId)
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));

                for (int i = 0; i < 100; i++)
                {
                    if (!String.IsNullOrEmpty(HttpContext.Request["question" + i]))
                    {
                        VotingQuestionClass question = new VotingQuestionClass();
                        question.Question = HttpContext.Request["question" + i].ToString();
                        question.QuestionType = (QuestionTypeEnum)Enum.Parse(typeof(QuestionTypeEnum), HttpContext.Request["questiontype" + i].ToString());
                        for (int j = 1; j < 100; j++)
                        {
                            if (!String.IsNullOrEmpty(HttpContext.Request["answer-" + j + "-" + i]))
                            {
                                VotingAnswersClass answer = new VotingAnswersClass();
                                answer.Answer = HttpContext.Request["answer-" + j + "-" + i].ToString();
                                question.Answers.Add(answer);
                            }
                            else
                                break;
                        }
                        voting.Questions.Add(question);
                    }
                    else
                        break;
                }
                foreach (string guid in voting.ToMemberIds.Split(','))
                {
                    Guid temp = new Guid();
                    bool didWork = Guid.TryParse(guid, out temp);
                    if (didWork)
                        voting.Voters.Add(new MemberDisplayBasic() { MemberId = temp });
                }
                
                var poll = VotingFactory.AddPoll(voting, mem);
                return Redirect(Url.Content("~/poll/" + voting.LeagueId + "?u=" + SiteMessagesEnum.sag));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public JsonResult SendEmailReminderAboutPoll(string pId, string lId)
        {
            try
            {
                VotingFactory.SendPollReminder(new Guid(lId), Convert.ToInt64(pId));
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateInput(false)]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult UpdatePollV2(VotingClass voting)
        {
            try
            {
                var poll = VotingFactory.UpdatePollV2(voting);
                return Redirect(Url.Content("~/poll/edit/" + voting.LeagueId.ToString().Replace("-", "") + "/" + voting.VotingId + "?u=" + SiteMessagesEnum.su));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult UpdatePoll(VotingClass voting)
        {
            try
            {
                var poll = VotingFactory.UpdatePoll(voting);
                return Redirect(Url.Content("~/poll/view/" + voting.LeagueId.ToString().Replace("-", "") + "/" + voting.VotingId + "?u=" + SiteMessagesEnum.su));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult DeletePoll(VotingClass voting)
        {
            try
            {
                var poll = VotingFactory.DeletePoll(voting);
                return Redirect(Url.Content("~/poll/" + voting.LeagueId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.de));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult ClosePoll(VotingClass voting)
        {
            try
            {
                var poll = VotingFactory.ClosePoll(voting);
                return Redirect(Url.Content("~/poll/" + voting.LeagueId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.pc));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        [LeagueAuthorize(EmailVerification = true, IsInLeague = true, IsSecretary = true, IsPollManager = true)]
        public ActionResult OpenPoll(VotingClass voting)
        {
            try
            {
                var poll = VotingFactory.OpenPoll(voting);
                return Redirect(Url.Content("~/poll/" + voting.LeagueId.ToString().Replace("-", "") + "?u=" + SiteMessagesEnum.pc));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
    }
}
