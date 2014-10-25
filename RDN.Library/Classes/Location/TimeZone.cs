using RDN.Library.DataModels.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Location
{
    public class TimeZoneFactory
    {


        public static List<RDN.Portable.Classes.Location.TimeZone> GetTimeZones()
        {
            var dc = new ManagementContext();
            var zones = (from xx in dc.TimeZone
                         select new RDN.Portable.Classes.Location.TimeZone
                         {
                             ZoneId = xx.ZoneId,
                             GMT = xx.GMT,
                             GMTOffset = xx.GMTOffset,
                             Location = xx.Location,
                             LocationGMT = xx.GMT + " " + xx.Location
                         }).ToList();
            return zones;
        }
    }
}
