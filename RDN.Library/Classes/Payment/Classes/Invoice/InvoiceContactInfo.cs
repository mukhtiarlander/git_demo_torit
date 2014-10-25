using System;
using System.Collections.Generic;
using RDN.Library.Classes.Payment.Enums;

namespace RDN.Library.Classes.Payment.Classes.Invoice
{
    public class InvoiceContactInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }        
        public string State { get; set; }
        public string StateCode { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }
}
