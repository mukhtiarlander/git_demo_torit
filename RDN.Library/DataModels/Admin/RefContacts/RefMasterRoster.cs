using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RDN.Library.DataModels.Admin.RefContacts
{
    /// <summary>
    ///     https://docs.google.com/spreadsheet/ccc?key=0AusjG0WM8t3wdEl0TFpRTE1oaEFxU3dkeml0b2hzSEE&hl=en_US#gid=0
    /// </summary>
    [Table("RDN_Admin_Ref_Roster")]
    public class RefMasterRoster
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DerbyName { get; set; }

        public string LegalName { get; set; }
        public string Affiliation { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        public string Country { get; set; }
        public string HeadRef { get; set; }

        public string JamRef { get; set; }
        public string IPR { get; set; }
        public string OPR { get; set; }
        public string OfficialSince { get; set; }
        public string WFTDACertified { get; set; }
        public string Email { get; set; }
        public string FacebookLink { get; set; }


        public string Resume { get; set; }

    }
}
