using RDN.Library.DataModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RDN.Library.DataModels.Social
{
    [Table("RN_Social_Auth_Keys")]
    public class SocialAuthKeys : InheritDb
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuthId { get; set; }

        public string LatestAuthToken { get; set; }

        public DateTime Expires { get; set; }

        public long SocialSiteType { get; set; }
    }
}
