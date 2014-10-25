using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using RDN.Library.Classes.Admin.Admin.Classes;
using RDN.Library.Classes.Admin.Admin.Enums;
using RDN.Library.Classes.EmailServer;
using RDN.Library.DataModels;
using RDN.Raspberry.Models.Admin;
using RDN.Raspberry.Models.OutModel;
using RDN.Raspberry.Models.Utilities;
using RDN.Library.DataModels.Scoreboard;
using RDN.Utilities.Config;
using RDN.Library.Classes.Error;
using RDN.Utilities.Error;
using RDN.Portable.Config;



namespace RDN.Raspberry.Controllers
{
    public class AdminController : BaseController
    {
        [Authorize]
        public ActionResult AddEmailToSubscriberList()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddEmailToSubscriberList(IdModel mod)
        {
            mod.IsSuccess = Library.Classes.EmailServer.EmailServer.AddEmailToSubscriberList(mod.Id);
            return View(mod);
        }
        [Authorize]
        public ActionResult AddEmailToNonSubscriberList()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddEmailToNonSubscriberList(IdModel mod)
        {
            mod.IsSuccess = Library.Classes.EmailServer.EmailServer.AddEmailToUnsubscribeList(mod.Id);
            return View(mod);
        }
        [ValidateInput(false)]
        [Authorize]
        public ActionResult AdminEmailMessages()
        {
            AdminMessagesModel model = new AdminMessagesModel();
            model.Items = RDN.Library.Classes.Admin.Admin.Admin.GetLastAdminEmailMessages(10);
            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost]
        [Authorize]
        public ActionResult AdminEmailMessages(AdminMessagesModel m)
        {
            RDN.Library.Classes.Admin.Admin.Admin.SaveNextAdminMessage(m.NewMessage);
            AdminMessagesModel model = new AdminMessagesModel();
            model.Items = RDN.Library.Classes.Admin.Admin.Admin.GetLastAdminEmailMessages(10);
            return View(model);
        }

