using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDN.League.Models.League
{
    public class Sponsorship
    {
        public long SponsorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PromoCode { get; set; }
        public string Website { get; set; }
        public bool IsDeleted { get; set; }
        public long UsedCount { get; set; }

        #region References
        public virtual RDN.Library.DataModels.League.League OwnerLeague { get; set; }
        #endregion
    }
}