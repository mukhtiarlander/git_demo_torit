using RDN.Library.Cache;
using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Config;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Mobile;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Controls.Voting;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Voting;
using RDN.Portable.Classes.Voting.Enums;
using RDN.Portable.Config;
using RDN.Utilities.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Voting
{
    public class VotingFactory
    {



        public static void SendPollReminder(Guid leagueId, long pollId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                var member = MemberCache.GetMemberDisplay(memId);
                var poll = GetPollV2(leagueId, pollId, memId);

                var mems = poll.Voters;

                foreach (var mem in mems)
                {
                    SendEmailAboutNewPoll(leagueId.ToString(), pollId, member.DerbyName, mem.DerbyName, mem.UserId);
                }

                List<Guid> memIds = poll.Voters.Select(x => x.MemberId).ToList();
                var fact = new MobileNotificationFactory()
                    .Initialize("New Poll Created:", poll.Title, Mobile.Enums.NotificationTypeEnum.NewPollCreated)
                    .AddId(pollId)
                    .AddMembers(memIds)
                    .SendNotifications();


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

        }


        public static VotingClass AddPoll(VotingClass poll, Guid memberId)
        {
            try
            {
                var member = MemberCache.GetMemberDisplay(memberId);
                var dc = new ManagementContext();
                RDN.Library.DataModels.Controls.Voting.VotingV2 voting = new DataModels.Controls.Voting.VotingV2();
                voting.IsDeleted = false;
                voting.IsPublic = poll.IsPublic;
                voting.IsPollAnonymous = poll.IsPollAnonymous;

                voting.LeagueOwner = dc.Leagues.Where(x => x.LeagueId == member.CurrentLeagueId).FirstOrDefault();
                voting.Description = poll.Description;
                voting.Title = poll.Title;
                voting.IsOpenToLeague = poll.IsOpenToLeague;

                for (int i = 0; i < poll.Questions.Count; i++)
                {
                    VotingQuestion question = new VotingQuestion();
                    question.Question = poll.Questions[i].Question;
                    question.QuestionSortId = i;
                    question.QuestionType = (int)poll.Questions[i].QuestionType;

                    for (int j = 0; j < poll.Questions[i].Answers.Count; j++)
                    {
                        if (!String.IsNullOrEmpty(poll.Questions[i].Answers[j].Answer))
                        {
                            VotingAnswer answer = new VotingAnswer();
                            answer.Answer = poll.Questions[i].Answers[j].Answer;
                            answer.Question = question;
                            question.Answers.Add(answer);
                        }
                    }

                    voting.Questions.Add(question);
                }
                //List<Guid> listOfGuids = new List<Guid>();

                foreach (var mem in poll.Voters)
                {
                    voting.Voters.Add(new VotingVoters() { HasVoted = false, Member = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault() });
                }

                dc.VotingV2.Add(voting);

                int c = dc.SaveChanges();
                poll.VotingId = voting.VotingId;
                if (poll.BroadcastPoll)
                {
                    var mems = MemberCache.GetLeagueMembers(memberId, member.CurrentLeagueId);
                    for (int j = 0; j < poll.Voters.Count; j++)
                    {
                        if (memberId != poll.Voters[j].MemberId)
                        {
                            var m = mems.Where(x => x.MemberId == poll.Voters[j].MemberId).FirstOrDefault();
                            SendEmailAboutNewPoll(poll.LeagueId, poll.VotingId, member.DerbyName, m.DerbyName, m.UserId);
                        }
                    }
                }
                List<Guid> memIds = poll.Voters.Select(x => x.MemberId).ToList();
                var fact = new MobileNotificationFactory()
                    .Initialize("New Poll Created:", poll.Title, Mobile.Enums.NotificationTypeEnum.NewPollCreated)
                    .AddId(poll.VotingId)
                    .AddMembers(memIds)
                    .SendNotifications();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: poll.ToMemberIds);
            }
            return poll;
        }
        private static void SendEmailAboutNewPoll(string leagueId, long pollId, string createdByMemberName, string derbyName, Guid userId)
        {
            try
            {
                if (userId != new Guid())
                {
                    var emailData = new Dictionary<string, string>
                                        {
                                            { "derbyname",derbyName}, 
                                            { "FromUserName", createdByMemberName }, 
                                                                                        { "viewPollLink",                                               LibraryConfig.InternalSite +"/poll/votev2/"+leagueId.ToString().Replace("-","") +"/" +pollId}
                                        };
                    var user = System.Web.Security.Membership.GetUser((object)userId);
                    if (user != null)
                    {
                        EmailServer.EmailServer.SendEmail(LibraryConfig.DefaultEmailMessage, LibraryConfig.DefaultEmailFromName, user.UserName, LibraryConfig.DefaultEmailSubject + " New Poll Created", emailData, EmailServer.EmailServerLayoutsEnum.PollNewPollCreated);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static PollBase GetPollsNotVotedOn(Guid leagueId, Guid memId)
        {
            PollBase fact = new PollBase();
            var dc = new ManagementContext();
            try
            {
                var membersCount = MemberCache.GetLeagueMembers(memId, leagueId).Count;

                fact.LeagueId = leagueId;

                var votingV2 = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == leagueId && x.Voters.Select(y => y.Member.MemberId).Contains(memId) && x.IsClosed == false && x.IsDeleted == false && x.Questions.FirstOrDefault().Votes.Where(y => y.Member.MemberId == memId).Count() == 0).ToList();

                fact.LeagueId = leagueId;
                for (int i = 0; i < votingV2.Count; i++)
                {
                    VotingClass v = new VotingClass();
                    v.IsPublic = votingV2[i].IsPublic;
                    v.IsDeleted = votingV2[i].IsDeleted;
                    v.IsOpenToLeague = votingV2[i].IsOpenToLeague;
                    v.Title = votingV2[i].Title;
                    v.VotingId = votingV2[i].VotingId;
                    v.Version = 2;
                    fact.Polls.Add(v);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return fact;
        }

        public static PollBase GetPollsV2(Guid leagueId, Guid currentMemberId)
        {
            PollBase fact = new PollBase();
            var dc = new ManagementContext();
            try
            {
                var membersCount = MemberCache.GetLeagueMembers(currentMemberId, leagueId).Count;
                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == leagueId && x.IsDeleted == false).ToList();
                fact.IsPollManager = MemberCache.IsPollMgrOrBetterOfLeague(currentMemberId);
                fact.LeagueId = leagueId;
                for (int i = 0; i < voting.Count; i++)
                {
                    VotingClass v = new VotingClass();
                    v.CanEditPoll = fact.IsPollManager;
                    v.IsOpenToLeague = voting[i].IsOpenToLeague;
                    v.IsClosed = voting[i].IsClosed;
                    if (voting[i].Voters.Select(x => x.Member.MemberId).Contains(currentMemberId))
                    {
                        v.IsCurrentMemberAllowedToVote = true;
                    }
                    if (fact.IsPollManager || v.IsOpenToLeague)
                        v.CanViewPoll = true;
                    if (fact.IsPollManager || (v.IsCurrentMemberAllowedToVote && !v.IsClosed))
                        v.CanVotePoll = true;
                    //v.CanVotePoll = false;
                    //v.CanViewPoll = false;
                    //v.CanEditPoll = false;
                    v.Version = 2;
                    v.IsPublic = voting[i].IsPublic;
                    v.IsDeleted = voting[i].IsDeleted;
                    v.Question = voting[i].Title;
                    v.VotingId = voting[i].VotingId;
                    v.Created = voting[i].Created + new TimeSpan(voting[i].LeagueOwner.TimeZone,0,0);
                    v.NonVotes = voting[i].Voters.Count;
                    if (voting[i].Questions.Count > 0)
                        v.Voted = voting[i].Questions.FirstOrDefault().Votes.Count;

                    fact.Polls.Add(v);
                }
                fact.Polls = fact.Polls.OrderByDescending(x => x.Created).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return fact;
        }
        public static bool UpdatePollMobileAPI(VotingClass poll)
        {
            var dc = new ManagementContext();
            try
            {

                Guid lId = new Guid(poll.LeagueId);
                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();
                voting.Title = poll.Title;
                voting.Description = poll.Description;
                voting.IsOpenToLeague = poll.IsOpenToLeague;

                ////question got deleted.
                //shouldn't be allowed to delete.
                //if (poll.Questions.Count != voting.Questions.Count)
                //{
                //    for (int k = 0; k < voting.Questions.Count; k++)
                //    {
                //        var aTemp = poll.Questions.Where(x => x.QuestionId == voting.Questions[k].QuestionId).FirstOrDefault();
                //        if (aTemp == null)
                //            voting.Questions[k].IsRemoved = true;
                //    }
                //}

                for (int i = 0; i < poll.Questions.Count; i++)
                {
                    var temp = voting.Questions.Where(x => x.QuestionId == poll.Questions[i].QuestionId).FirstOrDefault();
                    if (temp == null)
                    {
                        temp = new VotingQuestion();
                        temp.Question = poll.Questions[i].Question;
                        temp.QuestionSortId = voting.Questions.Count;
                        temp.QuestionType = (int)poll.Questions[i].QuestionType;
                        for (int j = 0; j < poll.Questions[i].Answers.Count; j++)
                        {
                            if (!String.IsNullOrEmpty(poll.Questions[i].Answers[j].Answer))
                            {
                                VotingAnswer answer = new VotingAnswer();
                                answer.Answer = poll.Questions[i].Answers[j].Answer;
                                answer.Question = temp;
                                temp.Answers.Add(answer);
                            }
                        }
                        voting.Questions.Add(temp);
                    }
                    else
                    {
                        temp.Question = poll.Questions[i].Question;
                        temp.QuestionSortId = i;
                        temp.QuestionType = (int)poll.Questions[i].QuestionType;

                        //answer got deleted
                        if (poll.Questions[i].Answers.Count != temp.Answers.Count)
                        {
                            for (int k = 0; k < temp.Answers.Count; k++)
                            {
                                var aTemp = poll.Questions[i].Answers.Where(x => x.AnswerId == temp.Answers[k].AnswerId).FirstOrDefault();
                                if (aTemp == null)
                                    temp.Answers[k].WasRemoved = true;
                            }
                        }

                        for (int j = 0; j < poll.Questions[i].Answers.Count; j++)
                        {
                            var tempAnswer = temp.Answers.Where(x => x.AnswerId == poll.Questions[i].Answers[j].AnswerId).FirstOrDefault();
                            if (tempAnswer == null)
                            {
                                if (!String.IsNullOrEmpty(poll.Questions[i].Answers[j].Answer))
                                {
                                    tempAnswer = new VotingAnswer();
                                    tempAnswer.Answer = poll.Questions[i].Answers[j].Answer;
                                    tempAnswer.Question = temp;
                                    temp.Answers.Add(tempAnswer);
                                }
                            }
                            else
                            {
                                tempAnswer.Answer = poll.Questions[i].Answers[j].Answer;
                            }
                        }

                    }
                }


                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdatePollV2(VotingClass poll)
        {
            var dc = new ManagementContext();
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                Guid lId = new Guid(poll.LeagueId);
                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();
                voting.Title = poll.Title;
                voting.Description = poll.Description;
                voting.IsOpenToLeague = poll.IsOpenToLeague;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdatePoll(VotingClass poll)
        {
            var dc = new ManagementContext();
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                Guid lId = new Guid(poll.LeagueId);
                var voting = dc.Voting.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();
                voting.Question = poll.Question;
                voting.Description = poll.Description;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateAnswerToPoll(long answerId, string text)
        {
            var dc = new ManagementContext();
            try
            {
                var voting = dc.Answers.Where(x => x.AnswerId == answerId).FirstOrDefault();
                voting.Answer = text;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool RemoveAnswer(long answerId)
        {
            var dc = new ManagementContext();
            try
            {
                var voting = dc.Answers.Where(x => x.AnswerId == answerId).FirstOrDefault();
                voting.WasRemoved = true;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ReorderQuestions(long pollId, long[] idsInOrder)
        {
            var dc = new ManagementContext();
            try
            {
                var voting = dc.VotingV2.Where(x => x.VotingId == pollId).FirstOrDefault();
                for (int i = 0; i < idsInOrder.Length; i++)
                {
                    var q = voting.Questions.Where(x => x.QuestionId == idsInOrder[i]).FirstOrDefault();
                    q.QuestionSortId = i;
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateQuestionToPoll(long questionId, string text)
        {
            var dc = new ManagementContext();
            try
            {
                var voting = dc.VotingQuestions.Where(x => x.QuestionId == questionId).FirstOrDefault();
                voting.Question = text;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool AddAnswerToQuestionForPoll(long questionId, string text)
        {
            var dc = new ManagementContext();
            try
            {
                var voting = dc.VotingQuestions.Where(x => x.QuestionId == questionId).FirstOrDefault();

                VotingAnswer ans = new VotingAnswer();
                ans.Answer = text;
                ans.Question = voting;
                voting.Answers.Add(ans);

                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool ClosePoll(VotingClass poll)
        {
            var dc = new ManagementContext();
            try
            {
                Guid lId = new Guid(poll.LeagueId);

                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();

                if (voting != null)
                {
                    voting.IsClosed = true;
                }
                else
                {
                    var votingv1 = dc.Voting.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();
                    votingv1.IsClosed = true;
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool OpenPoll(VotingClass poll)
        {
            var dc = new ManagementContext();
            try
            {
                var mem = RDN.Library.Classes.Account.User.GetMemberId();
                Guid lId = new Guid(poll.LeagueId);

                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();

                if (voting != null)
                {
                    voting.IsClosed = false;
                }
                else
                {
                    var votingv1 = dc.Voting.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();
                    votingv1.IsClosed = false;
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool DeletePoll(VotingClass poll)
        {
            var dc = new ManagementContext();
            try
            {
                Guid lId = new Guid(poll.LeagueId);

                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();

                if (voting != null)
                {
                    voting.IsDeleted = true;
                }
                else
                {
                    var votingv1 = dc.Voting.Where(x => x.LeagueOwner.LeagueId == lId && x.VotingId == poll.VotingId).FirstOrDefault();
                    votingv1.IsDeleted = true;
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static VotingClass GetPollV2(Guid leagueId, long pollId, Guid mem)
        {
            VotingClass v = new VotingClass();
            var dc = new ManagementContext();
            try
            {
                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == leagueId && x.IsDeleted == false && x.VotingId == pollId).FirstOrDefault();
                if (voting == null)
                    return null;
                v.IsPublic = voting.IsPublic;
                v.IsClosed = voting.IsClosed;
                v.IsDeleted = voting.IsDeleted;
                v.IsOpenToLeague = voting.IsOpenToLeague;
                v.Title = voting.Title;
                v.Version = 2;
                foreach (var voter in voting.Voters)
                {
                    MemberDisplay m = new MemberDisplay();
                    m.MemberId = voter.Member.MemberId;
                    m.DerbyName = voter.Member.DerbyName;
                    m.PlayerNumber = voter.Member.PlayerNumber;
                    m.UserId = voter.Member.AspNetUserId;
                    v.Voters.Add(m);
                }
                //= MemberCache.GetLeagueMembers(mem, leagueId);
                foreach (var question in voting.Questions.OrderBy(x => x.QuestionSortId))
                {
                    VotingQuestionClass q = new VotingQuestionClass();
                    q.Question = question.Question;
                    q.SortableOrderId = question.QuestionSortId;
                    q.QuestionId = question.QuestionId;
                    q.QuestionType = (QuestionTypeEnum)question.QuestionType;
                    var answers = question.Answers.Where(x => x.WasRemoved == false).ToList();
                    for (int vo = 0; vo < answers.Count; vo++)
                    {
                        try
                        {
                            VotingAnswersClass vc = new VotingAnswersClass();
                            vc.Answer = answers[vo].Answer;
                            vc.AnswerId = answers[vo].AnswerId;
                            vc.WasRemoved = answers[vo].WasRemoved;
                            q.Answers.Add(vc);
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    for (int vo = 0; vo < question.Votes.Count; vo++)
                    {
                        try
                        {
                            VotesClass vc = new VotesClass();
                            vc.IPAddress = question.Votes[vo].IPAddress;
                            vc.OtherText = question.Votes[vo].OtherText;
                            vc.Created = question.Votes[vo].Created;
                            if (question.Votes[vo].AnswerSelected != null)
                            {
                                vc.AnswerId = question.Votes[vo].AnswerSelected.AnswerId;
                                vc.AnswerIds.Add(vc.AnswerId);
                            }
                            foreach (var ans in question.Votes[vo].AnswersSelected)
                            {
                                vc.AnswerIds.Add(ans.AnswerSelected.AnswerId);
                            }
                            if (question.Votes[vo].Member != null)
                            {
                                vc.MemberId = question.Votes[vo].Member.MemberId;
                                vc.DerbyName = question.Votes[vo].Member.DerbyName;
                                vc.UserId = question.Votes[vo].Member.AspNetUserId;
                            }
                            vc.VoteId = question.Votes[vo].VoteId;
                            q.Votes.Add(vc);
                            v.Voters.Remove(v.Voters.Where(x => x.MemberId == vc.MemberId).FirstOrDefault());
                        }
                        catch (Exception exception)
                        {
                            ErrorDatabaseManager.AddException(exception, exception.GetType());
                        }
                    }
                    v.Questions.Add(q);
                }
                v.Description = voting.Description;
                v.IsClosed = voting.IsClosed;
                v.VotingId = voting.VotingId;
                v.IsPollAnonymous = voting.IsPollAnonymous;
                v.LeagueId = leagueId.ToString().Replace("-", "");
                //making due for old polls.

                return v;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        //public static VotingClass GetPoll(Guid leagueId, long pollId)
        //{

        //    var dc = new ManagementContext();
        //    try
        //    {
        //        VotingClass v = new VotingClass();
        //        var mem = RDN.Library.Classes.Account.User.GetMemberId();
        //        var voting = dc.Voting.Where(x => x.LeagueOwner.LeagueId == leagueId && x.IsDeleted == false && x.VotingId == pollId).FirstOrDefault();
        //        v.IsPublic = voting.IsPublic;
        //        v.IsDeleted = voting.IsDeleted;
        //        v.IsClosed = voting.IsClosed;
        //        v.Version = 1;
        //        v.Question = voting.Question;
        //        v.Description = voting.Description;
        //        v.VotingId = voting.VotingId;
        //        v.IsPollAnonymous = voting.IsPollAnonymous;
        //        v.LeagueId = leagueId.ToString().Replace("-", "");
        //        //making due for old polls.
        //        if (voting.Created < new DateTime(2013, 10, 26))
        //        {
        //            v.MembersDidntVote = MemberCache.GetLeagueMembers(mem, leagueId);
        //            for (int vo = 0; vo < voting.Votes.Count; vo++)
        //            {
        //                try
        //                {
        //                    VotesClass vc = new VotesClass();
        //                    vc.IPAddress = voting.Votes[vo].IPAddress;
        //                    vc.OtherText = voting.Votes[vo].OtherText;
        //                    vc.Created = voting.Votes[vo].Created;
        //                    if (voting.Votes[vo].AnswerSelected != null)
        //                        vc.AnswerId = voting.Votes[vo].AnswerSelected.AnswerId;
        //                    if (voting.Votes[vo].Member != null)
        //                    {
        //                        vc.MemberId = voting.Votes[vo].Member.MemberId;
        //                        vc.DerbyName = voting.Votes[vo].Member.DerbyName;
        //                        vc.UserId = voting.Votes[vo].Member.AspNetUserId;
        //                    }
        //                    vc.VoteId = voting.Votes[vo].VoteId;
        //                    v.Votes.Add(vc);
        //                    v.MembersDidntVote.Remove(v.MembersDidntVote.Where(x => x.MemberId == vc.MemberId).FirstOrDefault());
        //                }
        //                catch (Exception exception)
        //                {
        //                    ErrorDatabaseManager.AddException(exception, exception.GetType());
        //                }
        //            }
        //        }
        //        else
        //        {
        //            v.MembersDidntVote = new List<Account.Classes.MemberDisplay>();
        //            for (int vo = 0; vo < voting.Votes.Count; vo++)
        //            {
        //                try
        //                {
        //                    VotesClass vc = new VotesClass();
        //                    vc.HasVoted = voting.Votes[vo].HasVoted;
        //                    vc.IPAddress = voting.Votes[vo].IPAddress;
        //                    vc.OtherText = voting.Votes[vo].OtherText;
        //                    vc.Created = voting.Votes[vo].Created;
        //                    if (voting.Votes[vo].AnswerSelected != null)
        //                        vc.AnswerId = voting.Votes[vo].AnswerSelected.AnswerId;
        //                    if (voting.Votes[vo].Member != null)
        //                    {
        //                        vc.MemberId = voting.Votes[vo].Member.MemberId;
        //                        vc.DerbyName = voting.Votes[vo].Member.DerbyName;
        //                    }
        //                    vc.VoteId = voting.Votes[vo].VoteId;
        //                    if (vc.HasVoted)
        //                    {
        //                        v.Votes.Add(vc);
        //                    }
        //                    else
        //                        v.MembersDidntVote.Add(new Account.Classes.MemberDisplay() { MemberId = vc.MemberId, DerbyName = vc.DerbyName });
        //                }
        //                catch (Exception exception)
        //                {
        //                    ErrorDatabaseManager.AddException(exception, exception.GetType());
        //                }
        //            }
        //        }
        //        for (int vo = 0; vo < voting.Answers.Count; vo++)
        //        {
        //            try
        //            {
        //                VotingAnswersClass vc = new VotingAnswersClass();
        //                vc.Answer = voting.Answers[vo].Answer;
        //                vc.AnswerId = voting.Answers[vo].AnswerId;
        //                v.Answers.Add(vc);
        //            }
        //            catch (Exception exception)
        //            {
        //                ErrorDatabaseManager.AddException(exception, exception.GetType());
        //            }
        //        }

        //        return v;
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, exception.GetType());
        //    }
        //    return null;
        //}
        public static bool AddVote(Guid leagueId, long pollId, Guid memberId, long answerId, string otherText)
        {

            var dc = new ManagementContext();
            try
            {
                var voting = dc.Voting.Include("Votes").Include("Answers").Where(x => x.LeagueOwner.LeagueId == leagueId && x.IsDeleted == false && x.VotingId == pollId).FirstOrDefault();
                if (voting == null)
                    return false;
                var vote = voting.Votes.Where(x => x.Member.MemberId == memberId).FirstOrDefault();
                if (vote == null)
                {
                    Votes v = new Votes();
                    v.AnswerSelected = voting.Answers.Where(x => x.AnswerId == answerId).FirstOrDefault();
                    v.Member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                    v.OtherText = otherText;
                    v.HasVoted = true;
                    voting.Votes.Add(v);

                }
                else
                {
                    vote.OtherText = otherText;
                    vote.AnswerSelected = voting.Answers.Where(x => x.AnswerId == answerId).FirstOrDefault();
                    vote.HasVoted = true;
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool AddVoteV2(Guid leagueId, long pollId, List<VotingQuestionClass> questions, Guid memberId)
        {

            var dc = new ManagementContext();
            try
            {
                var voting = dc.VotingV2.Where(x => x.LeagueOwner.LeagueId == leagueId && x.IsDeleted == false && x.VotingId == pollId).FirstOrDefault();

                foreach (var question in voting.Questions)
                {
                    try
                    {
                        //gets the local question.
                        var q = questions.Where(x => x.QuestionId == question.QuestionId).FirstOrDefault();
                        //checks for a vote within the DB.
                        var vote = question.Votes.Where(x => x.Member.MemberId == memberId).FirstOrDefault();
                        if (vote == null)
                        {//adds the vote if not in the DB.
                            Votes v = new Votes();
                            var tempVote = q.Votes.FirstOrDefault();
                            foreach (var answerId in tempVote.AnswerIds)
                            {
                                var a = question.Answers.Where(x => x.AnswerId == answerId).FirstOrDefault();
                                VotesAnswersSelected sel = new VotesAnswersSelected();
                                sel.AnswerSelected = a;
                                sel.Vote = v;
                                v.AnswersSelected.Add(sel);
                            }
                            v.HasVoted = true;
                            v.Member = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                            v.OtherText = tempVote.OtherText;
                            question.Votes.Add(v);
                        }
                        else
                        {
                            var tempVote = q.Votes.FirstOrDefault();

                            var pastAnswers = vote.AnswersSelected.ToList();
                            //remove answers from past set that weren't selected this time.
                            for (int i = 0; i < pastAnswers.Count; i++)
                            {
                                if (!tempVote.AnswerIds.Contains(pastAnswers[i].AnswerSelected.AnswerId))
                                {
                                    vote.AnswersSelected.Remove(pastAnswers[i]);
                                }
                            }
                            //add new answers selected
                            foreach (var answerId in tempVote.AnswerIds)
                            {
                                var existingAnswer = pastAnswers.Where(x => x.AnswerSelected.AnswerId == answerId).FirstOrDefault();
                                if (existingAnswer == null)
                                {
                                    var a = question.Answers.Where(x => x.AnswerId == answerId).FirstOrDefault();
                                    VotesAnswersSelected sel = new VotesAnswersSelected();
                                    sel.AnswerSelected = a;
                                    sel.Vote = vote;
                                    vote.AnswersSelected.Add(sel);
                                }
                            }
                            vote.OtherText = tempVote.OtherText;
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }
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
