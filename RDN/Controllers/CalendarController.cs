using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Calendar;
using RDN.Library.Classes.Calendar.Models;
using RDN.Library.Classes.Error;
using RDN.Library.Util.Enum;
using RDN.Models.OutModel;
using RDN.Portable.Classes.Controls.Calendar;
using RDN.Library.Classes.Controls.Calendar;

namespace RDN.Controllers
{
    public class CalendarController : Controller
    {
        public JsonResult CalendarEventsSearching()
        {
            List<CalendarEvent> events = new List<CalendarEvent>();
            if (!String.IsNullOrEmpty(HttpContext.Request.Params["sSearch"]))
            {
                string search = HttpContext.Request.Params["sSearch"];
                events = SiteCache.SearchCalendarEvents(DateTime.UtcNow.AddDays(-10), search, 0, 100);
            }
            else
            {
                events = SiteCache.SearchCalendarEvents(DateTime.UtcNow.AddDays(-1), "", 0, 100);
            }
            return Json(new
            {
                sEcho = HttpContext.Request.Params["sEcho"],
                iTotalRecords = events.Count,
                iTotalDisplayRecords = events.Count,
                aaData = (
                    from n in events
                    select new[]
                    {
                        n.Name,
                        n.NameUrl,
                        n.StartDateDisplay,
                        n.EndDateDisplay,
                        n.Address,
                        n.AddressUrl,
                        n.OrganizerUrl,
                        n.OrganizersName,
                        "true"
                    }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CalendarEvents(int? year, int? month)
        {
            try
            {
                //we shouldn't be looking at any dates less than 2012. or greater than 2100
                if (year != null && year.Value < 2012)
                    year = null;
                else if (year != null && year.Value > 2100)
                    year = null;
                EventsOutModel mod = new EventsOutModel();
                int day = 1;
                if (DateTime.UtcNow.Day > 28)
                    day = 28;
                else if (DateTime.UtcNow.Day == 1)
                    day = 1;
                else
                    day = DateTime.UtcNow.AddDays(-1).Day;
                if (year != null && month != null)
                    mod.StartDate = new DateTime(year.Value, month.Value, day);
                else
                    mod.StartDate = DateTime.UtcNow.AddDays(-1);
                mod.EndDate = mod.StartDate.AddDays(30);

                mod.Events = SiteCache.GetCalendarEvents(mod.StartDate, mod.EndDate);

                return View(mod);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        public ActionResult EventCalendar(string name, string id)
        {
            try
            {
                var calEvent = SiteCache.GetCalendarEvent(new Guid(id));
                if (calEvent != null)
                    return View(calEvent);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }
        [HttpPost]
        public ActionResult EventCalendarExport(CalendarEventPortable cal)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                return new FileStreamResult(CalendarEventFactory.ExportEvent(cal.CalendarItemId, memId), "text/calendar") { FileDownloadName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(cal.Name) + ".ics" };

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

    }
}
