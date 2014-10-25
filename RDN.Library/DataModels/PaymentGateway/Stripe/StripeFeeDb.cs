using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Fee")]
    public class StripeFeeDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripeFeeId { get; set; }

        public int AmountInCents { get; set; }
        public string Application { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string type { get; set; }
    }
}
