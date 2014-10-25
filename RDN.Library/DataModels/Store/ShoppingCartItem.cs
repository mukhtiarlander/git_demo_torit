using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_ShoppingCart_Items")]
    public class ShoppingCartItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShoppingCartItemId { get; set; }        
        [Required]
        public int Quantity { get; set; }
        /// <summary>
        /// in case it has a size, small medium and large.
        /// </summary>
        public long Size { get; set; }

        public int Color { get; set; }
        public bool WillPickUpLocally { get; set; }

        [Required]
        public StoreItem StoreItem { get; set; }

        public virtual ShoppingCart Cart { get; set; }
    }
}
