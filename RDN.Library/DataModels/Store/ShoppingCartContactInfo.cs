using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Store
{
    [Table("RDN_Store_ShoppingCart_Addresses")]
    public class ShoppingCartContactInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceContactInfoId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
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
    }
}
