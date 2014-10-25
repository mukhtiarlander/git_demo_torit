using Google.GData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Controls.Calendar
{
    public class CalendarSyncGoogle
    {
        private string publicCalendarUrl { get; set; }
        public CalendarSyncGoogle SetCalendarUrl(string calUrl)
        {
            publicCalendarUrl = calUrl;
            return this;
        }

        public AtomFeed GetGoogleCalendar()
        {
            Service service = new Service();
            FeedQuery query = new FeedQuery();
            query.Uri = new Uri(publicCalendarUrl);
            return service.Query(query);

        }
    }
}
