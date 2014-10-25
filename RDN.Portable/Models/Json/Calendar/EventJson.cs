using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace RDN.Portable.Models.Json.Calendar
{
    [DataContract]
    public class EventJson
    {
        [DataMember]
        public string LeagueId { get; set; }
        [DataMember]
        public string CalendarItemId { get; set; }
        [DataMember]
        public string OrganizersName { get; set; }
        [DataMember]
        public string OrganizersId { get; set; }
        [DataMember]
        public string OrganizerUrl { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string NameUrl { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string LogoUrl { get; set; }
        [DataMember]
        public string Location { get; set; }

        public string DateTimeLocation
        {
            get
            {
                return StartDate.ToLocalTime().ToString("d/M") + ", " + StartDate.ToLocalTime().ToString("h:mm tt") + " @ " + Location;
            }
        }
        public string DateTimeHuman
        {
            get
            {
                return StartDate.ToLocalTime().ToString("d/M/yyyy") + " " + StartDate.ToLocalTime().ToString("h:mm tt") + " - " + EndDate.ToLocalTime().ToString("h:mm tt");
            }
        }
        public string AddressHuman
        {
            get
            {
                return "@ " + Address;
            }
        }
        [DataMember]
        public string Description { get; set; }
        const string HTML_TAG_PATTERN = "<.*?>";
        public string DescriptionNonHtml
        {
            get
            {
                if (!String.IsNullOrEmpty(Description))
                    return Regex.Replace(Description, HTML_TAG_PATTERN, string.Empty);

                return string.Empty;
            }
        }
        [DataMember]
        public string TicketUrl { get; set; }
        [DataMember]
        public string EventUrl { get; set; }
        [DataMember]
        public string RDNUrl { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Miles { get; set; }
    }
}
