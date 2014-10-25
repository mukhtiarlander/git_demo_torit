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
    [Table("RDN_Voting_Question")]
    public class VotingQuestion : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long QuestionId { get; set; }

        public string Question { get; set; }
        public int QuestionSortId { get; set; }
        public int QuestionType { get; set; }
        //public bool IsRemoved { get; set; }

        public virtual Collection<Votes> Votes { get; set; }
        public virtual Collection<VotingAnswer> Answers { get; set; }
        public VotingQuestion()
        {
            Answers = new Collection<VotingAnswer>();
            Votes = new Collection<Votes>();
        }

    }
}
