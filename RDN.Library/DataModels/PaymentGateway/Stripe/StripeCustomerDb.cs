using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Customer")]
    public class StripeCustomerDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripeCustomerId { get; set; }

        public string Email { get; set; }
        public bool LiveMode { get; set; }
        public DateTime CreatedByStripe { get; set; }

        
        public bool? Deleted { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public StripeCardDb StripeCard { get; set; }
        //public StripeDiscount StripeDiscount { get; set; }
        //public StripeNextRecurringCharge StripeNextRecurringCharge { get; set; }
        //public StripeSubscription StripeSubscription { get; set; }
    }
}
