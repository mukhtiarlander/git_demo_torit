using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDNationLibrary.Static
{
    public  class Config
    {
        public static readonly string ORAGANIZATION = "RDNation.com";
        public static readonly string SCOREBOARD_NAME = "Thor's Hammer";
        public static readonly string SCOREBOARD_NAME_NOSPACES = "Thors_Hammer";
        public static readonly string SCOREBOARD_VERSION_NUMBER = "1.0.0";
        
        public static string SAVE_GAMES_FILE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\RDNationGame.xml";
        public static string SAVE_POLICY_FILE = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\RDNationPolicy.xml";
        public static string SAVE_APPLICATION_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\";
        public static string SAVE_LOGOS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Logos\\";
        public static string SAVE_ADVERTS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Adverts\\";
        public static string SAVE_ERRORS_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + ORAGANIZATION + "\\" + SCOREBOARD_NAME_NOSPACES + "\\Errors\\";

        

        //periods
        public static int WFTDA_DEFAULT_PERIODS = 2;
        public static int MADE_DEFAULT_PERIODS = 4;

        //timeouts
        public static int WFTDA_DEFAULT_TIMEOUTS_PERIOD = 2;
        public static int MADE_DEFAULT_TIMEOUTS_PERIOD = 2;


    }
}
