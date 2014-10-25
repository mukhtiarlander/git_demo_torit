using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Location
{
    [Table("RDN_Location")]
    public class Location : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LocationId { get; set; }
        [Required]
        public string LocationName { get; set; }
        public bool IsRemoved { get; set; }
        public virtual ContactCard.ContactCard Contact { get; set; }

        public Location()
        {
            
        }

    }
}
