using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.ContactCard;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.League;
using RDN.Portable.Classes.Contacts;
using RDN.Portable.Classes.ContactCard.Enums;
using RDN.Portable.Classes.Contacts.Enums;

namespace RDN.Library.Classes.Contacts
{
    public class Contact
    {
        public static RDN.Portable.Classes.League.Classes.League GetLeagueContacts(Guid leagueId)
        {
            try
            {
                var dc = new ManagementContext();
                var league = dc.Leagues.Include("Contacts").Where(x => x.LeagueId == leagueId).FirstOrDefault();
                return League.LeagueFactory.DisplayLeague(league);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }

        public static bool EditLeagueContact(Guid leagueId, ContactDisplayBasic contact)
        {
            try
            {
                var dc = new ManagementContext();
                var con = dc.LeagueContacts.Where(x => x.Contact.ContactId == contact.ContactId).FirstOrDefault();

                con.Contact.CompanyName = contact.CompanyName;
                con.Contact.Firstname = contact.FirstName;
                con.Contact.Lastname = contact.LastName;
                con.Contact.Link = contact.Link;
                con.Contact.IsViewableToLeagueMember = contact.IsViewableToLeagueMember;

                int countryId = Convert.ToInt32(contact.CountryId);
                ContactCard.ContactCardFactory.UpdateAddressToContact(contact.Address1, contact.Address2, contact.CityRaw, contact.StateRaw, contact.Zip, AddressTypeEnum.None, con.Contact.ContactCard, dc.Countries.Where(x => x.CountryId == countryId).FirstOrDefault());
                ContactCard.ContactCardFactory.UpdatePhoneNumberToCard(contact.PhoneNumber, con.Contact.ContactCard, String.Empty);
                ContactCard.ContactCardFactory.UpdateEmailToContactCard(contact.Email, con.Contact.ContactCard);
                
                con.Notes = contact.Notes;
                con.ContactTypeEnum = (byte)((ContactTypeForOrganizationEnum)Enum.Parse(typeof(ContactTypeForOrganizationEnum), contact.ContactTypeSelected));
                con.League = con.League;
                con.Contact = con.Contact;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool AddLeagueContact(Guid leagieId, ContactDisplayBasic contact)
        {
            try
            {
                var dc = new ManagementContext();
                DataModels.Contacts.Contact con = new DataModels.Contacts.Contact();
                con.CompanyName = contact.CompanyName;
                con.Firstname = contact.FirstName;
                con.Lastname = contact.LastName;
                con.Link = contact.Link;
                con.IsViewableToLeagueMember = contact.IsViewableToLeagueMember;
                con.ContactCard = new DataModels.ContactCard.ContactCard();
                if (!String.IsNullOrEmpty(contact.Address1) || !String.IsNullOrEmpty(contact.Address2) || !String.IsNullOrEmpty(contact.CityRaw) || !String.IsNullOrEmpty(contact.StateRaw))
                {
                    int countryId = Convert.ToInt32(contact.CountryId);
                    ContactCard.ContactCardFactory.AddAddressToContact(contact.Address1, contact.Address2, contact.CityRaw, contact.StateRaw, contact.Zip, AddressTypeEnum.None, con.ContactCard, dc.Countries.Where(x => x.CountryId == countryId).FirstOrDefault());
                }
                if (!String.IsNullOrEmpty(contact.PhoneNumber))
                {
                    ContactCard.ContactCardFactory.AddPhoneNumberToCard(contact.PhoneNumber, con.ContactCard);
                }
                if (!String.IsNullOrEmpty(contact.Email))
                {
                    ContactCard.ContactCardFactory.AddEmailToContactCard(contact.Email, con.ContactCard);
                }
                dc.Contacts.Add(con);

                var league = dc.Leagues.Where(x => x.LeagueId == leagieId).FirstOrDefault();
                LeagueContacts lc = new LeagueContacts();
                lc.Notes = contact.Notes;
                lc.ContactTypeEnum = (byte)contact.ContactTypeForOrg;
                lc.Contact = con;
                lc.League = league;
                league.Contacts.Add(lc);

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
