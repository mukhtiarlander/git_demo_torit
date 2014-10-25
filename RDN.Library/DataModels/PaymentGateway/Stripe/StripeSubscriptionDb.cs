using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Subscription")]
    public class StripeSubscriptionDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripeSubscriptionId { get; set; }

        public DateTime? CanceledAt { get; set; }
        public StripeCustomerDb Customer{ get; set; }
        public DateTime? EndedAt { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public DateTime? PeriodStart { get; set; }
        public int Quantity { get; set; }
        public DateTime? Start { get; set; }
        public string Status { get; set; }
        public virtual StripePlanDb StripePlan { get; set; }
        public DateTime? TrialEnd { get; set; }
        public DateTime? TrialStart { get; set; }
        
    }
}
