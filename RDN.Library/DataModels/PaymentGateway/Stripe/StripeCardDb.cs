using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Stripe
{
    [Table("RDN_Stripe_Customer_Card")]
    public class StripeCardDb : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StripeCardId { get; set; }

        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine1Check { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressState { get; set; }
        public string AddressZip { get; set; }
        public string AddressZipCheck { get; set; }
        public string Country { get; set; }
        public string CvcCheck { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string Fingerprint { get; set; }
        public string Last4 { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
