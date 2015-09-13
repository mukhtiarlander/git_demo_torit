using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geocoding;
using Geocoding.Google;
using RDN.Library.Classes.Error;

namespace RDN.Library.Classes.Location
{
    public class GeocodingManager
    {
        public static GeoCoordinate FindLatLongOfAddress(string street1, string street2, string zip, string city, string state, string country)
        {
            GeoCoordinate geo = new GeoCoordinate();
            try
            {
                IGeocoder geocoder = new GoogleGeocoder();
                var street = street1;
                if (!string.IsNullOrEmpty(street))
                {
                    if (!string.IsNullOrEmpty(street2))
                    {
                        street = street + " , " + street2;
                    }
                }
                else
                {
                    street = street2;
                }
                if (!String.IsNullOrEmpty(state) && !String.IsNullOrEmpty(zip))
                {
                    IEnumerable<Address> addresses = geocoder.Geocode(street, city, state, zip, country);
                    if (addresses != null)
                    {
                        var enumerable = addresses as IList<Address> ?? addresses.ToList();
                        if (enumerable.FirstOrDefault() != null && enumerable.FirstOrDefault().Coordinates != null)
                        {
                            geo.Latitude = enumerable.FirstOrDefault().Coordinates.Latitude;
                            geo.Longitude = enumerable.FirstOrDefault().Coordinates.Longitude;
                        }
                        return geo;
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: street1 + " " + street2 + " " + city + " " + country + " " + state + " " + zip);
            }
            return null;
        }
    }
}
