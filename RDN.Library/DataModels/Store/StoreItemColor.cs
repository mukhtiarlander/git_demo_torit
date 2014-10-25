using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_Item_Colors")]
    public class StoreItemColor : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ColorId { get; set; }

        public virtual Color.Color Color { get; set; }

        public virtual StoreItem StoreItem { get; set; }
    }
}
