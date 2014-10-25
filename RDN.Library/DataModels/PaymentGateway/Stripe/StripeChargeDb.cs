using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Charge")]
    public class StripeChargeDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripeChargeId { get; set; }

        public int? AmountInCents { get; set; }
        public int? AmountInCentsRefunded { get; set; }
        public DateTime Created { get; set; }
        public string Currency { get; set; }

        public string Description { get; set; }
        public string FailureMessage { get; set; }

        public int? FeeInCents { get; set; }
        public string Id { get; set; }

        public bool? LiveMode { get; set; }
        public bool? Paid { get; set; }
        public bool? Refunded { get; set; }
        public virtual StripeCardDb StripeCard { get; set; }
        public virtual StripeCustomerDb Customer { get; set; }
        public virtual StripeInvoiceDb Invoice { get; set; }
        public virtual ICollection<StripeFeeDb> FeeDetails { get; set; }

        public StripeChargeDb()
        {
            FeeDetails = new Collection<StripeFeeDb>();
        }
    }
}
