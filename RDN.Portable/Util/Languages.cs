using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Util
{
    public class LanguagesUtil
    {
        public static List<string> Languages = new List<string> { "English"
            //,            "Español" 
        };
        public static string ConvertToAbbreviation(string name)
        {
            switch (name)
            {
                case "English":
                    return "en-US";
                case "Español":
                    return "es-ES";
            }
            return "en-US";
        }
        public static string ConvertToName(string abbreviation)
        {
            switch (abbreviation)
            {
                case "en-US":
                    return "English";
                case "es-ES":
                    return "Español";
            }
            return "en-US";
        }

    }
}
