using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Enums.Paywall;
using RDN.Library.DataModels.Base;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoicePaywall
    {

        public Guid InternalObject { get; set; }
        public long PaywallId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        /// <summary>
        /// The number of days their subscription is prolonged. Add 31 for 1 month.
        /// </summary>
        public int PaywallLengthOfDays { get; set; }
        public string ValidUntilDisplay { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime Created { get; set; }
        public Guid MemberPaidId { get; set; }
        public string PaywallPassword { get; set; }
        public PaywallPriceTypeEnum PriceType { get; set; }
        public string PaywallLocation { get; set; }
        public long SecondsViewedPaywall { get; set; }
        public long TimesUsedPassword { get; set; }
        public DateTime? LastViewedPaywall { get; set; }

        public InvoicePaywall()
        {
            PriceType = PaywallPriceTypeEnum.Daily_Payment;
            PaywallLengthOfDays = 1;
            ValidUntil = DateTime.Now.AddDays(1);
        }
    }
}
