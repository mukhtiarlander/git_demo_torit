using System;
using System.Web.Mvc;
using RDN.Utilities.Config;
using RDN.Utilities.Error;
using RDN.Library.Classes.Error;

namespace RDN.Controllers
{
    public class ScoreboardController : Controller
    {
        //
        // GET: /Scoreboard/

        public ActionResult Index()
        {
            return View();
        }

        public void Download(string id)
        {
            try
            {
                Library.Classes.Admin.Admin.Admin.ScoreboardDownloadClick(version: ScoreboardConfig.SCOREBOARD_VERSION_NUMBER, email: id, ip: Request.UserHostAddress, httpRaw: Request.ServerVariables["ALL_RAW"]);
                Response.Redirect("http://codingforcharity.org/rdnation/version/setup.exe");
            }
            catch (Exception e)
            {
                try
                {
                    ErrorDatabaseManager.AddException(e, GetType());
                }
                catch { }
                Response.Redirect("http://codingforcharity.org/rdnation/version/setup.exe");
            }
        }
        
        [HttpPost]
        public void Download()
        {
            try{
            string email = null;
            if (Request.Form["Email"] != null && !String.IsNullOrEmpty(Request["Email"]))
                email = Request.Form["Email"];
            Library.Classes.Admin.Admin.Admin.ScoreboardDownloadClick(version: ScoreboardConfig.SCOREBOARD_VERSION_NUMBER, email: email, ip: Request.UserHostAddress, httpRaw: Request.ServerVariables["ALL_RAW"]);
            Response.Redirect("http://codingforcharity.org/rdnation/version/setup.exe");
            }
            catch (Exception e)
            {
                try
                {
                ErrorDatabaseManager.AddException(e, GetType());
                }
                catch { }
                Response.Redirect("http://codingforcharity.org/rdnation/version/setup.exe");
            }
        }
    }
}
