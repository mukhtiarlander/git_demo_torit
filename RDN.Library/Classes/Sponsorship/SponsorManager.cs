using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sponsors.Classes;
using RDN.Portable.Classes.Sponsorship;

namespace RDN.Library.Classes.Sponsorship
{
    public class SponsorManager
    {
        public static List<SponsorshipDisplay> DisplaySponsorships(Guid leagueId)
        {
            return SponsorShipManager.GetSponsorshipList(leagueId).Select(x => new SponsorshipDisplay()
            {
                Description = x.Description,
                ExpiresDate = x.ExpiresDate,
                IsRemoved = x.IsRemoved,
                OwnerId = x.OwnerId,
                Price = x.Price,
                SponsorshipId = x.SponsorshipId,
                Name = x.Name

            }).ToList();
        }
    }
}
