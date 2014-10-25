using RDN.Library.Classes.Payment.Enums;
using RDN.Portable.Classes.Payment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Billing.Classes
{
    public class AddSubscription
    {
        public Guid AddSubscriptionOwnerId { get; set; }
        public string AddSubscriptionOwnerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public string CCNumber { get; set; }
        public string SecurityCode { get; set; }
        public int MonthOfExpiration { get; set; }
        public int YearOfExpiration { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
    }
}
