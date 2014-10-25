using System;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.Location;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.ContactCard
{
    [Table("RDN_ContactCard_Addresses")]
    public class Address : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityRaw { get; set; }
        public string StateRaw { get; set; }
        public string Zip { get; set; }
        // in ContactCard.Enums.AddressType
        public byte AddressType { get; set; }

        public double  TimeZone { get; set; }
        public bool IsDefault { get; set; }

        public GeoCoordinate Coords { get; set; }
        #region References
        
        [Required]
        public virtual ContactCard ContactCard { get; set; }        
        public virtual Country Country { get; set; }

        #endregion
    }
}
