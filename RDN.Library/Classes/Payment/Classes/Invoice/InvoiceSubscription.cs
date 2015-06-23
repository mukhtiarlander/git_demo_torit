using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.DataModels.Base;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoiceSubscription
    {
        public static readonly int NUMBER_OF_DAYS_FOR_TRIAL_SUBSCRIPTION = 45;

        public Guid InternalObject { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DigitalPurchaseText { get; set; }
        public string NameRecurring { get; set; }
        public string DescriptionRecurring { get; set; }
        public string ArticleNumber { get; set; }
        public string PlanId { get; set; }
        public decimal Price { get; set; }
        /// <summary>
        /// The number of days their subscription is prolonged. Add 31 for 1 month.
        /// </summary>
        public int SubscriptionPeriodLengthInDays { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime Created { get; set; }

        public SubscriptionPeriod SubscriptionPeriod { get; set; }
        public SubscriptionPeriodStripe SubscriptionPeriodStripe { get; set; }

        public InvoiceSubscription()
        {
            SubscriptionPeriod = SubscriptionPeriod.Monthly;
            SubscriptionPeriodLengthInDays = 31;
            ValidUntil = DateTime.Now.AddDays(31);
        }
    }
}