        [Authorize]
        public ActionResult ChangeEmailFromThenTo()
        {
            IdModel mod = new IdModel();
            mod.IsSuccess = false;
            return View(mod);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangeEmailFromThenTo(IdModel mod)
        {
            mod.IsSuccess = Library.Classes.EmailServer.EmailServer.ChangeEmailFromThenTo(mod.Id, mod.Id2);
            return View(mod);
        }


        [Authorize]
        public ActionResult Index()
        {
            var output = new EmptyModel();
            return View(output);
        }

        [Authorize]
        public ActionResult SendMassScoreboardEmails()
        {
            var output = new GenericSingleModel<MassEmail>(new MassEmail());
            return View(output);
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SendMassScoreboardEmails(MassEmail model)
        {
            var output = new GenericSingleModel<MassEmail>(model);
            bool result = false;
            try
            {
                //sends mass email to all leagues
                if (model.IsMassSendVerified)
                {
                    switch (model.MassEmailType)
                    {
                        case MassEmailEnum.AllEmailsToSendMontlyUpdatesTo:
                            result = Library.Classes.Admin.Admin.Admin.SendMassEmailsForMonthlyBroadcasts(model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.AllLeaguesWorldWide:
                            result = Library.Classes.Admin.Admin.Admin.SendMassScoreboardEmailsForLeaguesWorldWide(model.IsMassSendVerified, model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.AllScoreboardDownloadersAndFeedbackers:
                            result = Library.Classes.Admin.Admin.Admin.SendMassScoreboardEmailsForFeedback(model.IsMassSendVerified, model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.RefMasterList:
                            result = Library.Classes.Admin.Admin.Admin.SendMassScoreboardEmailsForMasterRefRoster(model.IsMassSendVerified, model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.AllEmailsAvailable:
                            result = Library.Classes.Admin.Admin.Admin.SendMassEmailsToAllContacts(model.IsMassSendVerified, model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.AllRegisteredUsers:
                            result = Library.Classes.Admin.Admin.Admin.SendMassScoreboardEmailsForAllUsers(model.IsMassSendVerified, model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.AllRegisteredLeagues:
                            result = Library.Classes.Admin.Admin.Admin.SendMassEmailsToRegisteredLeagues(model.IsMassSendVerified, model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.AllRegisteredEmails:
                            result = Library.Classes.Admin.Admin.Admin.SendMassEmailsForAllRegisteredEmails(model.IsMassSendVerified, model.Subject, model.HtmlBody);
                            break;
                        case MassEmailEnum.AllLeaguesThatDontExistWithinRDNation:
                            result = Library.Classes.Admin.Admin.Admin.SendMassEmailsForLeaguesNotSignedUpToRDNation(model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                        case MassEmailEnum.AllLeagueOwners:
                            result = Library.Classes.Admin.Admin.Admin.SendMassEmailsToOwnersOfLeagues(model.Subject, model.HtmlBody, model.TestEmail);
                            break;
                    }
                }
                else
                    result = Library.Classes.Admin.Admin.Admin.SendTestEmail(model.Subject, model.HtmlBody, model.TestEmail);



                if (!result)
                    AddMessage(new SiteMessage { MessageType = SiteMessageType.Error, Message = "An error occured" });
                else
                {
                    if (model.IsMassSendVerified)
                    {
                        AddMessage(new SiteMessage { MessageType = SiteMessageType.Success, Message = "Successfully sent out mass emails" });
                        return RedirectToAction("Index", "Admin");
                    }

                    AddMessage(new SiteMessage { MessageType = SiteMessageType.Success, Message = "Successfully sent out a test email to: " + model.TestEmail });
                    return View(output);
                }
            }
            catch (Exception e)
            {
                ErrorDatabaseManager.AddException(e, e.GetType(), errorGroup: ErrorGroupEnum.Database);
            }
            return View(output);
        }

        [Authorize]
        public ActionResult AddLeagueContact()
        {
            var model = new AddLeagueContact();
            FillAddLeagueContactModel(ref model);

            var output = new GenericSingleModel<AddLeagueContact>(model);
            return View(output);
        }

        [Authorize]
        public ActionResult TestEmailLayout()
        {
            return View(new TestEmailLayout
            {
                Sent = false,
                LayoutId = "Blank",
                Xml =
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n" +
                    "<properties>\n" +
                    "    <body>Value of the body tag!</body>\n" +
                    "</properties>"
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TestEmailLayout(TestEmailLayout content)
        {
            try
            {
                if (string.IsNullOrEmpty(content.Email))
                {
                    content.Sent = false;
                    content.Error = "No email specified";
                    return View(content);
                }
                if (string.IsNullOrEmpty(content.LayoutId))
                {
                    content.Sent = false;
                    content.Error = "No layout specified";
                    return View(content);
                }

                var body = content.Xml.Replace("\r\n", string.Empty).Replace(@"\", string.Empty);
                var xml = XDocument.Parse(body);

                var emailData = xml.Root.Elements().ToDictionary(xNode => xNode.Name.LocalName, xNode => xNode.Value);

                EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, content.Email, "Email layout test - RDNation.com", emailData, (EmailServerLayoutsEnum)Enum.Parse(typeof(EmailServerLayoutsEnum), content.LayoutId));
                content.Sent = true;
            }
            catch (Exception e)
            {
                content.Error = e.Message;
                content.Sent = false;
            }
            return View(content);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddLeagueContact(AddLeagueContact model)
        {
            var result = Library.Classes.Admin.Admin.Admin.CreateContactLeague(model.Name, model.AssociationId, model.LeagueTypeId,
                                                                  model.CountryId, model.State, model.City,
                                                                  model.HomePage, model.Facebook, model.PrimaryEmails,
                                                                  model.Emails, model.Comments);
            if (result == CreateLeagueContactEnum.Saved)
            {
                model.Name = string.Empty;
                model.State = string.Empty;
                model.City = string.Empty;
                model.HomePage = string.Empty;
                model.Facebook = string.Empty;
                model.PrimaryEmails = string.Empty;
                model.Emails = string.Empty;
                model.Comments = string.Empty;

                AddMessage(new SiteMessage { MessageType = SiteMessageType.Success, Message = "League added." });
            }
            else if (result == CreateLeagueContactEnum.Error)
                AddMessage(new SiteMessage { MessageType = SiteMessageType.Error, Message = "An error occured." });
            else
                AddMessage(new SiteMessage { MessageType = SiteMessageType.Error, Message = "An incorrect email was detected in one of the email boxes. Remember, one valid email per line." });

            FillAddLeagueContactModel(ref model);
            var output = new GenericSingleModel<AddLeagueContact>(model);
            return View(output);
        }

        private void FillAddLeagueContactModel(ref AddLeagueContact model)
        {
            if (model.Countries == null || model.Countries.Count == 0)
            {
                model.Countries = new List<SelectListItem>();
                var countries = Library.Classes.Location.LocationFactory.GetCountriesDictionary();
                foreach (var country in countries)
                    model.Countries.Add(new SelectListItem
                                            {
                                                Text = country.Value,
                                                Value = country.Key.ToString(),
                                                Selected = country.Key.ToString() == model.CountryId
                                            });
            }

            if (model.Associations == null || model.Associations.Count == 0)
            {
                model.Associations = new List<SelectListItem>();
                var associations = Library.Classes.Admin.Admin.Admin.GetLeagueAssociations();
                foreach (var association in associations)
                    model.Associations.Add(new SelectListItem
                    {
                        Text = association.Value,
                        Value = association.Key.ToString(),
                        Selected = association.Key.ToString() == model.AssociationId
                    });
            }

            if (model.LeagueTypes == null || model.LeagueTypes.Count == 0)
            {
                model.LeagueTypes = new List<SelectListItem>();
                var types = Library.Classes.Admin.Admin.Admin.GetLeagueTypes();
                foreach (var type in types)
                    model.LeagueTypes.Add(new SelectListItem
                    {
                        Text = type.Value,
                        Value = type.Key.ToString(),
                        Selected = type.Key.ToString() == model.LeagueTypeId
                    });
            }
        }

        [Authorize]
        public ActionResult LeagueContacts()
        {
            var model = new SimplePager<ContactLeague>();
            model.CurrentPage = 1;
            model.NumberOfRecords = Library.Classes.Admin.Admin.Admin.GetContactLeagueCount();
            model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);

            var output = FillLeagueContacts(model);
            return View(output);
        }

        [Authorize]
        [HttpPost]
        public ActionResult LeagueContacts(SimplePager<ContactLeague> model)
        {
            if (model.ItemToDelete > 0)
            {
                Library.Classes.Admin.Admin.Admin.DeleteContactLeague(model.ItemToDelete);
                model.NumberOfRecords--;
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);
            }

            var output = FillLeagueContacts(model);
            return View(output);
        }

        private GenericSingleModel<SimplePager<ContactLeague>> FillLeagueContacts(SimplePager<ContactLeague> model)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimplePager<ContactLeague>> { Model = model };
            output.Model.Items = Library.Classes.Admin.Admin.Admin.GetContactLeague((model.CurrentPage - 1) * 20, 20);
            return output;
        }

        [Authorize]
        public ActionResult Errors()
        {
            var model = new SimplePager<Library.Classes.Error.Classes.Error>();
            model.CurrentPage = 1;
            model.NumberOfRecords = Library.Classes.Error.ErrorDatabaseManager.GetNumberOfExceptions();
            model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);

            var output = FillErrorModel(model);
            return View(output);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Errors(SimplePager<Library.Classes.Error.Classes.Error> model)
        {
            if (model.DeleteAll)
            {

                model.CurrentPage = 1;
                model.NumberOfRecords = model.NumberOfRecords - Library.Classes.Error.ErrorDatabaseManager.DeleteAllErrorObjects();
                model.NumberOfPages = 1;
            }
            else if (model.ItemToDelete > 0)
            {
                Library.Classes.Error.ErrorDatabaseManager.DeleteErrorObject(model.ItemToDelete);
                model.NumberOfRecords--;
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);
            }

            var output = FillErrorModel(model);
            return View(output);
        }

        private GenericSingleModel<SimplePager<Library.Classes.Error.Classes.Error>> FillErrorModel(SimplePager<Library.Classes.Error.Classes.Error> model)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimplePager<Library.Classes.Error.Classes.Error>> { Model = model };
            output.Model.Items = Library.Classes.Error.ErrorDatabaseManager.GetErrorObjects((model.CurrentPage - 1) * 20, 20);
            return output;
        }

        //[Authorize]
        //public ActionResult ErrorsOld()
        //{
        //    var model = new SimplePager<Exception>();
        //    model.CurrentPage = 1;
        //    model.NumberOfRecords = Library.ViewModel.ErrorServerViewModel.GetNumberOfExceptions();
        //    model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);

        //    var output = FillErrorModelOld(model);
        //    return View(output);
        //}

        //[Authorize]
        //[HttpPost]
        //public ActionResult ErrorsOld(SimplePager<RDN_Errors_Log> model)
        //{
        //    if (model.ItemToDelete > 0)
        //    {
        //        Library.ViewModel.ErrorServerViewModel.DeleteError(model.ItemToDelete);
        //        model.NumberOfRecords--;
        //        model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);
        //    }

        //    var output = FillErrorModelOld(model);
        //    return View(output);
        //}

        //private GenericSingleModel<SimplePager<RDN_Errors_Log>> FillErrorModelOld(SimplePager<RDN_Errors_Log> model)
        //{
        //    for (var i = 1; i <= model.NumberOfPages; i++)
        //        model.Pages.Add(new SelectListItem
        //        {
        //            Text = i.ToString(),
        //            Value = i.ToString(),
        //            Selected = i == model.CurrentPage
        //        });
        //    var output = new GenericSingleModel<SimplePager<RDN_Errors_Log>> { Model = model };
        //    output.Model.Items = Library.ViewModel.ErrorServerViewModel.GetExceptions((model.CurrentPage - 1) * 20, 20);
        //    return output;
        //}

        [Authorize]
        public ActionResult Feedback()
        {
            var model = new SimplePager<ScoreboardFeedback>();
            model.CurrentPage = 1;
            model.NumberOfRecords = Library.ViewModel.ErrorServerViewModel.GetNumberOfFeedbackItems();
            model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);

            var output = FillFeedbackModel(model);
            return View(output);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Feedback(SimplePager<ScoreboardFeedback> model)
        {
            if (model.ItemToDelete > 0)
            {
                Library.ViewModel.ErrorServerViewModel.DeleteFeedbackItem(model.ItemToDelete);
                model.NumberOfRecords--;
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);
            }
            else if (model.ItemToArchive > 0)
            {
                Library.ViewModel.ErrorServerViewModel.ArchiveFeedbackItem(model.ItemToArchive);
                model.NumberOfRecords--;
                model.NumberOfPages = (int)Math.Ceiling((double)model.NumberOfRecords / 20);
            }

            var output = FillFeedbackModel(model);
            return View(output);
        }

        private GenericSingleModel<SimplePager<ScoreboardFeedback>> FillFeedbackModel(SimplePager<ScoreboardFeedback> model)
        {
            for (var i = 1; i <= model.NumberOfPages; i++)
                model.Pages.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i == model.CurrentPage
                });
            var output = new GenericSingleModel<SimplePager<ScoreboardFeedback>> { Model = model };
            output.Model.Items = Library.ViewModel.ErrorServerViewModel.GetFeedbackItems((model.CurrentPage - 1) * 20, 20);
            return output;
        }
    }
}
