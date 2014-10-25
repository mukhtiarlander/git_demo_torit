using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Voting
{
    public class VotesClass
    {
        public long VoteId { get; set; }

        public string OtherText { get; set; }

        public bool HasVoted { get; set; }
        public string IPAddress { get; set; }
        public long AnswerId { get; set; }
        public List<long> AnswerIds { get; set; }
        public Guid MemberId { get; set; }
        public string DerbyName { get; set; }
        public Guid UserId { get; set; }
        public DateTime Created { get; set; }
        public VotesClass()
        {
            AnswerIds = new List<long>();
        }
    }
}
