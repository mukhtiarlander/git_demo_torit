using RDN.Library.Classes.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Raspberry.Controllers
{
    public class CalendarController : Controller
    {
        [Authorize]
        public ActionResult ViewDeletedEvents(string id)
        {
            var ev = CalendarEventFactory.UnDeleteAttendanceEvents(new Guid(id));


            return View(ev);
        }
    }
}