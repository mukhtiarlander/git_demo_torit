using RDN.Library.Classes.Account.Classes;
using RDN.Library.Classes.Controls.Voting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Voting
{
    public class VotingQuestionClass
    {
        public long QuestionId { get; set; }
        public int SortableOrderId { get; set; }
        public string Question { get; set; }
        public QuestionTypeEnum QuestionType { get; set; }
        public List<VotesClass> Votes { get; set; }
        public List<VotingAnswersClass> Answers { get; set; }

        public VotingQuestionClass()
        {
            Answers = new List<VotingAnswersClass>();
            Votes = new List<VotesClass>();
        }

        public int GetPercentage(long answerId)
        {
            if (Votes.Count > 0)
                return Votes.Where(x => x.AnswerIds.Contains(answerId)).Count() * 100 / Votes.Count;
            else
                return 0;
        }

        public int GetBarLength(long answerId, int controlWidth)
        {
            int percentage = GetPercentage(answerId);
            return percentage * controlWidth / 100;
        }



    }
}
