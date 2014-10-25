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
    /// <summary>
    /// answers selected with the votes.
    /// </summary>
    [Table("RDN_Voting_Votes_Answers_Selected")]
    public class VotesAnswersSelected : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VoteSelectedAnswerId { get; set; }
        //did this person actually vote yet.
        public virtual VotingAnswer AnswerSelected { get; set; }
        public virtual Votes Vote { get; set; }

        public VotesAnswersSelected()
        {
        }

    }
}
