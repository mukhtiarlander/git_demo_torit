using RDN.Library.Classes.Account.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Voting
{
    public class VotingClass
    {
        public int Version { get; set; }
        public long VotingId { get; set; }
        public DateTime Created { get; set; }

        public bool IsCurrentMemberAllowedToVote { get; set; }

        public string Question { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPollAnonymous { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosed { get; set; }
        //can league members see it.
        public bool IsOpenToLeague{ get; set; }
        public bool BroadcastPoll { get; set; }
        public string ToMemberIds { get; set; }
        public int Voted { get; set; }
        /// <summary>
        /// count of folks who didn't vote
        /// </summary>
        public int NonVotes { get; set; }
        public string LeagueName { get; set; }
        public string LeagueId { get; set; }
        [Obsolete]
        public List<VotesClass> Votes { get; set; }
        [Obsolete]
        public List<VotingAnswersClass> Answers { get; set; }
        public string Comment { get; set; }
        public List<MemberDisplay> MembersDidntVote { get; set; }
        [Obsolete]
        public List<MemberDisplay> Recipients { get; set; }
        public List<VotingQuestionClass> Questions { get; set; }
        public List<MemberDisplay> Voters { get; set; }
        public VotingClass()
        {
            Answers = new List<VotingAnswersClass>();
            Votes = new List<VotesClass>();
            Questions = new List<VotingQuestionClass>();
            Voters = new List<MemberDisplay>();
        }
        [Obsolete]
        public int GetPercentage(long answerId)
        {
            if (Votes.Count > 0)
                return Votes.Where(x => x.AnswerId == answerId).Count() * 100 / Votes.Count;
            else
                return 0;
        }
        public int GetPercentageV2(int totalAnswers, int totalVoters)
        {

            return totalAnswers * 100 / totalVoters;

        }

        public int GetBarLength(long answerId, int controlWidth)
        {
            int percentage = GetPercentage(answerId);
            return percentage * controlWidth / 100;
        }



    }
}
