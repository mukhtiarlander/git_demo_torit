using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Plan")]
    public class StripePlanDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripePlanId { get; set; }

        public int? AmountInCents { get; set; }
        public string Currency { get; set; }
        public string Id { get; set; }
        public string Interval { get; set; }
        public int IntervalCount { get; set; }
        public bool? LiveMode { get; set; }
        public string Name { get; set; }
        public int? TrialPeriodDays { get; set; }
        
    }
}
