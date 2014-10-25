using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Store.Classes
{
    public class StoreShoppingCartContactInfo
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public string CompanyName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Street { get; set; }

        public string Street2 { get; set; }
        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        [Required]
        [MaxLength(50)]
        public string Zip { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }
    }
}
