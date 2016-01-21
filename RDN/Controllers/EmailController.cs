using Common.EmailServer.Library.Classes.Subscribe;
using RDN.Models.Email;
using System;
using System.Web.Mvc;

namespace RDN.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult UnSubscribe(string email, string listType, string id)
        {
            EmailOutModel outModel = new EmailOutModel();
            outModel.Email = email;
            outModel.Successful = SubscriberManager.UnSubscribe((SubscriberType)Convert.ToInt64(listType), email, id);

            return View(outModel);
        }


    }
}