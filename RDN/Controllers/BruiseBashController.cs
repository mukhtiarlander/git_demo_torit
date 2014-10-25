using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Models.BruiseBash;
using RDN.Library.Classes.BruiseBash;

namespace RDN.Controllers
{
    public class BruiseBashController : Controller
    {
        //
        // GET: /BruiseBash/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
        [Authorize]
        public ActionResult Add()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Add(BruiseBashAddModel model, HttpPostedFileBase file)
        {

            BruiseBash car = new BruiseBash();
            car.Title = model.Title;
            car.Story = model.Story;
            car.File = file.FileName;


            Guid memberId = RDN.Library.Classes.Account.User.GetMemberId();

            Guid BruiseBashItemId = BruiseBash.SaveBruiseBashItem(car, file.InputStream, memberId);

            return Redirect(Url.Content("~/bruisebash/bruise/" + BruiseBashItemId.ToString().Replace("-", "") + "/" + model.Title));



        }

        public ActionResult ViewBruise(string id, string title)
        {

            BruiseBash bruise = BruiseBash.GetBruiseBashItem(new Guid(id));

            return View(bruise);
        }

    }
}
