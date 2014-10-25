using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Event")]
    public class StripeEventDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripeEventId { get; set; }

        public string StripeId { get; set; }

        public byte StripeEventTypeEnum { get; set; }

        public bool LiveMode { get; set; }

        public DateTime CreatedStripeDate { get; set; }
        public StripeCustomerDb Customer { get; set; }
        public StripeInvoiceDb Invoice{ get; set; }
        public StripeChargeDb Charge { get; set; }
        public StripeSubscriptionDb Subscription { get; set; }


    }
}
