using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Member;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Team
{
    [Table("RDN_TwoEvils_League_Scan")]
    public class TwoEvilsLeague : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TeamId { get; set; }

        public string Name { get; set; }
        [MaxLength(255)]
        public string Date { get; set; }


        #region Constructor
        public TwoEvilsLeague()
        {
            Skaters = new Collection<TwoEvilsProfile>();
        }
        #endregion

        public virtual ICollection<TwoEvilsProfile> Skaters { get; set; }
    }
}
