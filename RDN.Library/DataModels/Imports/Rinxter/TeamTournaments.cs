using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Imports.Rinxter
{
    [Table("RDN_Rinxter_TeamTournaments")]
    public class RTeamTournaments : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TeamTournamentId { get; set; }
        public int ID { get; set; }
        public int TournamentsID { get; set; }
        public int TeamID { get; set; }

        public virtual RTeams Teams { get; set; }
        public virtual RTournaments Tournaments { get; set; }
    }
}
