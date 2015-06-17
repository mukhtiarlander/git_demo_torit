using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RDN.Library.DataModels.Context;
using RDN.Library.Classes.Federation;
using RDN.Raspberry.Models.Utilities;
using RDN.Raspberry.Models.OutModel;
using RDN.Library.Cache;
using System.Net;
using RDN.Utilities.Config;
using RDN.Portable.Config;

namespace RDN.Raspberry.Controllers
{
    public class FederationController : BaseController
    {
        [Authorize]
        [HttpPost]
        public ActionResult AttachLeagueToFederation(IdModel mod)
        {
            mod.IsAttached = Library.Classes.Admin.League.League.AttachLeagueToFederation(new Guid(mod.Id), new Guid(mod.Id2));
            return View(mod);
        }

        [Authorize]
        public ActionResult AttachLeagueToFederation()
        {
            IdModel mod = new IdModel();
            mod.IsDeleted = false;
            mod.IsAttached = false;
            return View(mod);
        }

        [Authorize]
        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// screen for a federation thats pending approval.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult New()
        {


            var model = new SimpleModPager<Library.DataModels.Federation.Federation>();
            model.CurrentPage = 1;
            model.NumberOfRecords = Federation.GetNumberOfUnApprovedFederations();
            model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);

            var output = FillFederationModel(model);
            return View(output);
        }
        /// <summary>
        /// gets the form post to delete or approve a federation.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult New(SimpleModPager<Library.DataModels.Federation.Federation> model)
        {
            Guid fedId;
            if (Guid.TryParse(model.ItemToApprove, out fedId))
            {
                var memberId = Federation.ApproveFederation(fedId);

                //since the federation approval is in the memberCache Object, 
                //we clear it by hitting a URL setup to clear the cache.
                WebClient client = new WebClient();
                
                client.DownloadStringAsync(new Uri(LibraryConfig.URL_TO_CLEAR_MEMBER_CACHE +memberId.ToString()));
                WebClient client1 = new WebClient();
                client1.DownloadStringAsync(new Uri(LibraryConfig.URL_TO_CLEAR_MEMBER_CACHE_API +memberId.ToString()));
            }
            else if (Guid.TryParse(model.ItemToDelete, out fedId))
            {
                Federation.DeleteFederation(fedId);
            }
            var output = FillFederationModel(model);
            return View(output);
        }
        /// <summary>
        /// fills out the unapproved federations.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private GenericSingleModel<SimpleModPager<Library.DataModels.Federation.Federation>> FillFederationModel(SimpleModPager<Library.DataModels.Federation.Federation> model)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimpleModPager<Library.DataModels.Federation.Federation>> { Model = model };
            output.Model.Items = Federation.GetAllUnApprovedFederations((model.CurrentPage - 1) * 20, 20);
            return output;
        }

    }
}
