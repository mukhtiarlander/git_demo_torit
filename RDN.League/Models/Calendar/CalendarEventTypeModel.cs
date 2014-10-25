using RDN.Library.Classes.Calendar;
using RDN.Portable.Classes.Controls.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Models.Calendar
{
    public class CalendarEventTypeModel : CalendarEventType
    {
        public SelectList ColorList { get; set; }
        public Guid LeagueId { get; set; }

        public CalendarEventTypeModel()
        {}

        public CalendarEventTypeModel(CalendarEventType t)
        {
            this.CalendarEventTypeId = t.CalendarEventTypeId;
            this.ColorName = t.ColorName;
            this.ColorTempSelected = t.ColorTempSelected;
            this.EventType = t.EventType;
            this.EventTypeName = t.EventTypeName;
            this.PointsForExcused = t.PointsForExcused;
            this.PointsForNotPresent = t.PointsForNotPresent;
            this.PointsForPartial = t.PointsForPartial;
            this.PointsForPresent = t.PointsForPresent;
            this.PointsForTardy = t.PointsForTardy;
        }
    }
}