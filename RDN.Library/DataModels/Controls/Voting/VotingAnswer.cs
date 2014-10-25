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
    [Table("RDN_Voting_Answer")]
    public class VotingAnswer : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AnswerId { get; set; }
        public bool WasRemoved { get; set; }

        public string Answer { get; set; }
        [Obsolete("Use Question 2013")]
        public virtual Voting Voting { get; set; }
        public virtual VotingQuestion Question { get; set; }

        public VotingAnswer()
        {

        }

    }
}
