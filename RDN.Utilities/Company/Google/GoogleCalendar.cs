using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Company.Google
{
    public class GoogleCalendar
    {

        private static string GoogleUrl = "http://www.google.com/calendar/event?action=TEMPLATE";
        public string What { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string WebsiteName { get; set; }
        public string Website { get; set; }

        public string GetGoogleUrl()
        {
            //20130918T103000Z
            return GoogleUrl + "&text=" + What + "&dates=" + StartDate.ToString("yyyyMMddTHHmmssZ") + "/" + EndDate.ToString("yyyyMMddTHHmmssZ") + "&details=" + Description + "&location=" + Location + "&trp=false" + "&sprop=" + WebsiteName + "&sprop=name:" + Website;
        }
    }
}
