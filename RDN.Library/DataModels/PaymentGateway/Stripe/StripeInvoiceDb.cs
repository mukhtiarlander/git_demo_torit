using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Invoice")]
    public class StripeInvoiceDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripeInvoiceId { get; set; }

        public int? AmountDueInCents { get; set; }
        public int? AttemptCount { get; set; }
        public bool? Attempted { get; set; }
        public bool? Closed { get; set; }

        public DateTime? Date { get; set; }
        public int? EndingBalanceInCents { get; set; }
        public string Id { get; set; }
        public bool? LiveMode { get; set; }
        public DateTime? NextPaymentAttempt { get; set; }
        public string Object { get; set; }
        public bool? Paid { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public DateTime? PeriodStart { get; set; }
        public int? StartingBalanceInCents { get; set; }

        public int? SubtotalInCents { get; set; }
        public int? TotalInCents { get; set; }
        public virtual StripeCustomerDb Customer { get; set; }
        public virtual StripePlanDb SubscriptionPlan { get; set; }
        //public virtual StripeChargeDb ChargeId { get; set; }
        public string ChargeId { get; set; }

    }
}
