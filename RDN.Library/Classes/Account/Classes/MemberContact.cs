using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Communications.Enums;
using RDN.Library.Classes.ContactCard;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Portable.Classes.Account.Enums;
using RDN.Portable.Classes.ContactCard.Enums;

namespace RDN.Library.Classes.Account.Classes
{
    public class MemberContactFactory
    {
        

        public static bool RemoveContact(Guid memberId, long contactId)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                var con = mem.MemberContacts.Where(x => x.ContactId == contactId).FirstOrDefault();
                if (con != null)
                {
                    dc.MemberContacts.Remove(con);
                    int c = dc.SaveChanges();
                    return c > 0;
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static bool AddContactToMember(Guid memberId, string first, string last, MemberContactTypeEnum type, string email, string phone, string address1, string address2, string city, string state, string zip, string country)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                DataModels.Member.MemberContact contact = new DataModels.Member.MemberContact();
                contact.ContactTypeEnum = Convert.ToInt32(type);
                contact.Firstname = first;
                contact.Lastname = last;
                contact.ContactCard = new DataModels.ContactCard.ContactCard();

                if (!String.IsNullOrEmpty(email))
                {
                    ContactCard.ContactCardFactory.AddEmailToContactCard(email, contact.ContactCard);
                }
                if (!String.IsNullOrEmpty(address1))
                {
                    if (String.IsNullOrEmpty(country))
                        country = "0";
                    int co = Convert.ToInt32(country);
                    var count = dc.Countries.Where(x => x.CountryId == co).FirstOrDefault();
                    ContactCard.ContactCardFactory.AddAddressToContact(address1, address2, city, state, zip, AddressTypeEnum.None, contact.ContactCard, count);
                }

                if (!String.IsNullOrEmpty(phone))
                {
                    ContactCard.ContactCardFactory.AddPhoneNumberToCard(phone, contact.ContactCard);
                }
                var mem = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                mem.MemberContacts.Add(contact);
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
