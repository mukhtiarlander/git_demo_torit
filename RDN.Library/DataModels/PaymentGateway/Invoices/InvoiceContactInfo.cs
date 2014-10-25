using System;
using System.ComponentModel.DataAnnotations;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.PaymentGateway.Invoices
{
    [Table("RDN_Invoice_Addresses")]
    public class InvoiceContactInfo //: InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceContactInfoId { get; set; }
        [MaxLength(255)]
        [Obsolete("Use First and Last Name")]
        public string Name { get; set; }
        [MaxLength(255)]
        public string FirstName { get; set; }
        [MaxLength(255)]
        public string LastName { get; set; }
        [MaxLength(255)]
        public string CompanyName { get; set; }
        [MaxLength(255)]
        public string Street { get; set; }
        [MaxLength(255)]
        public string Street2 { get; set; }
        [MaxLength(255)]
        public string State { get; set; }
        [MaxLength(255)]
        public string StateCode { get; set; }
        [MaxLength(255)]
        public string Zip { get; set; }
        [MaxLength(255)]
        public string City { get; set; }
        [MaxLength(255)]
        public string Country { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(255)]
        public string Phone { get; set; }
        [MaxLength(255)]
        public string Fax { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
