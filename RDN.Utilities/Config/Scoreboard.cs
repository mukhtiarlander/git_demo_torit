using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Utilities.Config
{
    public class ScoreboardConfig
    {
        /// <summary>
        /// Major.Minor.Revision
        /// </summary>
        public static readonly string SCOREBOARD_VERSION_NUMBER = "2.2.9";
        public static readonly string SCOREBOARD_NAME = string.Format("Thor's Hammer Scoreboard by RDNation.com {0}", SCOREBOARD_VERSION_NUMBER);

        public static readonly string SCOREBOARD_NAME_NOSPACES = "Thors_Hammer";

        public static readonly string ORAGANIZATION = "RDNation.com";

        public static int DEFAULT_SIZE_OF_RESIZED_IMAGE = 800;
        public static string SAVE_GAMES_FILE_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Saved Games\\";
        public static string SAVE_REPORTS_FILE_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Saved Reports\\";
        public static string SAVE_ROSTERS_FILE_LOCATION = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Saved Rosters\\";
        public static string SAVE_TEMP_GAMES_FILE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\RDNationTempGame.xml";
        public static string SAVE_GAMES_TEMP_ENCRYPTED_FILE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\RDNationGameTempEncrypted.xml";
        public static string SAVE_POLICY_FILE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\RDNationPolicy.xml";
        public static string SAVE_CRASHED_FILE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\ScoreBoardCrash.xml";
        public static string SAVE_APPLICATION_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\";
        public static string SAVE_BACKGROUND_IMAGES_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Images\\";
        public static string SAVE_FILES_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Files\\";
        public static string SAVE_LOGOS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Logos\\";
        public static string SAVE_SLIDESHOW_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\SlideShow\\";
        public static string SAVE_INTRODUCTIONS_SLIDESHOW_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\IntroSlideShow\\";
        public static string SAVE_ADVERTS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Adverts\\";
        public static string SAVE_ERRORS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Errors\\";
        public static string SAVE_FEEDBACK_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Feedback\\";
        public static string LOG_SCOREBOARD_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Logs\\";
        public static string SAVE_SKATERS_PICTURE_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Skaters\\";
        public static string SAVE_SERVER_FILES_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Server\\";
        public static string SAVE_SERVER_FILES_FOLDER_IMG = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Server\\img\\";
        public static string SAVE_SERVER_FILES_FOLDER_JS = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Server\\js\\";
        public static string SAVE_SERVER_FILES_FOLDER_CSS = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Server\\css\\";
        public static string SAVE_SERVER_FILES_FOLDER_IMG_WFT = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Server\\img\\wftdaoverlay\\";
        //public static string SAVE_SERVER_FILES_FOLDER_JSEXT = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Server\\js\\external\\";
        //public static string SAVE_SERVER_FILES_FOLDER_JSEXT_IMG = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Server\\js\\external\\Img\\";
       
        /// <summary>
        /// sends every 60 seconds.
        /// </summary>
        public static readonly int SEND_LIVE_GAMES_MILLISECOND_INTERVAL = 15000;

        public static string KEY_FOR_UPLOAD = "spoiledtechie_dot_com_rules";
        public static string KEY_FOR_DOWNLOAD = "spoiledtechie_dot_com_rules";

        private static string _UPLOAD_URL_HOST_DEBUG = "http://localhost:49534/";

        public static string UPLOAD_ERRORS_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "Scoreboard/uploadErrors?k=" + KEY_FOR_UPLOAD;
        public static string UPLOAD_LIVE_GAMES_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "Scoreboard/uploadLiveGames?k=" + KEY_FOR_UPLOAD;
        public static string IS_WEBSITE_ONLINE_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "Scoreboard/isonline";
        public static string FEEDBACK_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "Scoreboard/feedback?k=" + KEY_FOR_UPLOAD;
        public static string GET_LIVE_GAME_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "livegame/game/38c05bbd7118444990da6fd7a19e3caf";
        public static string SCOREBOARD_MACHINE_ID_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "Scoreboard/scoreboardLaunched?k=" + KEY_FOR_UPLOAD + "&mac=";
        public static string SCOREBOARD_UPLOAD_MEMBER_PICTURE_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "scoreboard/UploadMemberPictureFromGame?k=" + KEY_FOR_UPLOAD;

        public static string SCOREBOARD_UPLOAD_LOGO_URL_DEBUG = _UPLOAD_URL_HOST_DEBUG + "logos/uploadTeamLogo?k=" + KEY_FOR_UPLOAD;

        public static string ALL_LOGOS_JSON_DEBUG = _UPLOAD_URL_HOST_DEBUG + "logos/getalllogos";


        public static string UPLOAD_ERRORS_URL = "https://api.rdnation.com/error/submit";
        public static string UPLOAD_LIVE_GAMES_URL = "https://api.rdnation.com/Scoreboard/uploadLiveGames?k=" + KEY_FOR_UPLOAD;
        //public static string UPLOAD_LIVE_GAMES_URL = "http://localhost:49534/Scoreboard/uploadLiveGames?k=" + KEY_FOR_UPLOAD;
        public static string IS_WEBSITE_ONLINE_URL = "https://api.rdnation.com/Scoreboard/isonline";
        public static string SCOREBOARD_MACHINE_ID_URL = "https://api.rdnation.com/Scoreboard/scoreboardLaunched?k=" + KEY_FOR_UPLOAD;
        public static string SCOREBOARD_MACHINE_ID_URL_NEW = "https://api.rdnation.com/Scoreboard/scoreboardLaunchedNew?k=" + KEY_FOR_UPLOAD;
        public static string FEEDBACK_URL = "https://api.rdnation.com/Scoreboard/feedback?k=" + KEY_FOR_UPLOAD;
        public static string UPDATER_URL = "https://codingforcharity.org/rdnation/version/update.xml";
        public static string UPLOAD_LOGOS_LOCATION = "https://api.rdnation.com/logos/uploadTeamLogo?k=" + KEY_FOR_UPLOAD;
        public static string SCOREBOARD_UPLOAD_MEMBER_PICTURE_URL = "https://api.rdnation.com/scoreboard/UploadMemberPictureFromGame?k=" + KEY_FOR_UPLOAD;
        public static string SCOREBOARD_UPLOAD_LOGO_URL = "https://api.rdnation.com/logos/uploadTeamLogo?k=" + KEY_FOR_UPLOAD;
        public static string SCOREBOARD_UPLOAD_AND_PUBLISH_GAME = "https://league.rdnation.com/game/upload";


        //public static string LOGOS_ONLINE_LOCATION = "https://codingforcharity.org/rdnation/logos/";

        public static string ALL_LOGOS_JSON = "https://api.rdnation.com/Logos/GetAllLogos";



        public static string SCOREBOARD_WIKI_URL = "http://wiki.rdnation.com/Welcome_to_The_Wiki_For_Thors_Hammer";
        public static string SCOREBOARD_ONLINE_HELP_URL = "http://zebras.rdnation.com/yaf_topics34_Roller-Derby-Scoreboard-Discussion.aspx";
        /// <summary>
        /// url for team manager help.
        /// </summary>
        public static string SCOREBOARD_TEAM_MANAGER_WIKI_URL = "http://wiki.rdnation.com/Team_Manager";
        public static string SCOREBOARD_LOGO_MANAGER_WIKI_URL = "http://wiki.rdnation.com/Uploading_Logos";
        public static string REFEREE_MANAGER_WIKI_URL = "http://wiki.rdnation.com/Referee_Manager";
        public static string SCOREBOARD_SCOREBOARD_WIKI_URL = "http://wiki.rdnation.com/Choosing_a_Scoreboard";
        public static string SCOREBOARD_SLIDESHOW_MANAGER_WIKI_URL = "http://wiki.rdnation.com/SlideShow_Manager";
        public static string SCOREBOARD_STATS_COLLECTION_WIKI_URL = "http://wiki.rdnation.com/Stats_Collection";
        public static string SCOREBOARD_STATS_COLLECTION_SERVER_SETTINGS_WIKI_URL = "http://wiki.rdnation.com/Stats_Collection#Server_Settings";

        public static readonly string DEFAULT_GAME_NAME = "Scrimmage";

        //periods
        public static int WFTDA_DEFAULT_PERIODS = 2;
        public static int MADE_DEFAULT_PERIODS = 4;

        //timeouts
        public static int WFTDA_DEFAULT_TIMEOUTS_PERIOD = 2;
        public static int MADE_DEFAULT_TIMEOUTS_PERIOD = 2;



    }
}
