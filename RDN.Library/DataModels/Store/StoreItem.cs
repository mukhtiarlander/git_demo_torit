using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_Items")]
    public class StoreItem : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoreItemId { get; set; }

        public string Name { get; set; }
        [MaxLength(50)]
        public string ArticleNumber { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public decimal ShippingCosts { get; set; }
        public decimal ShippingCostsAdditional { get; set; }
        public bool VisibleOnRdn { get; set; }
        public int QuantityInStock { get; set; }
        public bool CanRunOutOfStock { get; set; }
        public bool CanPickUpLocally { get; set; }
        public bool ExemptFromShipping { get; set; }
        public bool IsPublished { get; set; }
        public int ItemTypeEnum { get; set; }
        public long SizesEnum { get; set; }
        //date last published.  So we can charge users automatically for keeping an item published.
        public DateTime? LastPublished { get; set; }
        // Note about whatever, like extra long delivery time or something
        public string Note { get; set; }
        //views count of item.
        public int Views { get; set; }
        //amount of times item has been bought.
        public int Bought { get; set; }
        //merch was notified of the items expiration.
        public bool NotifiedOfExpiration { get; set; }
        //merch was charged a listing fee automatically
        public bool ChargedNewListingFee { get; set; }

        public StoreItem()
        {
            Photos = new Collection<StoreItemPhoto>();
            Colors = new Collection<StoreItemColor>();
            Attributes = new Collection<StoreItemAttribute>();
            Reviews = new Collection<Review>();
        }

        [Required]
        public virtual Merchant Merchant { get; set; }
        public virtual StoreItemCategory Category { get; set; }
        public virtual ICollection<StoreItemPhoto> Photos { get; set; }
        public virtual ICollection<StoreItemColor> Colors { get; set; }
        public virtual ICollection<StoreItemAttribute> Attributes { get; set; }
		public virtual ICollection<Review> Reviews { get; set; }
    }

}
