using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.ContactCard;
using RDN.Library.DataModels.Location;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Error;
using RDN.Portable.Classes.Geo;

namespace RDN.Library.Classes.Location
{
    public class LocationFactory
    {
        public static void UpdateLocations()
        {
            var dc = new ManagementContext();
            var adds = dc.Addresses;
            foreach (var add in adds)
            {
                try
                {
                    var coords = OpenStreetMap.FindLatLongOfAddress(add.Address1, add.Address2, add.Zip, add.CityRaw, add.StateRaw, add.Country != null ? add.Country.Name : String.Empty);
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
                    add.ContactCard = add.ContactCard;


                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }
            }
            dc.SaveChanges();

        }
        public static bool RemoveLocation(Guid locationId)
        {
            try
            {
                var dc = new ManagementContext();
                var loc = (from xx in dc.Locations
                           where xx.LocationId == locationId
                           select xx).FirstOrDefault();

                if (loc != null)
                    loc.IsRemoved = true;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;

        }

        public static RDN.Portable.Classes.Location.Location GetLocation(Guid locationId)
        {
            RDN.Portable.Classes.Location.Location l = new RDN.Portable.Classes.Location.Location();
            try
            {
                var dc = new ManagementContext();
                var loc = (from xx in dc.Locations
                           where xx.LocationId == locationId
                           select xx).FirstOrDefault();

                if (loc == null)
                    return l;
                l = DisplayLocation(loc);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return l;

        }

        public static RDN.Portable.Classes.Location.Location DisplayLocation(DataModels.Location.Location loc)
        {
            RDN.Portable.Classes.Location.Location l = new RDN.Portable.Classes.Location.Location();
            try
            {
                if (loc != null)
                {
                    l.LocationName = loc.LocationName;
                    l.LocationId = loc.LocationId;
                    if (loc.Contact != null && loc.Contact.Addresses.FirstOrDefault() != null)
                    {
                        var add = loc.Contact.Addresses.FirstOrDefault();
                        RDN.Portable.Classes.ContactCard.Address a = new RDN.Portable.Classes.ContactCard.Address();
                        a.Address1 = add.Address1;
                        a.Address2 = add.Address2;
                        a.AddressId = add.AddressId;
                        a.CityRaw = add.CityRaw;
                        if (add.Country != null)
                            a.Country = add.Country.Code;
                        a.IsDefault = add.IsDefault;
                        a.StateRaw = add.StateRaw;
                        a.Zip = add.Zip;
                        a.CountryId = add.Country.CountryId;
                        if (loc.Contact.Communications.FirstOrDefault() != null)
                            l.Website = loc.Contact.Communications.First().Data;    
                        l.Contact.Addresses.Add(a);
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return l;
        }

        public static Portable.Classes.Location.Location CreateNewLocation(LocationOwnerType ownerType, string name, string address1, string address2, string city, int country, string state, string zip, string website, Guid idOfOwner)
        {
            Portable.Classes.Location.Location l = new Portable.Classes.Location.Location();
            string log = string.Empty;
            try
            {
                var dc = new ManagementContext();
                DataModels.Location.Location location = new DataModels.Location.Location();
                if (ownerType == LocationOwnerType.calendar)
                {
                    var cal = dc.Calendar.Where(x => x.CalendarId == idOfOwner).FirstOrDefault();
                    cal.Locations.Add(location);
                }
                else if (ownerType == LocationOwnerType.shop)
                {
                    var shop = dc.Merchants.Where(x => x.PrivateManagerId == idOfOwner).FirstOrDefault();
                    shop.Locations.Add(location);
                }
                location.LocationName = name;
                location.Contact = new DataModels.ContactCard.ContactCard();
                Address a = new Address();
                a.Address1 = address1;
                a.Address2 = address2;
                a.CityRaw = city;
                a.Country = dc.Countries.Where(x => x.CountryId == country).FirstOrDefault();
                a.StateRaw = state;
                a.ContactCard = location.Contact;
                var coords = OpenStreetMap.FindLatLongOfAddress(a.Address1, a.Address2, a.Zip, a.CityRaw, a.StateRaw, a.Country != null ? a.Country.Name : string.Empty);
                a.Coords = new System.Device.Location.GeoCoordinate();
                if (coords != null)
                {
                    log += "not" + coords.Latitude + " " + coords.Longitude;
                    a.Coords.Latitude = coords.Latitude;
                    a.Coords.Longitude = coords.Longitude;
                }
                else
                {
                    a.Coords.Latitude = 0.0;
                    a.Coords.Longitude = 0.0;
                }
                a.Coords.Altitude = 0.0;
                a.Coords.Course = 0.0;
                a.Coords.HorizontalAccuracy = 1.0;
                a.Coords.Speed = 0.0;
                a.Coords.VerticalAccuracy = 1.0;

                location.Contact.Addresses.Add(a);
                if (!String.IsNullOrEmpty(website))
                {
                    string comType = CommunicationTypeEnum.Website.ToString();
                    Communication web = new Communication();
                    web.Data = website;
                    web.CommunicationTypeEnum = (byte)CommunicationTypeEnum.Website;
                    location.Contact.Communications.Add(web);
                }
                dc.Locations.Add(location);
                int c = dc.SaveChanges();

                l.LocationId = location.LocationId;
                l.LocationName = name;
                Portable.Classes.ContactCard.Address address = new Portable.Classes.ContactCard.Address();
                address.Address1 = address1;
                address.Address2 = address2;
                address.CityRaw = city;
                if (a.Country != null)
                    address.Country = a.Country.Name;
                address.StateRaw = a.StateRaw;
                address.Zip = a.Zip;
                l.Contact.Addresses.Add(address);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: name + " " + address1 + " " + address2 + " " + city + " " + country + " " + state + " " + zip + " " + website + " " + idOfOwner);
            }
            return l;

        }

        public static int UpdateLocation(Guid locationId, string name, string address1, string address2, string city, int country, string state, string zip, string website, Guid idOfOwner)
        {
              string log = string.Empty;
            int c = 0;
              try
            {
                var dc = new ManagementContext();
                DataModels.Location.Location location = (from xx in dc.Locations
                           where xx.LocationId == locationId
                           select xx).FirstOrDefault();

                if (location != null)
                {
                    location.LocationName = name;
                    Address a = location.Contact.Addresses.First();
                   a.Address1 = address1;
                   a.Address2 = address2;
                   a.CityRaw = city;
                   a.Zip = zip;
                   a.Country = dc.Countries.Where(x => x.CountryId == country).FirstOrDefault();
                   a.StateRaw = state;
                   a.ContactCard = location.Contact;
                    var coords = OpenStreetMap.FindLatLongOfAddress(address1, address2, zip, city, state, a.Country != null ? a.Country.Name : string.Empty);
                   a.Coords = new System.Device.Location.GeoCoordinate();
                    if (coords != null)
                    {
                        log += "not" + coords.Latitude + " " + coords.Longitude;
                       a.Coords.Latitude = coords.Latitude;
                       a.Coords.Longitude = coords.Longitude;
                    }
                    else
                    {
                       a.Coords.Latitude = 0.0;
                       a.Coords.Longitude = 0.0;
                    }
                   a.Coords.Altitude = 0.0;
                   a.Coords.Course = 0.0;
                   a.Coords.HorizontalAccuracy = 1.0;
                   a.Coords.Speed = 0.0;
                   a.Coords.VerticalAccuracy = 1.0;

                    if (!String.IsNullOrEmpty(website))
                    {
                        Communication comm = location.Contact.Communications.FirstOrDefault();
                        if (comm == null)
                        {
                            comm = new Communication();
                            comm.Data = website;
                            comm.CommunicationTypeEnum = (byte)CommunicationTypeEnum.Website;
                            location.Contact.Communications.Add(comm);
                        }
                        else
                        {
                            comm.Data = website;
                            comm.CommunicationTypeEnum = (byte)CommunicationTypeEnum.Website;
                        }
                    }
                    c = dc.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: name + " " + address1 + " " + address2 + " " + city + " " + country + " " + state + " " + zip + " " + website + " " + idOfOwner);
            }
            return c;
        }

        public static Dictionary<int, string> GetCountriesDictionary()
        {
            var dc = new ManagementContext();
            return dc.Countries.ToDictionary(key => key.CountryId, value => value.Name);
        }
        public static List<CountryClass> GetCountries()
        {
            var dc = new ManagementContext();
            return (from xx in dc.Countries
                    select new CountryClass { CountryId = xx.CountryId, Name = xx.Name }).AsParallel().ToList();
        }

        public static List<DataModels.Location.Country> GetCountriesList()
        {
            var dc = new ManagementContext();
            return dc.Countries.ToList();
        }
    }
}
