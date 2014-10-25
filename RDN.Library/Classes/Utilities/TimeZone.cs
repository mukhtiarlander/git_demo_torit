using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Utilities
{
    /// <summary>
    /// time zones of the world
    /// </summary>
    public class TimeZone
    {
        public TimeZone(double zone, string zoneName)
        {
            this.Zone = zone;
            this.TimeZoneLocation = zoneName;
        }
        public double Zone { get; set; }
        public string TimeZoneLocation { get; set; }


        public static List<TimeZone> GetAllCommonTimeZones()
        {
            List<TimeZone> zones = new List<TimeZone>();

            zones.Add(new TimeZone(-12, "Eniwitok, Kwajalein"));
            zones.Add(new TimeZone(-11, "Midway Islands, Samoa"));
            zones.Add(new TimeZone(-10, "Hawaii"));
            zones.Add(new TimeZone(-9, "Alaska"));
            zones.Add(new TimeZone(-8, "Pacific Time (U.S. and Canada), Tijuana"));
            zones.Add(new TimeZone(-7, "Mountain Time (U.S. and Canada)"));
            zones.Add(new TimeZone(-6, "Central Time (U.S. and Canada), Mexico City, Saskatchewan"));
            zones.Add(new TimeZone(-5, "Eastern Time (U.S. and Canada), Bogota, Lima"));
            zones.Add(new TimeZone(-4, "Atlantic Time (Canada), Caracas, La Paz"));
            zones.Add(new TimeZone(-3, "Brasilia, Buenos Aires, Georgetown"));
            zones.Add(new TimeZone(-2, "Mid-Atlantic"));
            zones.Add(new TimeZone(-1, "Azores, Cape Verde Islands"));
            zones.Add(new TimeZone(0, "Greenwich Mean Time"));
            zones.Add(new TimeZone(1, "Berlin, Stockholm, Rome, Vienna"));
            zones.Add(new TimeZone(2, "Eastern Europe, Pretoria, Israel"));
            zones.Add(new TimeZone(3, "Moscow, St Petersburg"));
            zones.Add(new TimeZone(4, "Abu Dhabi, Muscat"));
            zones.Add(new TimeZone(5, "Islamabad, Karachi"));
            zones.Add(new TimeZone(6, "Almaty, Dhaka"));
            zones.Add(new TimeZone(7, "Bangkok, Jakarta"));
            zones.Add(new TimeZone(8, "Beijing, Hong Kong, Perth, Singapore"));
            zones.Add(new TimeZone(9, "Tokyo, Osaka, Sapporo, Seoul"));
            zones.Add(new TimeZone(10, "Brisbane, Canberra, Melbourne, Sydney"));
            zones.Add(new TimeZone(11, "Solomon Islands, New Caledonia"));
            zones.Add(new TimeZone(12, "Fiji, Kamchatka, Marshall Islands"));

            return zones;
        }

        public static List<TimeZone> GetAllTimeZones()
        {
            List<TimeZone> zones = new List<TimeZone>();

            zones.Add(new TimeZone(-12, "Eniwitok, Kwajalein"));
            zones.Add(new TimeZone(-11, "Midway Islands, Samoa"));
            zones.Add(new TimeZone(-10, "Hawaii"));
            zones.Add(new TimeZone(-9, "Alaska"));
            zones.Add(new TimeZone(-8, "Pacific Time (U.S. and Canada), Tijuana"));
            zones.Add(new TimeZone(-7, "Mountain Time (U.S. and Canada)"));
            zones.Add(new TimeZone(-6, "Central Time (U.S. and Canada), Mexico City, Saskatchewan"));
            zones.Add(new TimeZone(-5, "Eastern Time (U.S. and Canada), Bogota, Lima"));
            zones.Add(new TimeZone(-4, "Atlantic Time (Canada), Caracas, La Paz"));
            zones.Add(new TimeZone(-3.5, "Newfoundland"));
            zones.Add(new TimeZone(-3, "Brasilia, Buenos Aires, Georgetown"));
            zones.Add(new TimeZone(-2, "Mid-Atlantic"));
            zones.Add(new TimeZone(-1, "Azores, Cape Verde Islands"));
            zones.Add(new TimeZone(0, "Greenwich Mean Time"));
            zones.Add(new TimeZone(1, "Berlin, Stockholm, Rome, Vienna"));
            zones.Add(new TimeZone(2, "Eastern Europe, Pretoria, Israel"));
            zones.Add(new TimeZone(3.5, "Tehran"));
            zones.Add(new TimeZone(3, "Moscow, St Petersburg"));
            zones.Add(new TimeZone(4, "Abu Dhabi, Muscat"));
            zones.Add(new TimeZone(4.5, "Kabul"));
            zones.Add(new TimeZone(5, "Islamabad, Karachi"));
            zones.Add(new TimeZone(5.5, "Bombay, Calcutta, Madras, New Delhi, Colombo"));
            zones.Add(new TimeZone(6, "Almaty, Dhaka"));
            zones.Add(new TimeZone(7, "Bangkok, Jakarta"));
            zones.Add(new TimeZone(8, "Beijing, Hong Kong, Perth, Singapore"));
            zones.Add(new TimeZone(9, "Tokyo, Osaka, Sapporo, Seoul"));
            zones.Add(new TimeZone(9.5, "Adelaide, Darwin"));
            zones.Add(new TimeZone(10, "Brisbane, Canberra, Melbourne, Sydney"));
            zones.Add(new TimeZone(11, "Solomon Islands, New Caledonia"));
            zones.Add(new TimeZone(12, "Fiji, Kamchatka, Marshall Islands"));

            return zones;
        }

    }
}
