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
    [Table("RDN_VotingV2")]
    public class VotingV2 : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VotingId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        //can the members of the league see it.
        public bool IsOpenToLeague { get; set; }
        public bool IsPollAnonymous { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosed { get; set; }
        public bool OnlyShowResults { get; set; } // RDN-2319-Make Poll totally Secret

        public virtual League.League LeagueOwner { get; set; }

        public virtual Collection<VotingQuestion> Questions { get; set; }
        public virtual Collection<VotingVoters> Voters { get; set; }
        public VotingV2()
        {
            Questions = new Collection<VotingQuestion>();
            Voters = new Collection<VotingVoters>();
        }

    }
}
