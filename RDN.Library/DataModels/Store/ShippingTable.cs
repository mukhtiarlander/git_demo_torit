using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.PaymentGateway.Merchants;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_ShippingTable")]
    public class ShippingTable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShippingTableId { get; set; }
        // Carrier type. See classes->enums->Shipping type enum
        [Required]
        public byte ShippingType { get; set; }
        [Required]
        public double WeightUpTo { get; set; }
        [Required]
        public double WeightFrom { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public Merchant Merchant { get; set; }
    }
}
