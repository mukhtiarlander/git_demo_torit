using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Store
{
    /// <summary>
    /// attributes we collect on checkout.  These are the attributes the sellers might want to 
    /// collect that we don't offer to collect such as
    /// *Font Type:
    ///*Your Name:
    ///*Number:
    /// </summary>
    [Table("RDN_Store_Item_Attributes")]
    public class StoreItemAttribute : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AttributeId { get; set; }

        public string AttributeName { get; set; }

        public virtual StoreItem StoreItem { get; set; }
    }
}
