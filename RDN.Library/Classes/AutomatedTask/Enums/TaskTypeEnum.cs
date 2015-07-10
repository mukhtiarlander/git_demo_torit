using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.AutomatedTask.Enums
{
    public enum TaskTypeEnum
    {
        EmailThoseMembersThatDidntFillProfileOut = 1,
        EmailAdminToTellThemAutomationIsWorking = 2,
        ResendAllEmailsStuckWaitingToBeVerified = 3,
        EmailMembersThatHaveMessagesSittingInInbox = 4,
        EmailLeaguesWhichSubscriptionToRDNationIsAboutToOrHasExpired = 5,
        EmailMembersAboutNewForumContent = 6,
        EmailMerchantsAboutItemsExpiring = 7,
        ChargeMerchantsForItemsExpiring = 8,
        EmailMerchantsAboutStoreClosed = 9,
        EmailMerchantsAboutNoItemsOnStore = 10,
        UpdateCurrencyExchangeRates = 11,
        TextMessagesAreWorking = 12,
        SkaterAndLeagueOfDay = 13,
        ProcessRollinNewsMassPayments = 14,
        RinxterGamesTournamentsImport = 15,
        EmailAboutReviewingProductBought = 16,
        EmailIntroductoryEmails= 17,
        SkaterOfDay = 18,
    }
}
