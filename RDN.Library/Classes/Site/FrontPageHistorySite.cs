using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Site
{

    public class FrontPageHistorySite
    {
        public Guid LeagueId { get; set; }
        public Guid MemberId { get; set; }


        public static FrontPageHistorySite GetLatestSiteHistory()
        {
            ManagementContext db = new ManagementContext();
            Guid fake = new Guid();
            var league = db.FrontPageHistory.OrderByDescending(x => x.HistoryId).Where(x => x.LeagueId != fake).FirstOrDefault();
            FrontPageHistorySite site = new FrontPageHistorySite();
            if (league != null)
            {
                site.LeagueId = league.LeagueId;
            }
            var mem = db.FrontPageHistory.OrderByDescending(x => x.HistoryId).Where(x => x.MemberId != fake).FirstOrDefault();
            if (league != null)
            {
                site.MemberId = mem.MemberId;
            }
            return site;
        }
    }
}
