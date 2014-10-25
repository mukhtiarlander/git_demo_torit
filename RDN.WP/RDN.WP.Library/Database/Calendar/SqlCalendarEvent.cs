using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;
using RDN.Portable.Models.Json.Calendar;

namespace RDN.WP.Library.Database.Calendar
{
   public  class SqlCalendarEvent:EventJson
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string LeagueId { get; set; }
    }
}
