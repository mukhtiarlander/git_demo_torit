using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_Categories")]
    public class StoreItemCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoreItemCategoryId { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        public long ParentCategory { get; set; }

        public virtual List<StoreItem> Items { get; set; }

        public virtual Merchant Merchant { get; set; }

        public StoreItemCategory()
        {
            Items = new List<StoreItem>();
        }
    }
}
