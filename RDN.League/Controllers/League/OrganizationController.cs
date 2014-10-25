using RDN.Library.Cache;
using RDN.Library.Classes.Error;
using RDN.Library.Util;
using RDN.Library.Classes.League;
using RDN.Library.Util.Enum;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDN.League.Controllers.League
{
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
    public class OrganizationController : BaseController
    {

        #region Designation

        public ActionResult Designation()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Designation Saved.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New Designation Successfully Added.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Designation Successfully Deleted.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Designation Successfully Filled.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Designation Successfully Closed.";
                    this.AddMessage(message);
                }
                var leagueId = MemberCache.GetLeagueIdOfMember();

                var designationLists = RDN.Library.Classes.League.Organization.GetDesignationList(leagueId);
                return View(designationLists);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult AddForm()
        {
            return View();
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult AddDesignation(Organization oGanization)
        {
            try
            {
                oGanization.LeagueId = MemberCache.GetLeagueIdOfMember();
                 
                bool execute = RDN.Library.Classes.League.Organization.AddNewDesignation(oGanization);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/league/organization/Designation?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult EditDesignation(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }

                var Data = RDN.Library.Classes.League.Organization.GetData(id, new Guid(leagueId));

                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult EditDesignation(Organization oDesig)
        {
            try
            {
                bool execute = RDN.Library.Classes.League.Organization.UpdateDesignation(oDesig);

                return Redirect(Url.Content("~/organization/Designation?u=" + SiteMessagesEnum.s));
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));

        }
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult ViewDetails(long id, string leagueId)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                if (!MemberCache.IsMemberApartOfLeague(memId, new Guid(leagueId)))
                {
                    return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.na));
                }
                var Data = RDN.Library.Classes.League.Organization.GetData(id, new Guid(leagueId));
                if (!String.IsNullOrEmpty(Data.ShortNote))
                {
                    Data.ShortNote = Data.ShortNote.Replace(Environment.NewLine, "<br/>");
                }
                return View(Data);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/?u=" + SiteMessagesEnum.sww));
        }

        #endregion Designation

        #region Organization Chart

