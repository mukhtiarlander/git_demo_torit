using System;
using System.Linq;
using System.Collections.Generic;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.ContactCard;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Communications.Enums;
using RDN.Library.Classes.Location;
using RDN.Portable.Classes.ContactCard.Enums;
using RDN.Portable.Classes.Communications.Enums;

namespace RDN.Library.Classes.ContactCard
{
    public class ContactCardFactory
    {
       
        /// <summary>
        /// creates a new CommunicationType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int CreateCommunicationType(CommunicationTypeEnum type)
        {
            try
            {
                var dc = new ManagementContext();
                CommunicationType com = new CommunicationType();
                com.Type = type.ToString();
                dc.CommunicationType.Add(com);
                dc.SaveChanges();
                return com.CommunicationTypeId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }

        public static bool AddEmailToContactCard(string email, DataModels.ContactCard.ContactCard contact)
        {
            try
            {
                contact.Emails.Add(new RDN.Library.DataModels.ContactCard.Email { EmailAddress = email, IsDefault = true });
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateEmailToContactCard(string email, DataModels.ContactCard.Email emailCard)
        {
            try
            {
                emailCard.EmailAddress = email;
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateEmailToContactCard(string email, DataModels.ContactCard.ContactCard card)
        {
            try
            {
                if (card.Emails.FirstOrDefault() == null)
                {
                    DataModels.ContactCard.Email e = new DataModels.ContactCard.Email();
                    UpdateEmailToContactCard(email, e);
                    card.Emails.Add(e);
                }
                else
                    UpdateEmailToContactCard(email, card.Emails.FirstOrDefault());
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool AddAddressToContact(string address1, string address2, string city, string state, string zipCode, AddressTypeEnum addressType, DataModels.ContactCard.ContactCard contact, DataModels.Location.Country country)
        {

            try
            {
                var add = new DataModels.ContactCard.Address
                {
                    Address1 = address1,
                    Address2 = address2,
                    StateRaw = state,
                    CityRaw = city,
                    Zip = zipCode,
                    Country = country,
                    AddressType = (byte)addressType
                };

                var coords = OpenStreetMap.FindLatLongOfAddress(add.Address1, add.Address2, add.Zip, add.CityRaw, add.StateRaw, add.Country != null ? add.Country.Name : string.Empty);
                add.Coords = new System.Device.Location.GeoCoordinate();

                if (coords != null)
                {
                    add.Coords.Latitude = coords.Latitude;
                    add.Coords.Longitude = coords.Longitude;
                }
                else
                {
                    add.Coords.Latitude = 0;
                    add.Coords.Longitude = 0;
                }
                add.Coords.Altitude = 0;
                add.Coords.Course = 0;
                add.Coords.HorizontalAccuracy = 1;
                add.Coords.Speed = 0;
                add.Coords.VerticalAccuracy = 1;

                contact.Addresses.Add(add);
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: address1 + " " + address2 + " " + city + " " + state + " " + zipCode + " " + country);
            }
            return false;
        }
        public static bool UpdateAddressToContact(string address1, string address2, string city, string state, string zipCode, AddressTypeEnum addressType, DataModels.ContactCard.Address add, DataModels.Location.Country country)
        {
            try
            {

                add.Address1 = address1;
                add.Address2 = address2;
                add.StateRaw = state;
                add.CityRaw = city;
                add.Zip = zipCode;
                add.Country = country;
                add.AddressType = (byte)addressType;
                var coords = OpenStreetMap.FindLatLongOfAddress(add.Address1, add.Address2, add.Zip, add.CityRaw, add.StateRaw, add.Country != null ? add.Country.Name : string.Empty);
                if (coords != null)
                {
                    add.Coords = new System.Device.Location.GeoCoordinate();
                    add.Coords.Latitude = coords.Latitude;
                    add.Coords.Longitude = coords.Longitude;
                    add.Coords.Altitude = 0;
                    add.Coords.Course = 0;
                    add.Coords.HorizontalAccuracy = 1;
                    add.Coords.Speed = 0;
                    add.Coords.VerticalAccuracy = 1;
                }
                else
                {
                    add.Coords = new System.Device.Location.GeoCoordinate();
                    add.Coords.Latitude = 0.0;
                    add.Coords.Longitude = 0.0;
                    add.Coords.Altitude = 0;
                    add.Coords.Course = 0;
                    add.Coords.HorizontalAccuracy = 1;
                    add.Coords.Speed = 0;
                    add.Coords.VerticalAccuracy = 1;
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateAddressToContact(string address1, string address2, string city, string state, string zipCode, AddressTypeEnum addressType, DataModels.ContactCard.ContactCard card, DataModels.Location.Country country)
        {
            try
            {
                if (card.Addresses.FirstOrDefault() != null)
                    UpdateAddressToContact(address1, address2, city, state, zipCode, addressType, card.Addresses.FirstOrDefault(), country);
                else
                {
                    DataModels.ContactCard.Address add = new DataModels.ContactCard.Address();
                    UpdateAddressToContact(address1, address2, city, state, zipCode, addressType, add, country);
                    card.Addresses.Add(add);
                } return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool AddPhoneNumberToCard(string phone, DataModels.ContactCard.ContactCard contact)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                int phoneType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                contact.Communications.Add(new DataModels.ContactCard.Communication
                {
                    Data = phone,
                    IsDefault = true,
                    //CommunicationType = dc.CommunicationType.Where(x => x.CommunicationTypeId == phoneType).FirstOrDefault()
                    CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber
                });
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdatePhoneNumberToCard(string phone, DataModels.ContactCard.Communication phoneNumber, string verificationCode, MobileServiceProvider carrier = MobileServiceProvider.None, bool isVerified = false)
        {
            try
            {
                ManagementContext dc = new ManagementContext();
                phoneNumber.Data = phone;
                if (!String.IsNullOrEmpty(verificationCode))
                    phoneNumber.SMSVerificationCode = verificationCode;
                if (carrier != MobileServiceProvider.None)
                    phoneNumber.CarrierType = (byte)carrier;
                if (isVerified)
                    phoneNumber.IsCarrierVerified = isVerified;
                phoneNumber.CommunicationTypeEnum = (byte)CommunicationTypeEnum.PhoneNumber;
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdatePhoneNumberToCard(string phone, DataModels.ContactCard.ContactCard card, string verificationCode, MobileServiceProvider carrier = MobileServiceProvider.None, bool isVerified = false)
        {
            try
            {
                int phoneType = Convert.ToInt32(CommunicationTypeEnum.PhoneNumber);
                if (card.Communications.Where(x => x.IsDefault == true).Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault() == null)
                {
                    Communication c = new Communication();
                    c.IsDefault = true;
                    UpdatePhoneNumberToCard(phone, c, verificationCode, carrier, isVerified);
                    card.Communications.Add(c);
                }
                else
                {
                    UpdatePhoneNumberToCard(phone, card.Communications.Where(x => x.IsDefault == true).Where(x => x.CommunicationTypeEnum == (byte)CommunicationTypeEnum.PhoneNumber).FirstOrDefault(), verificationCode, carrier, isVerified);
                }
                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
    }
}
