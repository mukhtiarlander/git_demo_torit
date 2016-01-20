using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.League
{
    [Obsolete]
    public class Sponsor
    {
        public long SponsorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PromoCode { get; set; }
        public string Website { get; set; }
        public bool IsDeleted { get; set; }
        public long UsedCount { get; set; }

        public Guid SponsorForLeague { get; set; }
        public Guid SponsorAddByMember { get; set; }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Sponsored { get; set; }

        public Sponsor()
        {

        }

        public static bool Add_New_Sponsor(RDN.Library.Classes.League.Sponsor NewSponsor)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.League.Sponsorship con = new DataModels.League.Sponsorship();
                con.Description = NewSponsor.Description;
                con.PromoCode = NewSponsor.PromoCode;
                con.Name = NewSponsor.Name;
                con.Website = NewSponsor.Website;
                if (NewSponsor.BeginDate != new DateTime())
                    con.BeginDate = NewSponsor.BeginDate;
                if (NewSponsor.EndDate != new DateTime())
                    con.EndDate = NewSponsor.EndDate;
                con.Sponsored = NewSponsor.Sponsored;

                con.SponsorForLeague = dc.Leagues.Where(x => x.LeagueId == NewSponsor.SponsorForLeague).FirstOrDefault();
                con.SponsorAddByMember = dc.Members.Where(x => x.MemberId == NewSponsor.SponsorAddByMember).FirstOrDefault();
                dc.Sponsorships.Add(con);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: NewSponsor.EndDate + ":" + NewSponsor.BeginDate);

            }
            return false;
        }

        public static List<Sponsor> GetSponsorList(Guid leagueId)
        {
            List<Sponsor> SponsorList = new List<Sponsor>();
            try
            {
                var dc = new ManagementContext();
                var SponsorshipList = dc.Sponsorships.Where(x => x.SponsorForLeague.LeagueId == leagueId && x.IsDeleted == false).ToList();

                foreach (var b in SponsorshipList)
                {
                    SponsorList.Add(DisplaySponsorList(b));
                }
                return SponsorList;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return SponsorList;
        }

        private static Sponsor DisplaySponsorList(DataModels.League.Sponsorship oSponsor)
        {
            Sponsor bl = new Sponsor();
            bl.SponsorId = oSponsor.SponsorId;
            bl.Name = oSponsor.Name;
            bl.SponsorForLeague = oSponsor.SponsorForLeague.LeagueId;
            bl.IsDeleted = oSponsor.IsDeleted;
            bl.SponsorAddByMember = oSponsor.SponsorAddByMember.MemberId;
            bl.PromoCode = oSponsor.PromoCode;
            bl.Description = oSponsor.Description;
            bl.UsedCount = oSponsor.UsedCount;
            bl.Website = oSponsor.Website;
            bl.BeginDate = oSponsor.BeginDate.GetValueOrDefault();
            bl.EndDate = oSponsor.EndDate.GetValueOrDefault();
            bl.Sponsored = oSponsor.Sponsored;

            return bl;
        }

        /// <summary>
        /// This function used for Edit and View Details
        /// </summary>
        /// <param name="sponsorId"></param>
        /// <param name="sponsorForleagueId"></param>
        /// <returns>Sponsorship details</returns>
        public static Sponsor GetData(long sponsorId, Guid sponsorForleagueId)//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var sponsorInfo = dc.Sponsorships.Where(x => x.SponsorId == sponsorId && x.SponsorForLeague.LeagueId == sponsorForleagueId).FirstOrDefault();
                if (sponsorInfo != null)
                {
                    return DisplaySponsorList(sponsorInfo);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static bool UpdateSponsorInfo(RDN.Library.Classes.League.Sponsor SponsorToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbSponsor = dc.Sponsorships.Where(x => x.SponsorId == SponsorToUpdate.SponsorId).FirstOrDefault();
                if (dbSponsor == null)
                    return false;

                dbSponsor.Name = SponsorToUpdate.Name;
                dbSponsor.PromoCode = SponsorToUpdate.PromoCode;
                dbSponsor.Description = SponsorToUpdate.Description;
                dbSponsor.Website = SponsorToUpdate.Website;
                dbSponsor.BeginDate = SponsorToUpdate.BeginDate;
                dbSponsor.EndDate = SponsorToUpdate.EndDate;
                dbSponsor.Sponsored = SponsorToUpdate.Sponsored;

                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteSponsor(long sponsorId, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbSponsor = dc.Sponsorships.Where(x => x.SponsorId == sponsorId && x.SponsorForLeague.LeagueId == leagueId).FirstOrDefault();
                if (dbSponsor == null)
                    return false;
                dbSponsor.IsDeleted = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool UseCode(long sponsorId, Guid leagueId, out long usedCount)
        {
            usedCount = -1;
            try
            {
                var dc = new ManagementContext();
                var dbSponsor = dc.Sponsorships.Where(x => x.SponsorId == sponsorId && x.SponsorForLeague.LeagueId == leagueId).FirstOrDefault();
                if (dbSponsor == null)
                    return false;
                dbSponsor.UsedCount = dbSponsor.UsedCount + 1;
                usedCount = dbSponsor.UsedCount;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool UseCode(long sponsorId, Guid leagueId)
        {
            long usedCount;
            return UseCode(sponsorId, leagueId, out usedCount);
        }

    }
}