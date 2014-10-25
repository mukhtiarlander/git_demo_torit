using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.League;
using RDN.Portable.Classes.Federation.Enums;

namespace RDN.Library.Classes.League
{
    public class BoutList
    {

        public long ChallengeId { get; set; }

        public DateTime StartDateOfEvent { get; set; }
        public DateTime EndDateOfEvent { get; set; }
        public string Location { get; set; }
        public bool IsOfferTravelStipend { get; set; }
        public double CrowdSize { get; set; }
        public bool IsStreamLive { get; set; }
        public bool IsSkaterHousingOffered { get; set; }
        public bool IsHangOverBoutOffered { get; set; }
        public int RuleSetId { get; set; }
        public string RuleSetName { get; set; }
        public string TravelStipendAmount { get; set; }
        public string CurrentRanking { get; set; }
        public bool IsAwayGame { get; set; }
        public bool DisplayToPublic { get; set; }

        public string EventInformation { get; set; }
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; }

        public BoutList()
        {

        }
        public static long SaveBoutRequestList(Guid leagieId, RDN.Library.Classes.League.BoutList boutList)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.Bouts.BoutList con = new DataModels.Bouts.BoutList();
                con.Location = boutList.Location;
                con.RuleSetId = boutList.RuleSetId;
                con.TravelStipendAmount = boutList.TravelStipendAmount;
                con.IsStreamLive = boutList.IsStreamLive;
                con.IsSkaterHousingOffered = boutList.IsSkaterHousingOffered;
                con.IsOfferTravelStipend = boutList.IsOfferTravelStipend;
                con.IsHangOverBoutOffered = boutList.IsHangOverBoutOffered;
                con.EventInformation = boutList.EventInformation;
                con.DateOfEvent = boutList.StartDateOfEvent;
                con.CurrentRanking = boutList.CurrentRanking;
                con.CrowdSize = boutList.CrowdSize;
                con.IsPublic = boutList.DisplayToPublic;
                con.IsAwayGame = boutList.IsAwayGame;
                if (boutList.EndDateOfEvent > DateTime.UtcNow.AddYears(-1))
                    con.EndDateOfEvent = boutList.EndDateOfEvent;
                con.League = dc.Leagues.Where(x => x.LeagueId == leagieId).FirstOrDefault();

                dc.BoutLists.Add(con);

