using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Text;
using Geocoding;
using Geocoding.Google;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RDN.Library.Classes.Error;

namespace RDN.Library.Classes.Location
{
    public class OpenStreetMap
    {
        public static GeoCoordinate FindLatLongOfAddress2(string street1, string street2, string zip, string city, string state, string country)
        {
            GeoCoordinate geo = new GeoCoordinate();
            try
            {
                if (!string.IsNullOrEmpty(street1) || !string.IsNullOrEmpty(street2) || !string.IsNullOrEmpty(zip) || !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(state) || !string.IsNullOrEmpty(country))
                {
                    var httpClient = new WebClient();
                    string url = "http://nominatim.openstreetmap.org/search?format=json&polygon=1&addressdetails=1&q=" + street1 + " " + street2 + ", " + city + "," + state + " " + zip + " " + country;
                    string dl = httpClient.DownloadString(url);
                    if (dl.Length > 10)
                    {
                        var r = (JArray)JsonConvert.DeserializeObject(dl);
                        var latString = ((JValue)r[0]["lat"]).Value as string;
                        var longString = ((JValue)r[0]["lon"]).Value as string;
                        geo.Latitude = Convert.ToDouble(latString);
                        geo.Longitude = Convert.ToDouble(longString);
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
