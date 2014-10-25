using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.League
{
    [Table("RDN_League_ItemInfo")]
    public class ItemInfo : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ItemId { get; set; }
        public string ItemSerialNo { get; set; }
        public string ItemName { get; set; }
        public string UnitePice { get; set; }
        public string Location { get; set; }
        public string Quantity { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }

        #region References
        public virtual RDN.Library.DataModels.League.League ItemsForLeague { get; set; }
        public virtual RDN.Library.DataModels.Member.Member ItemAddByMember { get; set; }
        
        #endregion
    }
}
