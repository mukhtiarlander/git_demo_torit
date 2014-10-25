using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Controls.Voting
{
    [Table("RDN_Voting_Votes")]
    public class Votes : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VoteId { get; set; }
        //did this person actually vote yet.
        [Obsolete("tracking of recipients has been moved to VotingVoters 2013")]
        public bool HasVoted { get; set; }
        public string OtherText { get; set; }
        public string IPAddress { get; set; }
        [Obsolete("Use AnswersSelected 2014")]
        public virtual VotingAnswer AnswerSelected { get; set; }
        public virtual List<VotesAnswersSelected> AnswersSelected { get; set; }

        public virtual Member.Member Member { get; set; }
        [Obsolete("Use Question 2013")]
        public virtual Voting Voting { get; set; }
        public virtual VotingQuestion Question { get; set; }


        public Votes()
        {
            AnswersSelected = new List<VotesAnswersSelected>();
        }

    }
}
