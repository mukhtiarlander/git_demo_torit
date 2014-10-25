using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Team;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Member
{
    [Table("RDN_TwoEvils_Profile_Scan")]
    public class TwoEvilsProfile : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProfileId { get; set; }

        public string Name { get; set; }
        [MaxLength(255)]
        public string Number { get; set; }
        [MaxLength(255)]
        public string Date { get; set; }



        #region Constructor
        public TwoEvilsProfile()
        {
        }
        #endregion

        public virtual TwoEvilsLeague League { get; set; }
        public virtual DerbyRosterLeague LeagueDerbyRoster { get; set; }
    }
}

