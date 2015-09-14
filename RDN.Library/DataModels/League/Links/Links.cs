using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDN.Library.DataModels.Base;

namespace RDN.Library.DataModels.League.Links
{
     [Table("RDN_League_Links")]
    public class Links : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LinkId { get; set; }        
        public string Link { get; set; }
        public string Notes { get; set; }
        public bool IsRemoved { set; get; }

        #region References
        public virtual RDN.Library.DataModels.League.League LinksForLeague { get; set; }
        public virtual RDN.Library.DataModels.Member.Member LinksAddByMember { get; set; }
        #endregion
    }
}
