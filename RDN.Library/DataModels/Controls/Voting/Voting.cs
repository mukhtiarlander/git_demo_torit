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
    [Obsolete("Use VotingV2")]
    [Table("RDN_Voting")]
    public class Voting : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VotingId { get; set; }

        public string Question { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPollAnonymous { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosed { get; set; }
        public bool OnlyShowResults { get; set; } /// RDN-2319-Make Poll totally Secret

        public virtual League.League LeagueOwner { get; set; }

        public virtual Collection<Votes> Votes { get; set; }
        public virtual Collection<VotingAnswer> Answers { get; set; }
        public Voting()
        {
            Answers = new Collection<VotingAnswer>();
            Votes = new Collection<Votes>();
        }

    }
}
