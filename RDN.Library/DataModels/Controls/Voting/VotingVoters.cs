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
    [Table("RDN_Voting_Voters")]
    public class VotingVoters : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VoteId { get; set; }
        //did this person actually vote yet.
        public bool HasVoted { get; set; }

        public virtual Member.Member Member { get; set; }
        public virtual VotingV2 Voting{ get; set; }


        public VotingVoters()
        {

        }

    }
}
