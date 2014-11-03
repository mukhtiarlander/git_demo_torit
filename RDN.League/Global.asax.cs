using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RDN.League.Models.Enum;
using RDN.Library.Classes.Error;
using RDN.Library.DatabaseInitializers;
using RDN.Library.Util.Enum;
using RDN.Utilities.Config;
using StackExchange.Profiling;
using RDN.Portable.Config;
using RDN.Library.Cache;
using RDN.Library.Cache.Singletons;
using System.Configuration;
using RDN.Library.Classes.Payment.Enums;
using StackExchange.Profiling.EntityFramework6;
using System.Web.Optimization;

namespace RDN.League
{


    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            try
            {
                if (!HttpContext.Current.Request.IsLocal)
                {
                    filters.Add(new RequireHttpsAttribute());
                }
            }
            catch
            { }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "ValidateEmailOld", // Route name
                "Manager/ValidateEmail/{email}/{code}", // URL with parameters
                new { controller = "Manager", action = "ValidateEmailOld" } // Parameter defaults
            );
            routes.MapRoute(
                "ValidateEmail", // Route name
                "Manager/validateemails/{code}", // URL with parameters
                new { controller = "Manager", action = "ValidateEmail" } // Parameter defaults
            );

            #region ContactRoutes
            routes.MapRoute(
             "AddContact", // Route name
             "contact/add/{type}", // URL with parameters
             new { controller = "Contact", action = "AddContact" } // Parameter defaults
         );
            routes.MapRoute(
         "EditContact", // Route name
         "contact/edit/{type}/{contactId}", // URL with parameters
         new { controller = "Contact", action = "EditContact" } // Parameter defaults
     );
            #endregion

            #region Documents
            routes.MapRoute(
            "DownloadFile", // Route name
            "document/download/{documentId}", // URL with parameters
            new { controller = "Document", action = "DownloadFile" } // Parameter defaults
        );

            routes.MapRoute(
          "ViewFile", // Route name
          "document/view/{documentId}", // URL with parameters
          new { controller = "Document", action = "ViewFile" } // Parameter defaults
      );
            #endregion

            #region Merchants
            routes.MapRoute(
          "MerchantSettings", // Route name
          "merchant/settings", // URL with parameters
          new { controller = "Merchant", action = "MerchantSettings" } // Parameter defaults
      );
            #endregion

            #region Paywalls
            routes.MapRoute(
          "Paywalls", // Route name
          "paywall/all", // URL with parameters
          new { controller = "Paywall", action = "Paywalls" } // Parameter defaults
      );
            routes.MapRoute(
    "PaywallPayments", // Route name
    "paywall/payments/{privId}/{storeId}", // URL with parameters
    new { controller = "Paywall", action = "PaywallPayments" } // Parameter defaults
);
            routes.MapRoute(
"PaywallOrder", // Route name
"paywall/order/{privId}/{storeId}/{invoiceId}", // URL with parameters
new { controller = "Paywall", action = "PaywallOrder" } // Parameter defaults
);
            routes.MapRoute(
     "PaywallEdit", // Route name
     "paywall/edit/{paywallid}", // URL with parameters
     new { controller = "Paywall", action = "EditPaywall" } // Parameter defaults
 );
            routes.MapRoute(
"Paywalladd", // Route name
"paywall/add", // URL with parameters
new { controller = "Paywall", action = "AddPaywall" } // Parameter defaults
);
            routes.MapRoute(
"PaywallView", // Route name
"paywall/view/{paywallid}", // URL with parameters
new { controller = "Paywall", action = "ViewPaywall" } // Parameter defaults
);
            #endregion

            #region BillingRoutes
            routes.MapRoute(
"LeagueBilling", // Route name
"billing/league/{leagueId}", // URL with parameters
new { controller = "Billing", action = "LeagueBilling" } // Parameter defaults
);
            routes.MapRoute(
"LeagueBillingHistory", // Route name
"billing/league/history/{leagueId}", // URL with parameters
new { controller = "Billing", action = "LeagueBillingHistory" } // Parameter defaults
);
            routes.MapRoute(
"LeagueReceipt", // Route name
"billing/league/receipt/{invoiceId}", // URL with parameters
new { controller = "Billing", action = "LeagueReceipt" } // Parameter defaults
);
            routes.MapRoute(
"LeagueBillingAddSubscription", // Route name
"billing/league/addsubscription/{leagueId}", // URL with parameters
new { controller = "Billing", action = "LeagueAddSubscription" } // Parameter defaults
);
            routes.MapRoute(
"LeagueBillingUpdateSubscription", // Route name
"billing/league/update/{leagueId}", // URL with parameters
new { controller = "Billing", action = "LeagueBillingUpdateInfo" } // Parameter defaults
);

            #endregion

            #region StoreRoutes

            routes.MapRoute(
"StoreHome", // Route name
"store/home/{privId}/{storeId}", // URL with parameters
new { controller = "Store", action = "StoreHome", storeId = UrlParameter.Optional, privId = UrlParameter.Optional } // Parameter defaults
);
            routes.MapRoute(
"StoreNewItem", // Route name
"store/new/{privId}/{storeId}", // URL with parameters
new { controller = "Store", action = "StoreNewItem" } // Parameter defaults
);
            routes.MapRoute(
"StoreOrder", // Route name
"store/order/{privId}/{storeId}/{invoiceId}", // URL with parameters
new { controller = "Store", action = "StoreOrder" } // Parameter defaults
);
            //if you change this route, be sure to change links in CompileEmails in RDN.Library.Classes.Payment.Paypal.IPNHandler
            routes.MapRoute(
"StoreOrders", // Route name
"store/orders/{privId}/{storeId}", // URL with parameters
new { controller = "Store", action = "StoreOrders" } // Parameter defaults
);

            routes.MapRoute(
"StoreEditItem", // Route name
"store/item/edit/{itemId}/{privId}/{storeId}", // URL with parameters
new { controller = "Store", action = "StoreEditItem" } // Parameter defaults
);
            routes.MapRoute(
"StoreSettings", // Route name
"store/settings/{privId}/{storeId}", // URL with parameters
new { controller = "Store", action = "StoreSettings" } // Parameter defaults
);
            routes.MapRoute(
"ConfirmFromStripe", // Route name
"store/confirmfromstripe", // URL with parameters
new { controller = "Store", action = "ConfirmFromStripe" } // Parameter defaults
);
            #endregion

            #region ScoreboardRoutes
            routes.MapRoute(
"ScoreboardManageLiveGame", // Route name
"scoreboard/manage/{privatePass}/{gameId}", // URL with parameters
new { controller = "Scoreboard", action = "ManageGame", id = UrlParameter.Optional } // Parameter defaults
);
            #endregion

            #region MemberRoutes
            routes.MapRoute(
"ChangeEmailNotificationSetting", // Route name
"Member/ChangeEmailNotificationSetting", // URL with parameters
new { controller = "Member", action = "ChangeEmailNotificationSetting" } // Parameter defaults
);
            routes.MapRoute(
"MemberRetireSelf", // Route name
"Member/RetireSelf", // URL with parameters
new { controller = "Member", action = "RetireSelf" } // Parameter defaults
);
            routes.MapRoute(
"MemberVerifySms", // Route name
"member/verifysms", // URL with parameters
new { controller = "Member", action = "VerifySMS" } // Parameter defaults
);
            routes.MapRoute(
"RemoveMemberFromLeague", // Route name
"member/removememberfromleague", // URL with parameters
new { controller = "Member", action = "RemoveMemberFromLeague" } // Parameter defaults
);
            routes.MapRoute(
"SetPrivacySetting", // Route name
"member/setprivacysetting", // URL with parameters
new { controller = "Member", action = "SetPrivacySetting" } // Parameter defaults
);
            routes.MapRoute(
"MemberVerifySmsCode", // Route name
"member/verifysmscode", // URL with parameters
new { controller = "Member", action = "VerifySMSCode" } // Parameter defaults
);

            routes.MapRoute(
"MemberUnRetireSelf", // Route name
"Member/UnRetireSelf", // URL with parameters
new { controller = "Member", action = "UnRetireSelf" } // Parameter defaults
);

            routes.MapRoute(
"MemberEditAdmin", // Route name
"Member/Edit/{id}", // URL with parameters
new { controller = "Member", action = "EditMemberMySelf", id = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
"MemberChangeEmail", // Route name
"member/username", // URL with parameters
new { controller = "Member", action = "ChangeEmail" } // Parameter defaults
);
            routes.MapRoute(
"MemberChangePassword", // Route name
"Member/Password", // URL with parameters
new { controller = "Member", action = "ChangePassword" } // Parameter defaults
);
            routes.MapRoute(
"MemberMedical", // Route name
"member/medical", // URL with parameters
new { controller = "Member", action = "MemberMedical" } // Parameter defaults
);
            routes.MapRoute(
"MemberSettings", // Route name
"member/settings", // URL with parameters
new { controller = "Member", action = "MemberSetting" } // Parameter defaults
);
            routes.MapRoute(
"MemberSettingsChangeCalView", // Route name
"member/ChangeMemberSettingCalView", // URL with parameters
new { controller = "Member", action = "ChangeMemberSettingCalView" } // Parameter defaults
);
            routes.MapRoute(
"MemberContacts", // Route name
"member/contacts/{id}", // URL with parameters
new { controller = "Member", action = "MemberContacts", id = UrlParameter.Optional } // Parameter defaults
);
            routes.MapRoute(
"MemberContactsAdd", // Route name
"member/addcontact", // URL with parameters
new { controller = "Member", action = "AddMemberContact" } // Parameter defaults
);
            routes.MapRoute(
"MemberContactsRemove", // Route name
"member/removecontact", // URL with parameters
new { controller = "Member", action = "RemoveContact" } // Parameter defaults
);

            //            routes.MapRoute(
            //"MemberEdit", // Route name
            //"Member/Edit", // URL with parameters
            //new { controller = "Member", action = "EditMemberMySelf" } // Parameter defaults
            //);




            routes.MapRoute(
"MemberView", // Route name
"Member/{id}/{name}", // URL with parameters
new { controller = "Member", action = "ViewMember", name = UrlParameter.Optional } // Parameter defaults
);

            #endregion

            #region GameAndTournamentRoutes

            routes.MapRoute(
"TournamentCreate", // Route name
"tournament/create", // URL with parameters
new { controller = "Tournament", action = "CreateTournament" } // Parameter defaults
);
            routes.MapRoute(
"TournamentOwners", // Route name
"tournament/owners/{pid}/{id}", // URL with parameters
new { controller = "Tournament", action = "TournamentOwners" } // Parameter defaults
);
            routes.MapRoute(
"TournamentView", // Route name
"tournament/view/{pid}/{id}", // URL with parameters
new { controller = "Tournament", action = "ViewTournament" } // Parameter defaults
);
            routes.MapRoute(
"TournamentBrackets", // Route name
"tournament/brackets/{pid}/{id}", // URL with parameters
new { controller = "Tournament", action = "BracketsForTournament" } // Parameter defaults
);
            routes.MapRoute(
"TournamentViewAllOwned", // Route name
"tournament/view/all", // URL with parameters
new { controller = "Tournament", action = "ViewTournaments" } // Parameter defaults
);

            routes.MapRoute(
"GameUpload", // Route name
"game/upload", // URL with parameters
new { controller = "Game", action = "Upload" } // Parameter defaults
);
            routes.MapRoute(
"GameManageSingle", // Route name
"game/manage/{pid}/{id}", // URL with parameters
new { controller = "Game", action = "Manage" } // Parameter defaults
);

            routes.MapRoute(
"GameManageAll", // Route name
"game/manage/all", // URL with parameters
new { controller = "Game", action = "ManageAll" } // Parameter defaults
);

            //allows routes for the adding of old games played
            routes.MapRoute(
     "GameAddOld", // Route name
     "game/add/{name}/{id}", // URL with parameters
     new { controller = "Game", action = "AddOld" } // Parameter defaults
 );
            routes.MapRoute(
 "GameViewAll", // Route name
 "game/view/all/{type}", // URL with parameters
 new { controller = "Game", action = "ViewAll" } // Parameter defaults
);
            routes.MapRoute(
"GameView", // Route name
"game/{id}/{name}", // URL with parameters
new { controller = "Game", action = "ViewGame" } // Parameter defaults
);
            routes.MapRoute(
"BoutChallenge", // Route name
"boutchallenge/add/request", // URL with parameters
new { controller = "BoutChallenge", action = "BoutChallengeRequest" } // Parameter defaults
);
             routes.MapRoute(
"ViewAllRequest", // Route name
"boutchallenge/view/all", // URL with parameters
new { controller = "BoutChallenge", action = "ViewAllBoutRequest" } // Parameter defaults
);
             routes.MapRoute(
"EditRequest", // Route name
"boutchallenge/edit/{id}/{leagueId}", // URL with parameters
new { controller = "BoutChallenge", action = "EditBoutRequest" } // Parameter defaults
);
             routes.MapRoute(
"DeleteRequest", // Route name
"boutchallenge/delete/{id}/{leagueId}", // URL with parameters
new { controller = "BoutChallenge", action = "DeleteBoutRequest" } // Parameter defaults
);
             routes.MapRoute(
"CloseRequest", // Route name
"boutchallenge/close/{id}/{leagueId}", // URL with parameters
new { controller = "BoutChallenge", action = "CloseBoutRequest" } // Parameter defaults
);
             routes.MapRoute(
"ViewEvent", // Route name
"boutchallenge/view/{ChallengeId}/{leagueId}", // URL with parameters
new { controller = "BoutChallenge", action = "ViewEvent" } // Parameter defaults
);



            #endregion

            #region FederationRoutes
            routes.MapRoute(
             "FederationLeague", // Route name
             "Federation/League/{id}/{name}", // URL with parameters
             new { controller = "Federation", action = "League" } // Parameter defaults
         );
            routes.MapRoute(
            "FederationLeagueEdit", // Route name
            "Federation/League/Edit/{id}/{name}", // URL with parameters
            new { controller = "Federation", action = "EditLeague" } // Parameter defaults
        );
            routes.MapRoute(
      "FederationLeaguesView", // Route name
      "Federation/Leagues/View", // URL with parameters
      new { controller = "Federation", action = "ViewLeagues" } // Parameter defaults
  );
            routes.MapRoute(
            "FederationMembersAdd", // Route name
            "Federation/Members/Add", // URL with parameters
            new { controller = "Federation", action = "AddMembers" } // Parameter defaults
        );
            routes.MapRoute(
      "FederationMembersView", // Route name
      "Federation/Members/View", // URL with parameters
      new { controller = "Federation", action = "ViewMembers" } // Parameter defaults
  );

            routes.MapRoute(
"FederationMemberView", // Route name
"Federation/Member/Edit/{id}/{name}", // URL with parameters
new { controller = "Federation", action = "EditMember", name = UrlParameter.Optional } // Parameter defaults
);
            #endregion

            #region teamRoutes
            routes.MapRoute(
         "addTeam", // Route name
         "team/add/{leagueId}", // URL with parameters
         new { controller = "Team", action = "AddTeam" }
     );

            routes.MapRoute(
         "editTeam", // Route name
         "team/edit/{id}", // URL with parameters
         new { controller = "Team", action = "ViewTeam" }
     );

            routes.MapRoute(
 "viewTeam", // Route name
 "team/view/{id}/{name}", // URL with parameters
 new { controller = "Team", action = "EditTeam" }
);
            #endregion

            #region leagueRoutes

            routes.MapRoute(
"LeagueContacts", // Route name
"league/contacts", // URL with parameters
new { controller = "League", action = "ContactsLeague" } // Parameter defaults
);
            routes.MapRoute(
"LeagueChangeToOtherLeague", // Route name
"league/change/{leagueId}", // URL with parameters
new { controller = "League", action = "ChangeToLeague" } // Parameter defaults
);
            routes.MapRoute(
  "LeagueDocuments", // Route name
  "league/documents/{leagueId}", // URL with parameters
  new { controller = "League", action = "Documents" } // Parameter defaults
);
            routes.MapRoute(
      "ViewDocumentComments", // Route name
      "league/document/comments/{documentId}/{leagueDocumentId}", // URL with parameters
      new { controller = "League", action = "DocumentComments" } // Parameter defaults
  );
            routes.MapRoute(
"LeagueDocumentsSettings", // Route name
"league/documents/settings/{leagueId}", // URL with parameters
new { controller = "League", action = "DocumentSettings" } // Parameter defaults
);

            routes.MapRoute(
        "LeagueGroups", // Route name
        "league/groups", // URL with parameters
        new { controller = "League", action = "Groups" } // Parameter defaults
    );
            routes.MapRoute(
       "LeagueGroupsAdd", // Route name
       "league/groups/add", // URL with parameters
       new { controller = "League", action = "GroupsAdd" } // Parameter defaults
   );

            routes.MapRoute(
"LeagueGroupsSettings", // Route name
"league/groups/settings/{id}", // URL with parameters
new { controller = "League", action = "GroupSettings" } // Parameter defaults
);

  


            routes.MapRoute(
             "LeagueViewTeams", // Route name
             "league/teams/view", // URL with parameters
             new { controller = "League", action = "ViewTeams" } // Parameter defaults
         );

            routes.MapRoute(
            "setupLeague", // Route name
            "League/Setup/{id}/{name}", // URL with parameters
            new { controller = "League", action = "Setup", name = UrlParameter.Optional, id = UrlParameter.Optional } // Parameter defaults
        );
            routes.MapRoute(
        "LeagueMembersAddClass", // Route name
        "league/member/add/class", // URL with parameters
        new { controller = "League", action = "AddSkaterClass" } // Parameter defaults
    );
            routes.MapRoute(
 "LeagueMembersAdd", // Route name
 "League/Members/Add", // URL with parameters
 new { controller = "League", action = "AddMembers" } // Parameter defaults
);
            routes.MapRoute(
      "LeagueMembersView", // Route name
      "league/members/view/all/{refresh}", // URL with parameters
      new { controller = "League", action = "ViewMembers", refresh = UrlParameter.Optional } // Parameter defaults
  );
            routes.MapRoute(
"LeagueMembersRemoved", // Route name
"league/members/view/removed", // URL with parameters
new { controller = "League", action = "ViewMembersRemoved"} // Parameter defaults
);
        
            routes.MapRoute(
"LeagueMembersPermissions", // Route name
"league/members/view/permissions", // URL with parameters
new { controller = "League", action = "ViewMembersPermissions" } // Parameter defaults
);

            routes.MapRoute(
"LeagueMembersViewOBSOLETE", // Route name
"league/members/view", // URL with parameters
new { controller = "League", action = "ViewMembers" } // Parameter defaults
);
            routes.MapRoute(
"LeagueMembersViewInsurance", // Route name
"league/members/view/insurance", // URL with parameters
new { controller = "League", action = "ViewMembersInsurance" } // Parameter defaults
);
            routes.MapRoute(
"LeagueMembersViewDates", // Route name
"league/members/view/dates", // URL with parameters
new { controller = "League", action = "ViewMembersDates" } // Parameter defaults
);
            routes.MapRoute(
"LeagueMembersViewJobs", // Route name
"league/members/view/jobs", // URL with parameters
new { controller = "League", action = "ViewMembersJobs" } // Parameter defaults
);
            routes.MapRoute(
"LeagueMembersViewMedical", // Route name
"league/members/view/medical", // URL with parameters
new { controller = "League", action = "ViewMembersMedical" } // Parameter defaults
);


            routes.MapRoute(
"ViewMembersReportBuilder", // Route name
"league/members/view/report", // URL with parameters
new { controller = "League", action = "ViewMembersReportBuilder" } // Parameter defaults
);

            routes.MapRoute(
"ViewMembersClass", // Route name
"league/members/view/class", // URL with parameters
new { controller = "League", action = "ViewMembersClassification" } // Parameter defaults
);
            routes.MapRoute(
"ViewMembersMap", // Route name
"league/members/view/map", // URL with parameters
new { controller = "League", action = "ViewMembersMap" } // Parameter defaults
);

            routes.MapRoute(
"LeagueMemberView", // Route name
"League/Member/Edit/{id}/{name}", // URL with parameters
new { controller = "League", action = "EditMember", name = UrlParameter.Optional } // Parameter defaults
);


            routes.MapRoute(
"LeagueEdit", // Route name
"league/edit/{id}", // URL with parameters
new { controller = "League", action = "EditLeague", name = UrlParameter.Optional } // Parameter defaults
);
           
            // Task List
            routes.MapRoute(
"NewTaskList", // Route name
"tasks/league/list/add", // URL with parameters
new { controller = "Task", action = "TaskList" } // Parameter defaults
);
            routes.MapRoute(
"TaskLists", // Route name
"tasks/league/list", // URL with parameters
new { controller = "Task", action = "ViewLists" } // Parameter defaults
);
            routes.MapRoute(
"ViewAllTasks", // Route name
"tasks/league/list/view/{id}/{listTitle}", // URL with parameters
new { controller = "Task", action = "ViewTasks" } // Parameter defaults
);
            routes.MapRoute(
"DeleteTaskList", // Route name
"tasks/league/task/delete/{id}/{listTitle}", // URL with parameters
new { controller = "Task", action = "DeleteTaskList" } // Parameter defaults
);
            routes.MapRoute(
"UpdateTaskList", // Route name
"tasks/league/list/update/{id}/{leagueId}", // URL with parameters
new { controller = "Task", action = "UpdateTaskList" } // Parameter defaults
);

            // Task
            routes.MapRoute(
"NewTask", // Route name
"tasks/league/task/add/{id}/{listTitle}", // URL with parameters
new { controller = "Task", action = "Task" } // Parameter defaults
);
            routes.MapRoute(
"ViewTaskDetails", // Route name
"tasks/league/task/view/{id}/{listTitle}", // URL with parameters
new { controller = "Task", action = "ViewTaskDetail" } // Parameter defaults
);
            routes.MapRoute(
"DeleteTask", // Route name
"task/delete/{id}/{taskListId}/{listTitle}", // URL with parameters
new { controller = "Task", action = "DeleteTask" } // Parameter defaults
);
            routes.MapRoute(
"UpdateTask", // Route name
"tasks/league/task/update/{id}/{taskTitle}", // URL with parameters
new { controller = "Task", action = "UpdateTask" } // Parameter defaults
);


            //Request Board

            routes.MapRoute(
"RequestAddBoard", // Route name
"officiating/addrequest", // URL with parameters
new { controller = "Officiating", action = "AddRequest" } // Parameter defaults
);
            routes.MapRoute(
"RequestBoardAll", // Route name
"officiating/requests", // URL with parameters
new { controller = "Officiating", action = "Requests" } // Parameter defaults
);
            routes.MapRoute(
"RequestEdit", // Route name
"officiating/editrequest/{id}/{memberId}", // URL with parameters
new { controller = "Officiating", action = "EditRequest" } // Parameter defaults
);
            routes.MapRoute(
"RequestDelete", // Route name
"officiating/Delete/request/{id}/{memberId}", // URL with parameters
new { controller = "Officiating", action = "DeleteRequest" } // Parameter defaults
);
            routes.MapRoute(
"RequestDetails", // Route name
"officiating/viewrequest/{id}/{memberId}", // URL with parameters
new { controller = "Officiating", action = "ViewRequest" } // Parameter defaults
);

            //Items for league

            routes.MapRoute(
"Item", // Route name
"league/inventory/add", // URL with parameters
new { controller = "Inventory", action = "AddNewItem" } // Parameter defaults
);
            routes.MapRoute(
"ItemList", // Route name
"league/inventory/all", // URL with parameters
new { controller = "Inventory", action = "ViewItems" } // Parameter defaults
);


            routes.MapRoute(
"ItemDelete", // Route name
"league/inventory/delete/{id}/{leagueId}", // URL with parameters
new { controller = "Inventory", action = "DeleteItem" } // Parameter defaults
);

            routes.MapRoute(
"EditItem", // Route name
"league/inventory/Edit/{id}/{leagueId}", // URL with parameters
new { controller = "Inventory", action = "EditItem" } // Parameter defaults
);
            routes.MapRoute(
"ItemDetails", // Route name
"league/inventory/Details/{id}/{leagueId}", // URL with parameters
new { controller = "Inventory", action = "ViewItem" } // Parameter defaults
);


            //Job Board for league

            routes.MapRoute(
"JobBoard", // Route name
"league/job/add", // URL with parameters
new { controller = "JobBoard", action = "AddJob"} // Parameter defaults
);
            routes.MapRoute(
"JobBoardView", // Route name
"league/jobboard", // URL with parameters
new { controller = "JobBoard", action = "ViewJobBoard" } // Parameter defaults
);
            routes.MapRoute(
"EditJobPost", // Route name
"league/Job/edit/{id}/{leagueId}", // URL with parameters
new { controller = "JobBoard", action = "EditJob" } // Parameter defaults
);
            routes.MapRoute(
"DeleteJob", // Route name
"league/job/delete/{id}/{leagueId}", // URL with parameters
new { controller = "JobBoard", action = "DeleteJob" } // Parameter defaults
);
            routes.MapRoute(
"CloseJob", // Route name
"league/job/close/{id}/{leagueId}", // URL with parameters
new { controller = "JobBoard", action = "CloseJob" } // Parameter defaults
);
            routes.MapRoute(
"FillJob", // Route name
"league/job/fill/{id}/{leagueId}", // URL with parameters
new { controller = "JobBoard", action = "FillJob" } // Parameter defaults
);
            routes.MapRoute(
"ViewJobDetails", // Route name
"view/job/detail/{id}/{leagueId}", // URL with parameters
new { controller = "JobBoard", action = "ViewJob" } // Parameter defaults
);

            //Sponsor for league

                routes.MapRoute(
"Sponsorship", // Route name
"league/sponsor/add", // URL with parameters
new { controller = "Sponsor", action = "AddNewSponsor" } // Parameter defaults
);
                routes.MapRoute(
"Sponsors", // Route name
"league/Sponsors", // URL with parameters
new { controller = "Sponsor", action = "ViewSponsors" } // Parameter defaults
);
                routes.MapRoute(
"EditSponsor", // Route name
"league/Sponsor/edit/{id}/{leagueId}", // URL with parameters
new { controller = "Sponsor", action = "EditSponsor" } // Parameter defaults
);
                 routes.MapRoute(
"DeleteSponsor", // Route name
"league/Sponsor/delete/{id}/{leagueId}", // URL with parameters
new { controller = "Sponsor", action = "DeleteSponsor" } // Parameter defaults
);
                 routes.MapRoute(
"SponsorDetails", // Route name
"league/Sponsor/details/{id}/{leagueId}", // URL with parameters
new { controller = "Sponsor", action = "ViewSponsor" } // Parameter defaults
);
                 routes.MapRoute(
"UseCode", // Route name
"league/Sponsor/UseCode/{id}/{leagueId}", // URL with parameters
new { controller = "Sponsor", action = "UseCode" } // Parameter defaults
);

                 //Designation for league Organization

                 routes.MapRoute(
     "Designations", // Route name
     "league/organization/designation", // URL with parameters
     new { controller = "Organization", action = "Designation" } // Parameter defaults
     );
                 routes.MapRoute(
     "AddNewDesignation", // Route name
     "league/organization/designation/add", // URL with parameters
     new { controller = "Organization", action = "AddForm" } // Parameter defaults
     );
                 routes.MapRoute(
    "EditDesignation", // Route name
    "league/organization/designation/edit/{id}/{leagueId}", // URL with parameters
    new { controller = "Organization", action = "EditDesignation" } // Parameter defaults
    ); 
                 routes.MapRoute(
    "DetailsDesignation", // Route name
    "league/organization/designation/details/{id}/{leagueId}", // URL with parameters
    new { controller = "Organization", action = "ViewDetails" } // Parameter defaults
    );
            //Organize For Leaue organization

                routes.MapRoute(
    "AddOrganization", // Route name
    "league/organize/new/add", // URL with parameters
    new { controller = "Organization", action = "AddOrganize" } // Parameter defaults
    );

            //Organization Chart
                routes.MapRoute(
    "OrganizationChart", // Route name
    "league/organization/chart/{id}/{leagueId}", // URL with parameters
    new { controller = "Organization", action = "OrgChart" } // Parameter defaults
    );

            //Organization 
                routes.MapRoute(
    "OrganizationNew", // Route name
    "league/organization/new/{leagueId}", // URL with parameters
    new { controller = "Organization", action = "AddOrganization" } // Parameter defaults
    );
                routes.MapRoute(
    "LeagueAllOrganization", // Route name
    "league/organization/view/all", // URL with parameters
    new { controller = "Organization", action = "ViewAllOrganization" } // Parameter defaults
    );



            #endregion

            #region Dues Routes
            routes.MapRoute(
"DuesItemNew", // Route name
"dues/new/{type}/{id}/{duesId}", // URL with parameters
new { controller = "Dues", action = "GenerateNewDuesItem" }
);
            routes.MapRoute(
            "DuesMemberItemEdit", // Route name
"dues/member/edit/{duesItemId}/{duesManagementId}/{memberId}", // URL with parameters
new { controller = "Dues", action = "EditMemberDuesItem" },
new { duesItemId = @"\d+" }// Parameter defaults
);
            routes.MapRoute(
  "DuesMemberView", // Route name
"dues/member/{leagueId}/{memberId}", // URL with parameters
new { controller = "Dues", action = "DuesForMember" }// Parameter defaults
);

            routes.MapRoute(
"DuesItemEdit", // Route name
"dues/collection/edit/{duesItemId}/{duesManagementId}", // URL with parameters
new { controller = "Dues", action = "EditDuesItem" },
new { duesItemId = @"\d+" }// Parameter defaults
);
            routes.MapRoute(
"DuesItemReport", // Route name
"dues/report/item", // URL with parameters
new { controller = "Dues", action = "DuesItemReport" }
);

            routes.MapRoute(
"DuesItem", // Route name
"dues/collection/{duesItemId}/{duesManagementId}", // URL with parameters
new { controller = "Dues", action = "DuesItem" },
new { duesItemId = @"\d+" }// Parameter defaults
);
            routes.MapRoute(
"DuesClassficationNew", // Route name
"dues/classification/new/{type}/{id}/{duesManagementId}", // URL with parameters
new { controller = "Dues", action = "DuesClassificationNew" }
);
            routes.MapRoute(
"DuesClassficationDelete", // Route name
"dues/classification/delete/{type}/{id}/{duesManagementId}/{classId}", // URL with parameters
new { controller = "Dues", action = "DuesClassificationDelete" }
);
            routes.MapRoute(
"DuesClassficationEdit", // Route name
"dues/classification/edit/{type}/{id}/{duesManagementId}/{classId}", // URL with parameters
new { controller = "Dues", action = "DuesClassificationEdit" }
);
            routes.MapRoute(
"DuesClassfication", // Route name
"dues/classification/{type}/{id}/{duesManagementId}", // URL with parameters
new { controller = "Dues", action = "DuesClassification" }
);

            routes.MapRoute(
"DuesReceipt", // Route name
"dues/receipt/{type}/{invoiceId}", // URL with parameters
new { controller = "Dues", action = "DuesReceipt" } // Parameter defaults
);
            routes.MapRoute(
"DuesSettings", // Route name
"dues/settings/{type}/{id}/{duesId}", // URL with parameters
new { controller = "Dues", action = "DuesSettings" } // Parameter defaults
);
            routes.MapRoute(
"DuesManagementByMember", // Route name
"dues/members/{type}/{id}", // URL with parameters
new { controller = "Dues", action = "DuesManagementByMember" } // Parameter defaults
);
            routes.MapRoute(
"DuesManagement", // Route name
"dues/{type}/{id}/{duesId}", // URL with parameters
new { controller = "Dues", action = "DuesManagement", duesId = UrlParameter.Optional } // Parameter defaults
);
            #endregion

            #region Messages

            routes.MapRoute(
"MessagesNew", // Route name
"Messages/New/{type}/{id}", // URL with parameters
new { controller = "Message", action = "MessageNew" } // Parameter defaults
);
            routes.MapRoute(
"TextMessagesNew", // Route name
"textmessages/new/{type}/{id}", // URL with parameters
new { controller = "Message", action = "TextMessageNew" } // Parameter defaults
);
            routes.MapRoute(
"MessagesView", // Route name
"Messages/View/{groupId}", // URL with parameters
new { controller = "Message", action = "MessageView" } // Parameter defaults
);

            routes.MapRoute(
"Messages", // Route name
"Messages/{type}/{idOfGroup}", // URL with parameters
new { controller = "Message", action = "MessageHome" } // Parameter defaults
);



            #endregion

            #region Calendar Routes
            routes.MapRoute(
"BirthdayEvent", // Route name
"calendar/birthday/{id}/{name}", // URL with parameters
new { controller = "Calendar", action = "ViewBirthday" } // Parameter defaults
);
            routes.MapRoute(
"StartedSkatingEvent", // Route name
"calendar/started-skating/{id}/{name}", // URL with parameters
new { controller = "Calendar", action = "ViewStartedSkating" } // Parameter defaults
);
            routes.MapRoute(
"EventCheckInSmall", // Route name
"Calendar/event/checkin-s/{type}/{calendarId}/{eventId}", // URL with parameters
new { controller = "Calendar", action = "CheckInSmall" } // Parameter defaults
);
            routes.MapRoute(
"EventCheckInLarge", // Route name
"Calendar/event/checkin-l/{type}/{calendarId}/{eventId}", // URL with parameters
new { controller = "Calendar", action = "CheckInLarge" } // Parameter defaults
);
            routes.MapRoute(
"CalendarView", // Route name
"Calendar/view/{type}/{id}", // URL with parameters
new { controller = "Calendar", action = "CalendarView" } // Parameter defaults
);

            routes.MapRoute(
"CalendarNewEvent", // Route name
"Calendar/new/{type}/{id}/{check}", // URL with parameters
new { controller = "Calendar", action = "NewEvent", check = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
"CalendarDeleteEventReoccur", // Route name
"Calendar/event/delete/reoccur/{type}/{calId}/{eventId}", // URL with parameters
new { controller = "Calendar", action = "DeleteEventReoccurring" } // Parameter defaults
);
            routes.MapRoute(
"CalendarDeleteEvent", // Route name
"Calendar/event/delete/{type}/{calId}/{eventId}", // URL with parameters
new { controller = "Calendar", action = "DeleteEvent" } // Parameter defaults
);
            routes.MapRoute(
"CalendarEditReoccurringEvent", // Route name
"Calendar/event/edit/reoccurring/{type}/{calId}/{reoccuringEventId}", // URL with parameters
new { controller = "Calendar", action = "EditReoccurringEvent" } // Parameter defaults
);
            routes.MapRoute(
"CalendarEditEvent", // Route name
"Calendar/event/edit/{type}/{calId}/{eventId}", // URL with parameters
new { controller = "Calendar", action = "EditEvent" } // Parameter defaults
);



            routes.MapRoute(
"CalendarEvent", // Route name
"Calendar/event/{type}/{calId}/{eventId}", // URL with parameters
new { controller = "Calendar", action = "ViewEvent" } // Parameter defaults
);
            routes.MapRoute(
"CalendarSettings", // Route name
"Calendar/settings/{type}/{calId}", // URL with parameters
new { controller = "Calendar", action = "CalendarSettings" } // Parameter defaults
);
            routes.MapRoute(
"CalendarImport", // Route name
"Calendar/import/{type}/{calId}", // URL with parameters
new { controller = "Calendar", action = "CalendarImport" } // Parameter defaults
);
            routes.MapRoute(
"CalendarEventType", // Route name
"Calendar/event-type/new/{type}/{calId}", // URL with parameters
new { controller = "Calendar", action = "CalendarNewEventType" } // Parameter defaults
);
            routes.MapRoute(
"CalendarEventTypeEdit", // Route name
"Calendar/event-type/edit/{type}/{calId}/{eventTypeId}", // URL with parameters
new { controller = "Calendar", action = "CalendarEditEventType" } // Parameter defaults
);

            routes.MapRoute(
"CalendarReports", // Route name
"Calendar/reports/{type}/{id}", // URL with parameters
new { controller = "Calendar", action = "CalendarReport" } // Parameter defaults
);

            routes.MapRoute(
"CalendarPosts", // Route name
"Calendar/{type}/{id}/{year}/{month}", // URL with parameters
new { controller = "Calendar", action = "CalendarList", year = UrlParameter.Optional, month = UrlParameter.Optional } // Parameter defaults
);


            #endregion

            #region Location Routes
            routes.MapRoute(
"LocationNew", // Route name
"Location/new/{ownerType}/{redirectto}/{id}", // URL with parameters
new { controller = "Location", action = "NewLocation" } // Parameter defaults
);
            routes.MapRoute(
"LocationAll", // Route name
"location/all", // URL with parameters
new { controller = "Location", action = "AllLocations" } // Parameter defaults
);
            #endregion

            #region Forum Routes
            routes.MapRoute(
"ForumSettings", // Route name
"forum/settings/{forumId}/{groupId}", // URL with parameters
new { controller = "Forum", action = "ForumSettings", groupId = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
"ForumPosts", // Route name
"Forum/Posts/{type}/{id}/{groupid}/{categoryId}", // URL with parameters
new { controller = "Forum", action = "Posts", id = UrlParameter.Optional, groupid = UrlParameter.Optional, categoryId = UrlParameter.Optional } // Parameter defaults
);
            routes.MapRoute(
"ForumPostsRemoved", // Route name
"Forum/PostsRemoved/{type}/{id}/{groupid}/{categoryId}", // URL with parameters
new { controller = "Forum", action = "DeletedPosts", groupid = UrlParameter.Optional, categoryId = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
"ForumNewPost", // Route name
"Forum/new/{type}/{id}/{groupId}", // URL with parameters
new { controller = "Forum", action = "NewPost", groupId = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
"ForumViewPost", // Route name
"forum/post/view/{forumId}/{topicId}/{title}", // URL with parameters
new { controller = "Forum", action = "Post", title = UrlParameter.Optional } // Parameter defaults
);
            routes.MapRoute(
"ForumEditPost", // Route name
"forum/post/edit/{forumId}/{messageId}", // URL with parameters
new { controller = "Forum", action = "EditPost" } // Parameter defaults
);
            routes.MapRoute(
"ForumQuotePost", // Route name
"forum/post/quote/{forumId}/{topicId}/{messageId}", // URL with parameters
new { controller = "Forum", action = "QuotePost" } // Parameter defaults
);
            routes.MapRoute(
"ForumMovePost", // Route name
"forum/post/move/{forumId}/{messageId}", // URL with parameters
new { controller = "Forum", action = "MovePost" } // Parameter defaults
);
            routes.MapRoute(
"ForumReplyPost", // Route name
"forum/post/reply/{forumId}/{topicId}", // URL with parameters
new { controller = "Forum", action = "ReplyMessage" } // Parameter defaults
);
            routes.MapRoute(
"ForumMessageLike", // Route name
"forum/message/like", // URL with parameters
new { controller = "Forum", action = "GetLikeCount" } // Parameter defaults
);
            routes.MapRoute(
"ForumMessageAgree", // Route name
"forum/message/agree", // URL with parameters
new { controller = "Forum", action = "GetIAgreeCount" } // Parameter defaults
);

            
            #endregion

            #region Voting
            routes.MapRoute(
"CreatePoll", // Route name
"poll/add/{leagueId}", // URL with parameters
new { controller = "Vote", action = "CreatePoll" } // Parameter defaults
);
            routes.MapRoute(
"PollSettings", // Route name
"poll/settings/{leagueId}", // URL with parameters
new { controller = "Vote", action = "PollSettings" } // Parameter defaults
);
            routes.MapRoute(
"PollToVote", // Route name
"poll/vote/{leagueId}/{pollid}", // URL with parameters
new { controller = "Vote", action = "PollToVote" } // Parameter defaults
);
            routes.MapRoute(
"PollView2", // Route name
"poll/viewv2/{leagueId}/{pollid}", // URL with parameters
new { controller = "Vote", action = "PollViewV2" } // Parameter defaults
);
            routes.MapRoute(
"PollEditAdmin2", // Route name
"poll/edit/{leagueId}/{pollid}", // URL with parameters
new { controller = "Vote", action = "PollEditAdmin" } // Parameter defaults
);
            routes.MapRoute(
"PollToVote2", // Route name
"poll/votev2/{leagueId}/{pollid}", // URL with parameters
new { controller = "Vote", action = "PollToVoteV2" } // Parameter defaults
);
            routes.MapRoute(
"PollViewAdmin", // Route name
"poll/view/{leagueId}/{pollid}", // URL with parameters
new { controller = "Vote", action = "PollViewAdmin" } // Parameter defaults
);
            routes.MapRoute(
"Polls", // Route name
"poll/{leagueId}", // URL with parameters
new { controller = "Vote", action = "Polls" } // Parameter defaults
);
            #endregion

            #region Subscription

            routes.MapRoute(
            "SubscriptionList", // Route name
            "subscriptions/list/add", // URL with parameters
            new { controller = "Subscriptions", action = "AddNewList" } // Parameter defaults
            );

            routes.MapRoute(
            "SubscriptionListEdit", // Route name
            "subscriptions/list/edit/{listId}", // URL with parameters
            new { controller = "Subscriptions", action = "SubscriptionEdit" } // Parameter defaults
            );

            routes.MapRoute(
           "SubscriptionRemove", // Route name
           "subscriptions/list/remove/{listId}", // URL with parameters
           new { controller = "Subscriptions", action = "SubscriptionRemove" } // Parameter defaults
           );

            routes.MapRoute(
            "SubscriptionListAll", // Route name
            "subscriptions/lists", // URL with parameters
            new { controller = "Subscriptions", action = "ViewAllSubsLists" } // Parameter defaults
            );

            routes.MapRoute(
            "SubscriptionListDetails", // Route name
            "subscriptions/view/{listId}/{listName}", // URL with parameters
            new { controller = "Subscriptions", action = "SubscriptionListDetails" } // Parameter defaults
            );

            routes.MapRoute(
           "SubscriberAdd", // Route name
           "subscriptions/subscriber/add/{listId}/{listName}", // URL with parameters
           new { controller = "Subscriptions", action = "SubscriberAdd" } // Parameter defaults
           );

            routes.MapRoute(
          "SubscriberEdit", // Route name
          "subscriptions/subscriber/edit/{listId}/{listName}/{subscriberId}", // URL with parameters
          new { controller = "Subscriptions", action = "SubscriberEdit" } // Parameter defaults
          );

            routes.MapRoute(
          "SubscriberView", // Route name
          "subscriptions/subscriber/view/{listId}/{listName}/{subscriberId}", // URL with parameters
          new { controller = "Subscriptions", action = "SubscriberView" } // Parameter defaults
          );

            routes.MapRoute(
          "SubscriberRemove", // Route name
          "subscriptions/subscriber/remove/{listId}/{listName}/{subscriberId}", // URL with parameters
          new { controller = "Subscriptions", action = "SubscriberRemove" } // Parameter defaults
          );

            routes.MapRoute(
         "BlastEmail", // Route name
         "subscriptions/email/blast/{listId}/{listName}", // URL with parameters
         new { controller = "Subscriptions", action = "EmailBlast" } // Parameter defaults
         );
            #endregion Subscription


            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            //ValidateEmail(string email, string code)
        }
        protected void Application_BeginRequest()
        {

            if (Request.IsLocal)
            {

                MiniProfiler.Start();
            }
            else
            {
                if (!Context.Request.IsSecureConnection)
                    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));

            }

        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }

        protected void Application_Start()
        {
            try
            {
                MiniProfilerEF6.Initialize();
                ErrorInitializer.Initialize();
                AreaRegistration.RegisterAllAreas();

                RegisterGlobalFilters(GlobalFilters.Filters);
                RegisterRoutes(RouteTable.Routes);
                

                SiteSingleton.Instance.IsProduction = Convert.ToBoolean(ConfigurationManager.AppSettings["IsProduction"].ToString());
                SiteSingleton.Instance.IsPayPalLive = (PaymentMode)Enum.Parse(typeof(PaymentMode), ConfigurationManager.AppSettings["IsPayPalLive"].ToString());

                BundleConfig.RegisterBundles(BundleTable.Bundles);

                log4net.Config.XmlConfigurator.Configure();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, GetType());
            }
        }
        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();
            ErrorDatabaseManager.AddException(exc, GetType());
            Server.ClearError();
            Response.Redirect(ServerConfig.WEBSITE_INTERNAL_DEFAULT_LOCATION + "?u=" + SiteMessagesEnum.sww);
        }
    }
}