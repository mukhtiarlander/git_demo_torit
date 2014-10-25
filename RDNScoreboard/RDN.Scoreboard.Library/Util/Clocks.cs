using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Scoreboard.Library.Util
{
    public class Clocks
    {
        public static Regex CLOCK_CHECK = new Regex(@"\d{0,2}:\d{2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex _number = new Regex(@"\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex _minutesGet = new Regex(@"\d{1,2}:", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static Regex _secondsGet = new Regex(@":\d{2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// converts a time display of 55:44 to milliseconds
        /// </summary>
        /// <param name="timeDisplayed"></param>
        /// <returns></returns>
        public static long convertTimeDisplayToSeconds(string timeDisplayed)
        {
            long milli = 0;
            if (_minutesGet.IsMatch(timeDisplayed))
            {
                int mins = Convert.ToInt32(_number.Match(_minutesGet.Match(timeDisplayed).Value).Value);
                milli = mins * 60 ;

            }
            if (_secondsGet.IsMatch(timeDisplayed))
            {
                int sec = Convert.ToInt32( _number.Match(_secondsGet.Match(timeDisplayed).Value).Value);
                milli += sec ;
            }
            return milli;
        }
    }
}
