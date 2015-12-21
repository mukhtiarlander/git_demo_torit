using Common.EmailServer.Library.Classes.Subscribe;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.EmailServer.Enums;
using RDN.Library.DataModels.EmailServer.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.EmailServer
{
    public class SubscriptionFactory
    {

        public long listId { get; set; }
        public string ListName { get; set; }
        public Guid LeagueOwner { get; set; }
        public Guid owner { get; set; }
        public bool IsRemoved { get; set; }

        #region Subscriber

        public long SubscriberId { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public long EmailsSent { get; set; }
        public long SubscriberTypeEnum { get; set; }
        public bool OptedOut { get; set; }
        public int BounceCount { get; set; }
        public bool OptedOutDateTime { get; set; }

        #endregion Subscriber

        #region Email Blast

        public long BlastId { get; set; }
        public string BlastTitle { get; set; }
        public long SubscriptionId { get; set; }
        public string EmailBody { get; set; }
        public bool TestEmail { get; set; }
        public string TestEmailField { get; set; }

        #endregion Email Blast


        public static SubscriptionFactory Initialize()
        {
            return new SubscriptionFactory();
        }

        public SubscriptionFactory SetListId(long listId)
        {
            this.listId = listId;
            return this;
        }




        //public bool AddSubscriber(SubscriberTypeEnum type, string data)
        //{
        //    try
        //    {
        //        var dc = new ManagementContext();
        //        Subscriber subscriber = new Subscriber();
        //        subscriber.Data = data;
        //        subscriber.Name = "";
        //        // subscriber.SubscriberTypeEnum = (long)type;
        //        subscriber.List = dc.SubscriptionLists.Where(x => x.ListId == this.listId).FirstOrDefault();

        //        dc.SubscriptionSubscriber.Add(subscriber);
        //        int c = dc.SaveChanges();

        //        return c > 0;
        //    }
        //    catch (Exception exception)
        //    {
        //        ErrorDatabaseManager.AddException(exception, GetType());
        //    }
        //    return false;
        //}


        public static bool Add_New_SubscriptionList(RDN.Library.Classes.EmailServer.SubscriptionFactory NewList)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.EmailServer.Subscriptions.SubscriptionList con = new DataModels.EmailServer.Subscriptions.SubscriptionList();
                con.ListName = NewList.ListName;
                con.SubscriberTypeEnum = NewList.SubscriberTypeEnum;
                con.IsRemoved = false;
                dc.SubscriptionLists.Add(con);


                //DataModels.EmailServer.Subscriptions.SubscriptionOwner ownerCon = new DataModels.EmailServer.Subscriptions.SubscriptionOwner();
                //ownerCon.LeagueOwner = dc.Leagues.Where(x => x.LeagueId == NewList.LeagueOwner).FirstOrDefault();
                //ownerCon.Owner = dc.Members.Where(x => x.MemberId == NewList.owner).FirstOrDefault();

                //ownerCon.IsRemoved = false;

                //dc.SubscriptionOwners.Add(ownerCon);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }

        public static List<SubscriptionFactory> GetSubscriberList()
        {
            List<SubscriptionFactory> List = new List<SubscriptionFactory>();
            try
            {
                var dc = new ManagementContext();
                var SubscriptionList = dc.SubscriptionLists.Where(x => x.IsRemoved == false).ToList();

                foreach (var b in SubscriptionList)
                {
                    List.Add(DisplayData(b));
                }
                return List;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return List;
        }

        private static SubscriptionFactory DisplayData(DataModels.EmailServer.Subscriptions.SubscriptionList oData)
        {
            SubscriptionFactory bl = new SubscriptionFactory();
            bl.listId = oData.ListId;
            bl.ListName = oData.ListName;
            bl.SubscriberTypeEnum = oData.SubscriberTypeEnum;
            bl.IsRemoved = oData.IsRemoved;

            return bl;
        }

        /// <summary>
        /// This function used for Edit and View Details
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns>details</returns>
        public static SubscriptionFactory GetData(long ListId)//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var listInfo = dc.SubscriptionLists.Where(x => x.ListId == ListId && x.IsRemoved == false).FirstOrDefault();
                if (listInfo != null)
                {
                    return DisplayData(listInfo);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static bool UpdateListInfo(RDN.Library.Classes.EmailServer.SubscriptionFactory DataToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbData = dc.SubscriptionLists.Where(x => x.ListId == DataToUpdate.listId).FirstOrDefault();
                if (dbData == null)
                    return false;

                dbData.ListName = DataToUpdate.ListName;

                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool RemoveList(long ListId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbList = dc.SubscriptionLists.Where(x => x.ListId == ListId && x.IsRemoved == false).FirstOrDefault();
                if (dbList == null)
                    return false;
                dbList.IsRemoved = true;
                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        #region Subscriber

        public static List<SubscriptionFactory> GetSubscriberDetails(long listId)
        {
            List<SubscriptionFactory> List = new List<SubscriptionFactory>();
            try
            {
                var dc = new ManagementContext();
                var SubscriptionList = dc.SubscriptionSubscriber.Where(x => x.List.ListId == listId && x.IsRemoved == false).ToList();

                foreach (var b in SubscriptionList)
                {
                    List.Add(DisplaySubscriberData(b));
                }
                return List;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return List;
        }
        public static SubscriptionFactory GetSubscriberData(long listId, long subscriberId)
        {

            try
            {
                var dc = new ManagementContext();
                var subscriberInfo = dc.SubscriptionSubscriber.Where(x => x.SubscriberId == subscriberId && x.List.ListId == listId && x.IsRemoved == false).FirstOrDefault();
                if (subscriberInfo != null)
                {
                    return DisplaySubscriberData(subscriberInfo);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        private static SubscriptionFactory DisplaySubscriberData(DataModels.EmailServer.Subscriptions.Subscriber oData)
        {
            SubscriptionFactory bl = new SubscriptionFactory();
            bl.listId = oData.List.ListId;
            bl.BounceCount = oData.BounceCount;
            bl.EmailsSent = oData.EmailsSent;
            bl.IsRemoved = oData.IsRemoved;
            bl.Name = oData.Name;
            bl.Data = oData.Data;
            bl.OptedOut = oData.OptedOut;
            bl.SubscriberId = oData.SubscriberId;
            //  bl.SubscriberTypeEnum = oData.SubscriberTypeEnum;
            // bl.OptedOutDateTime = oData.OptedOutDateTime;

            return bl;
        }

        public static bool Add_New_Subscriber(RDN.Library.Classes.EmailServer.SubscriptionFactory NewData)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.EmailServer.Subscriptions.Subscriber con = new DataModels.EmailServer.Subscriptions.Subscriber();
                con.BounceCount = NewData.BounceCount;
                con.Data = NewData.Data;
                con.EmailsSent = NewData.EmailsSent;
                con.IsRemoved = false;
                con.List = dc.SubscriptionLists.Where(x => x.ListId == NewData.listId).FirstOrDefault();

                con.Name = NewData.Name;
                con.OptedOut = NewData.OptedOut;
                //  con.OptedOutDateTime = NewData.OptedOutDateTime;
                // con.SubscriberTypeEnum = NewData.SubscriberTypeEnum;

                dc.SubscriptionSubscriber.Add(con);

                DataModels.EmailServer.Subscriptions.SubscriptionOwner subsOwner = new DataModels.EmailServer.Subscriptions.SubscriptionOwner();
                subsOwner.IsRemoved = false;
                subsOwner.List = dc.SubscriptionLists.Where(x => x.ListId == NewData.listId).FirstOrDefault();
                subsOwner.LeagueOwner = dc.Leagues.Where(x => x.LeagueId == NewData.LeagueOwner).FirstOrDefault();

                dc.SubscriptionOwners.Add(subsOwner);

                int c = dc.SaveChanges();

                return c > 0;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return false;
        }


        public static bool UpdateSubscriberInfo(RDN.Library.Classes.EmailServer.SubscriptionFactory SubscriberToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbSponsor = dc.SubscriptionSubscriber.Where(x => x.SubscriberId == SubscriberToUpdate.SubscriberId).FirstOrDefault();
                if (dbSponsor == null)
                    return false;

                dbSponsor.Name = SubscriberToUpdate.Name;
                //  dbSponsor.SubscriberTypeEnum = SubscriberToUpdate.SubscriberTypeEnum;
                dbSponsor.Data = SubscriberToUpdate.Data;
                dbSponsor.OptedOut = SubscriberToUpdate.OptedOut;
                //  dbSponsor.OptedOutDateTime = SubscriberToUpdate.OptedOutDateTime;

                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }


        public static bool SubscriberRemove(long subscriberId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbSponsor = dc.SubscriptionSubscriber.Where(x => x.SubscriberId == subscriberId).FirstOrDefault();
                if (dbSponsor == null)
                    return false;

                dbSponsor.IsRemoved = true;

                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        #endregion Subscriber

        #region Email Blast

        public static List<SubscriptionType> GetSubscription()
        {
            var dc = new ManagementContext();
            return (from xx in dc.SubscriptionLists
                    where xx.SubscriberTypeEnum == 0
                    select new SubscriptionType { SubscriptionId = xx.ListId, SubscriptionName = xx.ListName }).AsParallel().ToList();
        }




        #endregion 


        public static bool DoUpgrade()
        {
            var dc = new ManagementContext();
            var list = dc.SubscribersList.ToList();

            foreach (var item in list)
            {
                SubscriberManager.AddSubscriber(SubscriberType.EmailScraped, item.EmailToAddToList);
            }

            var ubSubscribeList = dc.NonSubscribersList.ToList();

            foreach (var item in ubSubscribeList)
            {
                SubscriberManager.AddUnSubscriber(item.EmailToRemoveFromList);
            }

            var sd = dc.ScoreboardDownloads.ToList();

            foreach (var item in sd)
            {
                SubscriberManager.AddSubscriber(SubscriberType.ScoreboardDownloads, item.Email);
            }

            var sf = dc.ScoreboardFeedback.ToList();

            foreach (var item in sf)
            {
                SubscriberManager.AddSubscriber(SubscriberType.ScoreboardFeedback, item.Email);
            }


            var rr = dc.RefRoster.ToList();

            foreach (var item in rr)
            {
                SubscriberManager.AddSubscriber(SubscriberType.RefRoster, item.FacebookLink);
            }


            var la = dc.ContactLeagueAddresses.ToList();

            foreach (var item in la)
            {
                SubscriberManager.AddSubscriber(SubscriberType.LeagueAddresses, item.EmailAddress);
            }

            var ee= dc.EmailsForAllEntities.ToList();

            foreach (var item in ee)
            {
                SubscriberManager.AddSubscriber(SubscriberType.EmailScraped, item.EmailAddress);
            }

            return true;
        }
    }
}
