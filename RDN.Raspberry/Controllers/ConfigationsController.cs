using Common.Site.Classes.Configations;
using Common.Site.Database.Context;
using RDN.Raspberry.Models.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.Raspberry.Controllers
{
    public class ConfigationsController : BaseController
    {
        // GET: Configations
        ConfigurationManager _db;
        public ActionResult ViewConfigurations()
        {
            ConfigurationModel model = new ConfigurationModel();
             _db=new ConfigurationManager();

            model.Items= _db.GetConfigurations();

            if (TempData["Deleted"] != null)
                ViewBag.IsDeleted = "Yes";

            return View(model);
        }
        
        [HttpPost]
        public ActionResult Create(ConfigurationModel model)
        {
            try
            {   
                _db = new ConfigurationManager();

                SiteConfiguration siteConfiguration = new SiteConfiguration { Key = model.Key, Value = model.Value, Discription = model.Discription };
                
                _db.SaveConfigurations(siteConfiguration);

                return RedirectToAction("ViewConfigurations");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(long id,string key,string value,string description)
        {
            _db = new ConfigurationManager();

            SiteConfiguration siteConfiguration = new SiteConfiguration {ID=id, Key = key, Value = value, Discription = description };

           bool isSuccess= _db.EditConfigurations(siteConfiguration);            
            
          return Json(isSuccess);
        }

       
        // GET: Configations/Delete/5
        public ActionResult Delete(long id)
        {
            ConfigurationModel model = new ConfigurationModel();
            _db = new ConfigurationManager();

             bool isDeleted=_db.DeleteConfiguration(id);
             if (isDeleted)
                 TempData["Deleted"]="YES";

            return RedirectToAction("ViewConfigurations");            
        }
    }
}