                int c = dc.SaveChanges();
                //return c > 0;
               return  con.ChallengeId;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: boutList.StartDateOfEvent + ":" + boutList.EndDateOfEvent);

            }
            return 0;
        }

        public static List<BoutList> GetBoutList()
        {
            List<BoutList> bouts = new List<BoutList>();
            try
            {
                var dateTime = DateTime.UtcNow.AddDays(-1);
                var dc = new ManagementContext();
                var BoutLists = dc.BoutLists.Where(x => x.IsClosed == false && x.IsDeleted == false && x.DateOfEvent > dateTime).ToList();

                foreach (var b in BoutLists)
                {
                    bouts.Add(DisplayBoutList(b));
                }
                return bouts;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return bouts;
        }
        public static int GetBoutListCount()
        {
            try
            {
                var dc = new ManagementContext();
                var dateTime = DateTime.UtcNow.AddDays(-1);
                return dc.BoutLists.Where(x => x.IsClosed == false && x.IsDeleted == false && x.DateOfEvent > dateTime).Count();

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        /// <summary>
        /// This GetData Edit is used for Edit and Event View Form.
        /// </summary>
        /// <param name="challengeId"></param>
        /// <returns></returns>
        public static BoutList GetData(long challengeId, Guid leagueId = new Guid())//This Function used for "Edit" and "Event View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var BoutLists = dc.BoutLists.Where(x => x.ChallengeId == challengeId && x.League.LeagueId == leagueId).FirstOrDefault();

                if ((leagueId == null || leagueId == Guid.Empty) && (challengeId != null)) //used for RDNation.com 
                {
                    BoutLists = dc.BoutLists.Where(x => x.ChallengeId == challengeId).FirstOrDefault();                    
                }

                if (BoutLists != null)
                {
                    return DisplayBoutList(BoutLists);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static BoutList DisplayBoutList(DataModels.Bouts.BoutList BoutLists)
        {
            BoutList bl = new BoutList();
            bl.ChallengeId = BoutLists.ChallengeId;
            bl.CrowdSize = BoutLists.CrowdSize;
            bl.CurrentRanking = BoutLists.CurrentRanking;
            bl.StartDateOfEvent = BoutLists.DateOfEvent;
            bl.EndDateOfEvent = BoutLists.EndDateOfEvent.GetValueOrDefault();
            bl.EventInformation = BoutLists.EventInformation;
            bl.IsHangOverBoutOffered = BoutLists.IsHangOverBoutOffered;
            bl.IsOfferTravelStipend = BoutLists.IsOfferTravelStipend;
            bl.IsSkaterHousingOffered = BoutLists.IsSkaterHousingOffered;
            bl.IsStreamLive = BoutLists.IsStreamLive;
            bl.LeagueId = BoutLists.League.LeagueId;
            bl.LeagueName = BoutLists.League.Name;
            bl.Location = BoutLists.Location;
            bl.RuleSetId = BoutLists.RuleSetId;
            bl.RuleSetName =RDN.Portable.Util.Enums.EnumExt.ToFreindlyName((RuleSetsUsedEnum)BoutLists.RuleSetId);
            bl.TravelStipendAmount = BoutLists.TravelStipendAmount;
            bl.IsAwayGame = BoutLists.IsAwayGame;
            bl.DisplayToPublic = BoutLists.IsPublic;
            return bl;
        }
        /// <summary>
        /// Update Event Information
        /// </summary>
        /// <param name="challengeId"></param>
        /// <param name="leagueId"></param>
        /// <param name="EventToUpdate"></param>
        /// <returns></returns>
        public static bool UpdateEvent(RDN.Library.Classes.League.BoutList EventToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbBoutList = dc.BoutLists.Where(x => x.ChallengeId == EventToUpdate.ChallengeId).FirstOrDefault();
                if (dbBoutList == null)
                    return false;

                dbBoutList.CrowdSize = EventToUpdate.CrowdSize;
                dbBoutList.CurrentRanking = EventToUpdate.CurrentRanking;
                dbBoutList.DateOfEvent = EventToUpdate.StartDateOfEvent;
                dbBoutList.EventInformation = EventToUpdate.EventInformation;
                dbBoutList.IsHangOverBoutOffered = EventToUpdate.IsHangOverBoutOffered;
                dbBoutList.IsOfferTravelStipend = EventToUpdate.IsOfferTravelStipend;
                dbBoutList.IsSkaterHousingOffered = EventToUpdate.IsSkaterHousingOffered;
                dbBoutList.IsStreamLive = EventToUpdate.IsStreamLive;
                dbBoutList.Location = EventToUpdate.Location;
                dbBoutList.RuleSetId = EventToUpdate.RuleSetId;
                dbBoutList.TravelStipendAmount = EventToUpdate.TravelStipendAmount;
                if (EventToUpdate.EndDateOfEvent > new DateTime())
                    dbBoutList.EndDateOfEvent = EventToUpdate.EndDateOfEvent;
                else
                    dbBoutList.EndDateOfEvent = null;
                dbBoutList.IsAwayGame = EventToUpdate.IsAwayGame;
                dbBoutList.IsPublic = EventToUpdate.DisplayToPublic;


                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool CloseEvent(long challengeId, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbBoutList = dc.BoutLists.Where(x => x.ChallengeId == challengeId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (dbBoutList == null)
                    return false;
                dbBoutList.IsClosed = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool DeleteEvent(long challengeId, Guid leagueId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbBoutList = dc.BoutLists.Where(x => x.ChallengeId == challengeId && x.League.LeagueId == leagueId).FirstOrDefault();
                if (dbBoutList == null)
                    return false;
                dbBoutList.IsDeleted = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

    }
}
