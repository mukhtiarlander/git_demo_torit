using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_ShoppingCarts")]
    public class ShoppingCart : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ShoppingCartId { get; set; }        
        [MaxLength(255)]
        public string Ip { get; set; }
        public Guid UserId { get; set; }

        //// Required to have a merchant bound to this. Can NOT shop from multiple shops at the same time
        //[Required]
        //public Merchant Merchant { get; set; }

        public DateTime Expires { get; set; }

        public virtual ShoppingCartContactInfo BillingAddress { get; set; }
        public virtual ShoppingCartContactInfo ShippingAddress { get; set; }

        public virtual IList<ShoppingCartItem> Items { get; set; }        

        public ShoppingCart()
        {
            Items = new List<ShoppingCartItem>();
            Expires = DateTime.Now.AddDays(1);
        }
    }
}
