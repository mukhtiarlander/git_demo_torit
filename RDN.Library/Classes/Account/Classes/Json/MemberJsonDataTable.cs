using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Member;
using RDN.Utilities.Config;

namespace RDN.Library.Classes.Account.Classes.Json
{
    [Obsolete("Use RDN.Portable.Models.Json.SkaterJson")]
    public class MemberJsonDataTable 
    {
        public string photoUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string DerbyNameUrl { get; set; }
        public string DerbyName { get; set; }
        public string DerbyNumber { get; set; }
        public string Gender { get; set; }
        public string LeagueName { get; set; }
        public string LeagueUrl { get; set; }
        public string MemberId { get; set; }
        public string LeagueId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}
