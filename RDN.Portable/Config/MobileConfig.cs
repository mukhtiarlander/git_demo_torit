using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Config
{
    public class MobileConfig
    {
        public static readonly string MOBILE_VERSION_NUMBER_IPHONE = "1.0";
        public static readonly string MOBILE_VERSION_NUMBER_WP8 = "2.1.1";
        //private static readonly string APIURL = "http://192.168.1.101:16106";
        private static readonly string APIURL = "https://api.rdnation.com";

        public static readonly string LOGIN_URL_DEBUG_ANDROID = "http://10.0.2.2:52509/authenticate/login";
        public static readonly string LOGIN_URL = APIURL + "/authenticate/login";

        public static readonly string SIGNUP_URL_DEBUG_ANDROID = "http://10.0.2.2:52509/authenticate/signup";
        public static readonly string SIGNUP_URL = APIURL + "/authenticate/signup";
        public static readonly string ACCOUNT_SETTINGS_URL = APIURL + "/account/accountsettings";

        public static readonly string GET_SKATERS_BY_APLHA_URL_DEBUG_ANDROID = "http://10.0.2.2:52509/skater/getallskatersmobile?";
        public static readonly string GET_SKATERS_BY_APLHA_URL = APIURL + "/skater/getallskatersmobile?";
        public static readonly string GET_SKATER_BY_MEMBERID_URL = APIURL + "/skater/getskatermobile?";
        public static readonly string GET_SKATERS_BY_LEAGUEID_URL = APIURL + "/skater/getallskatersforleague?";

        public static readonly string GET_LEAGUES_BY_APLHA_URL = APIURL + "/league/getallleaguesmobile?";
        public static readonly string GET_LEAGUE_BY_LEAGUEID_URL = APIURL + "/league/getleaguemobile?";
        public static readonly string LEAGUE_MEMBERS_URL = APIURL + "/league/viewmemberroster?";
        public static readonly string LEAGUE_MEMBERS_BASIC_URL = APIURL + "/league/members?";
        public static readonly string LEAGUE_GROUPS_URL = APIURL + "/league/groups?";
        public static readonly string GET_LEAGUE_MEMBERS_BY_LEAGUEID_URL = APIURL + "/skater/getskatermobile?";
        public static readonly string GET_LEAGUE_EVENTS_BY_LEAGUEID_URL = APIURL + "/calendar/calendarevents?";

        public static readonly string GET_CALENDAR_EVENTS_URL = APIURL + "/calendar/calendareventsall?";
        public static readonly string SEARCH_CALENDAR_EVENTS_URL = APIURL + "/calendar/searcheventsall?";
        public static readonly string SEARCH_CALENDAR_EVENTS_BY_LL_URL = APIURL + "/calendar/searcheventsallbyll?";


        public static readonly string GET_CURRENT_GAMES_MOBILE_URL = APIURL + "/livegame/currentgamesmobile";
        public static readonly string GET_PAST_GAMES_MOBILE_URL = APIURL + "/livegame/pastgames?";

        public static readonly string SEND_NOTIFICATION_CREATE = APIURL + "/account/createmobilenotificationtoken";
        public static readonly string SEND_NOTIFICATION_CREATE_DEBUG_ANDROID = "http://10.0.2.2:52509/account/createmobilenotificationtoken";

        public static readonly string GET_SHOP_ITEMS_URL = APIURL + "/shops/getallmerch?";
        public static readonly string SEARCH_SHOP_ITEMS_URL = APIURL + "/shops/searchallmerch?";

        public static readonly string LEAGUE_FORUM_TOPICS_URL = APIURL + "/forum/posts?";
        public static readonly string LEAGUE_FORUM_TOPIC_URL = APIURL + "/forum/topic?";
        public static readonly string LEAGUE_FORUM_ADD_TOPIC_URL = APIURL + "/forum/addtopic";
        public static readonly string LEAGUE_FORUM_REPLY_TOPIC_URL = APIURL + "/forum/replytopic";
        public static readonly string LEAGUE_FORUM_WATCH_TOPIC_URL = APIURL + "/forum/watchtopic?";
        public static readonly string LEAGUE_EDIT_URL = APIURL + "/league/editleague?";
        public static readonly string LEAGUE_EDIT_SAVE_URL = APIURL + "/league/saveleague?";
        public static readonly string LEAGUE_COLORS_URL = APIURL + "/calendar/leaguecolors?";
        public static readonly string LEAGUE_LOCATIONS_URL = APIURL + "/league/getlocations?";
        public static readonly string LEAGUE_INITIAL_LOAD_URL = APIURL + "/league/loadinitial?";

        public static readonly string LEAGUE_POLLS_GET_URL = APIURL + "/vote/polls?";
        public static readonly string LEAGUE_POLLS_ADD_NEW_URL = APIURL + "/vote/createpolladd?";
        public static readonly string LEAGUE_POLLS_GET_POLL_URL = APIURL + "/vote/polltovote";
        public static readonly string LEAGUE_POLLS_VOTE_POLL_URL = APIURL + "/vote/voteonpoll";
        public static readonly string LEAGUE_POLLS_UPDATE_URL = APIURL + "/vote/updatepoll";
        public static readonly string LEAGUE_POLLS_DELETE_URL = APIURL + "/vote/deletepoll";
        public static readonly string LEAGUE_POLLS_CLOSE_URL = APIURL + "/vote/closepoll";

        public static readonly string MEMBER_EDIT_GET_URL = APIURL + "/member/editmember?";
        public static readonly string MEMBER_EDIT_SAVE_GET_URL = APIURL + "/member/savemember?";
        public static readonly string LEAGUE_FORUM_CATEGORIES_URL = APIURL + "/forum/categories?";
        public static readonly string MEMBER_MESSAGES_URL = APIURL + "/message/getmessages?";
        public static readonly string MEMBER_MESSAGES_VIEW_URL = APIURL + "/message/messageview?";
        public static readonly string MEMBER_MESSAGES_REPLY_URL = APIURL + "/message/postmessage?";
        public static readonly string MEMBER_MESSAGES_NEW_URL = APIURL + "/message/createnewmessage?";
        public static readonly string MEMBER_MESSAGES_NEW_TEXT_URL = APIURL + "/message/createnewtextmessage?";
        public static readonly string ORAGANIZATION = "RDNation.com";

        public static readonly string LEAGUE_DUES_MANAGE_URL = APIURL + "/dues/duesmanagement";
        public static readonly string LEAGUE_DUES_ITEM_URL = APIURL + "/dues/duesitem";
        public static readonly string LEAGUE_DUES_ITEM_REMOVE_URL = APIURL + "/dues/removeduespayment";
        public static readonly string LEAGUE_DUES_SEND_EMAIL_REMINDER_WITHSTANDING_URL = APIURL + "/dues/sendemailreminderwithstanding";
        public static readonly string LEAGUE_DUES_SEND_EMAIL_REMINDER_ALL_URL = APIURL + "/dues/sendemailreminderall";
        public static readonly string LEAGUE_DUES_SEND_EMAIL_REMINDER_URL = APIURL + "/dues/sendemailreminder";
        public static readonly string LEAGUE_DUES_PAY_URL = APIURL + "/dues/payduesamount";
        public static readonly string LEAGUE_DUES_SET_AMOUNT_URL = APIURL + "/dues/setduesamount";
        public static readonly string LEAGUE_DUES_SAVE_SETTINGS_URL = APIURL + "/dues/saveduessettings";
        public static readonly string LEAGUE_DUES_WAIVE_URL = APIURL + "/dues/waiveduesamount";
        public static readonly string LEAGUE_DUES_REMOVE_WAIVE_URL = APIURL + "/dues/removedueswaived";
        public static readonly string LEAGUE_DUES_EDIT_ITEM_URL = APIURL + "/dues/editduesitem";
        public static readonly string LEAGUE_DUES_DELETE_ITEM_URL = APIURL + "/dues/deleteduesitem";
        public static readonly string LEAGUE_DUES_FOR_MEMBER_URL = APIURL + "/dues/duesformember";
        public static readonly string LEAGUE_DUES_GET_SETTINGS_URL = APIURL + "/dues/duessettings";
        public static readonly string LEAGUE_DUES_GENERATE_NEW_URL = APIURL + "/dues/generatenewduesitem";
        public static readonly string LEAGUE_DUES_PAY_DUES_PERSONALLY_URL = APIURL + "/dues/payduesonlinepaypal";
        public static readonly string LEAGUE_DUES_EDIT_MEMBER_DUES_URL = APIURL + "/dues/editmemberduesitem";

        public static readonly string LEAGUE_CALENDAR_LIST_URL = APIURL + "/calendar/calendarlist";
        public static readonly string LEAGUE_CALENDAR_CHECK_SELF_IN_URL = APIURL + "/calendar/checkselfintoevent";
        public static readonly string LEAGUE_CALENDAR_SET_AVAILABILITY_URL = APIURL + "/calendar/setavailabilityforevent";
        public static readonly string LEAGUE_CALENDAR_VIEW_EVENT_URL = APIURL + "/calendar/viewevent";
        public static readonly string LEAGUE_CALENDAR_CHECK_MEMBER_IN_URL = APIURL + "/calendar/checkmemberintoevent";
        public static readonly string LEAGUE_CALENDAR_CHECK_MEMBER_IN_REMOVE_URL = APIURL + "/calendar/checkinmemberremove";
        public static readonly string LEAGUE_CALENDAR_GET_EVENT_TYPES_URL = APIURL + "/calendar/geteventtypes";
        public static readonly string LEAGUE_CALENDAR_CREATE_NEW_EVENT_URL = APIURL + "/calendar/newevent";

        public static readonly string UTILITIES_SEND_FEEDBACK = APIURL + "/feedback/savefeedback";
    
    }
}
