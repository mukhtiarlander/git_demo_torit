using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Member;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Team
{
    [Table("RDN_DerbyRoster_League_Scan")]
    public class DerbyRosterLeague : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamId { get; set; }

        public string Name { get; set; }

        public string WebSite { get; set; }
        public string State { get; set; }
        public string Country{ get; set; }

        #region Constructor
        public DerbyRosterLeague()
        {
            Skaters = new Collection<TwoEvilsProfile>();
        }
        #endregion

        public virtual ICollection<TwoEvilsProfile> Skaters { get; set; }
    }
}