/*
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult AddOrganize()
        {
            try
            { 
                var leagueId = MemberCache.GetLeagueIdOfMember();
                var Designations = RDN.Library.Classes.League.Organization.GetDesignationList(leagueId);
                var list = (from a in Designations
                            select new SelectListItem
                            {
                                Text = a.DesigTitle,
                                Value = ((int)a.DesignationId).ToString()
                            });

                ViewBag.DesignagtionList = new SelectList(list, "Value", "Text");  //Designation

                //Member List for Dropdown 
                var Members = RDN.Library.Classes.League.Organization.MemberList(leagueId);
                var Memberlist = (from a in Members
                            select new SelectListItem
                            {
                                Text = a.DerbyName,
                                Value = a.MemberId.ToString()
                            });
                ViewBag.MemberList = new SelectList(Memberlist, "Value", "Text");


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();

        }
*/
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult AddOrganize(Organization oRGanization)
        {
            
            try
            {
                oRGanization.LeagueId = MemberCache.GetLeagueIdOfMember();
                oRGanization.OrganizedBy = RDN.Library.Classes.Account.User.GetMemberId().ToString();

                bool execute = RDN.Library.Classes.League.Organization.NewOrganize(oRGanization);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/league/organization/chart/" + oRGanization.OrganizationId + "/" + oRGanization.LeagueId + "?u=" + SiteMessagesEnum.sac));  //league/organization/chart/{id}/{leagueId}
        } 
#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public JsonResult GetRank(int designationId)
        {
            Organization model = null;
            try
            {       
                    var leagueId = MemberCache.GetLeagueIdOfMember();
                    //bool check = RDN.Library.Classes.League.Organization.IsBossExist(designationId, leagueId);

                    var designationData = RDN.Library.Classes.League.Organization.GetData(designationId, leagueId);
                    
                    model = new Organization { DesignationId = designationData.DesignationId, DesigLavel = designationData.DesigLavel, DesigTitle = designationData.DesigTitle };
                    return Json(model, JsonRequestBehavior.AllowGet);
    
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        public ActionResult OrgChart(long id,string leagueId)
        {
            try
            { 
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                    string updated = nameValueCollection["u"];

                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "chart Saved.";
                        this.AddMessage(message);

                    }
                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "New position Successfully Added.";
                        this.AddMessage(message);
                    }
                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "Position Successfully Deleted.";
                        this.AddMessage(message);
                    }
                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "Position Successfully Filled.";
                        this.AddMessage(message);
                    }
                    if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                    {
                        SiteMessage message = new SiteMessage();
                        message.MessageType = SiteMessageType.Info;
                        message.Message = "Position Successfully Closed.";
                        this.AddMessage(message);
                    }
                    
                //var leagueId = MemberCache.GetLeagueIdOfMember();
                var Designations = RDN.Library.Classes.League.Organization.GetDesignationList(new Guid(leagueId));
                var list = (from a in Designations
                            select new SelectListItem
                            {
                                Text = a.DesigTitle,
                                Value = ((int)a.DesignationId).ToString()
                            });

                ViewBag.DesignagtionList = new SelectList(list, "Value", "Text");  //Designation

                //Member List for Dropdown 
                var Members = RDN.Library.Classes.League.Organization.MemberList(new Guid(leagueId));
                var Memberlist = (from a in Members
                                  select new SelectListItem
                                  {
                                      Text = a.DerbyName,
                                      Value = a.MemberId.ToString()
                                  });
                ViewBag.MemberList = new SelectList(Memberlist, "Value", "Text");
                 
                ViewBag.OrganizationId = id;

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return View();
        }

        public JsonResult GetChartData(long organizationId)
        {
             //Organization model = null;
            //var v = organizationId;
            var leagueId = MemberCache.GetLeagueIdOfMember();
            var org = RDN.Library.Classes.League.Organization.GetOrgList(leagueId, organizationId);

           
            return Json(org, JsonRequestBehavior.AllowGet);

                 //var data = new[] {  new { Name = "Mike" , ReportsTo= "",Designation="The President" },
                 //                    new { Name = "Jim" , ReportsTo="Mike", Designation="VP" },
                 //                    new { Name = "KJim" , ReportsTo="Mike", Designation="IVP" },

                 //};
                 //return Json(data, JsonRequestBehavior.AllowGet);
              
            //return Json(data,JsonRequestBehavior.AllowGet);
        }

        #endregion Organization Chart


#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]
        public ActionResult AddOrganization(string leagueId)
        {
            return View();

        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult AddOrganization(Organization oGanization)
        {
            try
            {
                oGanization.LeagueId = MemberCache.GetLeagueIdOfMember();

                bool execute = RDN.Library.Classes.League.Organization.NewOrganization(oGanization);

            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return Redirect(Url.Content("~/league/organization/view/all?u=" + SiteMessagesEnum.sac));
        }

#if !DEBUG
[RequireHttps] //apply to all actions in controller
#endif
        [Authorize]

        public ActionResult ViewAllOrganization()
        {
            try
            {
                NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Request.Url.Query);
                string updated = nameValueCollection["u"];

                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.s.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Organization Information Updated.";
                    this.AddMessage(message);

                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.sac.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "New Orgnization Successfully Added.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.de.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Organization Successfully Deleted.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.et.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Designation Successfully Filled.";
                    this.AddMessage(message);
                }
                if (!String.IsNullOrEmpty(updated) && updated == SiteMessagesEnum.cl.ToString())
                {
                    SiteMessage message = new SiteMessage();
                    message.MessageType = SiteMessageType.Info;
                    message.Message = "Designation Successfully Closed.";
                    this.AddMessage(message);
                }
                var leagueId = MemberCache.GetLeagueIdOfMember();

                var organizationLists = RDN.Library.Classes.League.Organization.GetOrganizationList(leagueId);
                return View(organizationLists);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }


            return View();
        }

    }
}
