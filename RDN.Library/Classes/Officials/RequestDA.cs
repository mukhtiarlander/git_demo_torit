using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.ContactCard.Enums;
using RDN.Portable.Classes.Games.Officiating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Officials
{
    public class RequestFactory
    {



        public static long Add_New_Request(RequestDA NewRequest)
        {

            try
            {
                var dc = new ManagementContext();
                DataModels.Officials.RefereeRequest con = new DataModels.Officials.RefereeRequest();
                con.Date = NewRequest.Date;
                con.Description = NewRequest.Description;
                con.EvaluationsProvided = NewRequest.EvaluationsProvided;
                con.FirstWhistle = NewRequest.FirstWhistle;
                con.IsRegulation = NewRequest.IsRegulation;
                con.IsReimbursement = NewRequest.IsReimbursement;
                con.IsSnacksProvided = NewRequest.IsSnacksProvided;
                con.NonsoNeded = NewRequest.NonsoNeded;
                con.NoRefNeded = NewRequest.NoRefNeded;
                con.RuleSetId = NewRequest.RuleSetId;
                con.TeamsPlaying = NewRequest.TeamsPlaying;
                con.TravelStipendForNSO = NewRequest.TravelStipendForNSO;
                con.TravelStipendForRefs = NewRequest.TravelStipendForRefs;
                con.Location = new DataModels.Location.Location();
                con.Location.LocationName = NewRequest.LocationName;
                con.Location.Contact = new DataModels.ContactCard.ContactCard();
                ContactCard.ContactCardFactory.AddAddressToContact(NewRequest.Address, NewRequest.Address1, NewRequest.City, NewRequest.State, NewRequest.Zip, AddressTypeEnum.Bout, con.Location.Contact, dc.Countries.Where(x => x.Code == NewRequest.Country || x.Name == NewRequest.Country).FirstOrDefault());
                con.RequestCreator = dc.Members.Where(x => x.MemberId == NewRequest.RequestCreator).FirstOrDefault();
                dc.Requests.Add(con);

                int c = dc.SaveChanges();


                //return c > 0;
                return con.RequestId;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());

            }
            return 0;
        }

        /// <summary>
        /// This function used for Edit and View Details
        /// </summary>
        /// <param name="RequestId"></param>
        /// <param name="RequestCreator"></param>
        /// <returns>Request details</returns>
        public static RequestDA GetData(long requestId, Guid requestCreator = new Guid())//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var requestInfo = dc.Requests.Where(x => x.RequestId == requestId && x.RequestCreator.MemberId == requestCreator).FirstOrDefault();

                if ((requestCreator == null || requestCreator == Guid.Empty) && (requestId != null)) // used for RDNation.com
                {
                    requestInfo = dc.Requests.Where(x => x.RequestId == requestId).FirstOrDefault();
                }   
            if (requestInfo != null)
                {
                    return DisplayRequestList(requestInfo);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static int GetRequestCount()//This Function used for "Edit" and "View" Form
        {
            try
            {
                var dc = new ManagementContext();
                var yesterday = DateTime.UtcNow.AddDays(-1);
                return dc.Requests.Where(x => x.IsDelete == false && x.Date > yesterday).Count();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }


        private static RequestDA DisplayRequestList(DataModels.Officials.RefereeRequest oRequest)
        {
            RequestDA bl = new RequestDA();
            if (oRequest.Location != null)
            {
                var loc = Location.LocationFactory.DisplayLocation(oRequest.Location);
                bl.LocationName = loc.LocationName;
                var add = loc.Contact.Addresses.FirstOrDefault();
                if (add != null)
                {
                    bl.Address = add.Address1;
                    bl.Address1 = add.Address2;
                    bl.City = add.CityRaw;
                    bl.State = add.StateRaw;
                    bl.Zip = add.Zip;
                }
            }


            bl.Date = oRequest.Date;
            bl.Description = oRequest.Description;
            bl.EvaluationsProvided = oRequest.EvaluationsProvided;
            bl.FirstWhistle = oRequest.FirstWhistle;
            bl.IsRegulation = oRequest.IsRegulation;
            bl.IsReimbursement = oRequest.IsReimbursement;
            bl.IsSnacksProvided = oRequest.IsSnacksProvided;

            bl.NonsoNeded = oRequest.NonsoNeded;
            bl.NoRefNeded = oRequest.NoRefNeded;

            bl.RuleSetId = oRequest.RuleSetId;
            bl.TeamsPlaying = oRequest.TeamsPlaying;
            bl.TravelStipendForNSO = oRequest.TravelStipendForNSO;
            bl.TravelStipendForRefs = oRequest.TravelStipendForRefs;
            bl.RequestId = oRequest.RequestId;

            bl.RequestCreator = oRequest.RequestCreator.MemberId;

            return bl;
        }

        public static List<RequestDA> GetRequestList()//(Guid MemberId)
        {
            List<RequestDA> requestLists = new List<RequestDA>();
            try
            {
                var dc = new ManagementContext();
                var yesterday = DateTime.UtcNow.AddDays(-1);
                var RequestList = dc.Requests.Where(x => x.IsDelete == false && x.Date > yesterday).ToList();

                foreach (var b in RequestList)
                {
                    requestLists.Add(DisplayRequestList(b));
                }
                return requestLists;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return requestLists;
        }

        public static bool UpdateRequest(RequestDA RequestToUpdate)
        {
            try
            {

                var dc = new ManagementContext();
                var dbRequest = dc.Requests.Where(x => x.RequestId == RequestToUpdate.RequestId).FirstOrDefault();
                if (dbRequest == null)
                    return false;
                var country = dc.Countries.Where(x => x.Code == RequestToUpdate.Country).FirstOrDefault();
                var card = dbRequest.Location.Contact;
                dbRequest.Location.LocationName = RequestToUpdate.LocationName;
                ContactCard.ContactCardFactory.UpdateAddressToContact(RequestToUpdate.Address, RequestToUpdate.Address1, RequestToUpdate.City, RequestToUpdate.State, RequestToUpdate.Zip, AddressTypeEnum.Bout, card, country);

                dbRequest.Date = RequestToUpdate.Date;
                dbRequest.Description = RequestToUpdate.Description;
                dbRequest.EvaluationsProvided = RequestToUpdate.EvaluationsProvided;
                dbRequest.FirstWhistle = RequestToUpdate.FirstWhistle;
                dbRequest.IsRegulation = RequestToUpdate.IsRegulation;
                dbRequest.IsReimbursement = RequestToUpdate.IsReimbursement;
                dbRequest.IsSnacksProvided = RequestToUpdate.IsSnacksProvided;

                dbRequest.NonsoNeded = RequestToUpdate.NonsoNeded;
                dbRequest.NoRefNeded = RequestToUpdate.NoRefNeded;
                dbRequest.RequestCreator.MemberId = RequestToUpdate.RequestCreator;
                dbRequest.RuleSetId = RequestToUpdate.RuleSetId;
                dbRequest.TeamsPlaying = RequestToUpdate.TeamsPlaying;
                dbRequest.TravelStipendForNSO = RequestToUpdate.TravelStipendForNSO;
                dbRequest.TravelStipendForRefs = RequestToUpdate.TravelStipendForRefs;



                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool DeleteRequest(long requestId, Guid MemberId)
        {
            try
            {

                var dc = new ManagementContext();
                var dbRequest = dc.Requests.Where(x => x.RequestId == requestId && x.RequestCreator.MemberId == MemberId).FirstOrDefault();
                if (dbRequest == null)
                    return false;
                dbRequest.IsDelete = true;
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
