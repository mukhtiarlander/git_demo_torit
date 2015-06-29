using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.Cache;
using RDN.Library.Classes.Store;
using RDN.Models.OutModel;
using RDN.Models.Utilities;
using RDN.Portable.Classes.Federation;

namespace RDN.Controllers
{
    public class PublicFederationController : Controller
    {
        private readonly int DEFAULT_PAGE_SIZE = 100;

        public ActionResult AllFederations(int? page)
        {
            var model = new SimpleModPager<FederationDisplay>();
            if (page == null)
                model.CurrentPage = 1;
            else
                model.CurrentPage = page.Value;
            model.NumberOfRecords = SiteCache.GetNumberOfFederationsSignedUp();
            model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / DEFAULT_PAGE_SIZE);
            model.PageSize = DEFAULT_PAGE_SIZE;
            var output = FillFederationModel(model);
         

            return View(output);
        }

        private GenericSingleModel<SimpleModPager<FederationDisplay>> FillFederationModel(SimpleModPager<FederationDisplay> model)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimpleModPager<FederationDisplay>> { Model = model };

            output.Model.Items = RDN.Library.Classes.Federation.Federation.GetFederationsForDisplay();
            return output;
        }

        public ActionResult Federation(string name, string id)
        {

            var fed =FederationCache.GetFederation(new Guid(id));
            

            return View(fed);
        }

    }
}
