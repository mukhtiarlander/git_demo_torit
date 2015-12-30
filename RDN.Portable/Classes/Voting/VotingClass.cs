using ProtoBuf;
using RDN.Portable.Classes.Account.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace RDN.Portable.Classes.Voting
{
    [ProtoContract]
    [DataContract]
    public class VotingClass : MemberPassParams
    {
        [ProtoMember(201)]
        [DataMember]
        public int Version { get; set; }
        [ProtoMember(202)]
        [DataMember]
        public long VotingId { get; set; }
        [ProtoMember(203)]
        [DataMember]
        public DateTime Created { get; set; }
        [ProtoMember(204)]
        [DataMember]
        public bool IsCurrentMemberAllowedToVote { get; set; }
        [ProtoMember(205)]
        [DataMember]
        public string Question { get; set; }
        [ProtoMember(206)]
        [DataMember]
        public string Title { get; set; }
        [ProtoMember(207)]
        [DataMember]
        public string Description { get; set; }

        [ProtoMember(208)]
        [DataMember]
        public bool IsPublic { get; set; }
        [ProtoMember(209)]
        [DataMember]
        public bool IsPollAnonymous { get; set; }
        [ProtoMember(210)]
        [DataMember]
        public bool IsDeleted { get; set; }
        [ProtoMember(211)]
        [DataMember]
        public bool IsClosed { get; set; }
        //can league members see it.
        [ProtoMember(212)]
        [DataMember]
        public bool IsOpenToLeague { get; set; }
        [ProtoMember(213)]
        [DataMember]
        public bool BroadcastPoll { get; set; }
        [ProtoMember(214)]
        [DataMember]
        public string ToMemberIds { get; set; }
        [ProtoMember(215)]
        [DataMember]
        public int Voted { get; set; }
        /// <summary>
        /// count of folks who didn't vote
        /// </summary>
        [ProtoMember(216)]
        [DataMember]
        public int NonVotes { get; set; }
        [ProtoMember(217)]
        [DataMember]
        public string LeagueName { get; set; }
        [ProtoMember(218)]
        [DataMember]
        public string LeagueId { get; set; }
        [ProtoMember(219)]
        [DataMember]
        public string Comment { get; set; }
        [ProtoMember(220)]
        [DataMember]
        public List<MemberDisplayBasic> MembersDidntVote { get; set; }
        [ProtoMember(221)]
        [DataMember]
        public List<VotingQuestionClass> Questions { get; set; }
        [ProtoMember(222)]
        [DataMember]
        public List<MemberDisplayBasic> Voters { get; set; }

        [ProtoMember(223)]
        [DataMember]
        public long AnswerId { get; set; }

        [ProtoMember(224)]
        [DataMember]
        public bool CanEditPoll { get; set; }
        [ProtoMember(225)]
        [DataMember]
        public bool CanViewPoll { get; set; }
        [ProtoMember(226)]
        [DataMember]
        public bool CanVotePoll { get; set; }

        [ProtoMember(227)]
        [DataMember]
        public bool DidCurrentMemberVoted { get; set; }

        /// RDN-2319-Make Poll totally Secret
        [ProtoMember(228)]
        [DataMember]
        public bool OnlyShowResults { get; set; }


        //[ProtoMember(23)]
        //[DataMember]
        //[Obsolete]
        //public List<VotesClass> Votes { get; set; }
        //[ProtoMember(24)]
        //[DataMember]
        //[Obsolete]
        //public List<VotingAnswersClass> Answers { get; set; }
        public VotingClass()
        {
            //Answers = new List<VotingAnswersClass>();
            //Votes = new List<VotesClass>();
            Questions = new List<VotingQuestionClass>();
            Voters = new List<MemberDisplayBasic>();
        }
        //[Obsolete]
        //public int GetPercentage(long answerId)
        //{
        //    if (Votes.Count > 0)
        //        return Votes.Where(x => x.AnswerId == answerId).Count() * 100 / Votes.Count;
        //    else
        //        return 0;
        //}
        public int GetPercentageV2(int totalAnswers, int totalVoters)
        {

            return totalAnswers * 100 / totalVoters;

        }

        //public int GetBarLength(long answerId, int controlWidth)
        //{
        //    int percentage = GetPercentage(answerId);
        //    return percentage * controlWidth / 100;
        //}



    }
}
