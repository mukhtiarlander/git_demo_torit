using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.ViewModel.Officials.Enums;


namespace Scoreboard.Library.ViewModel.Members.Officials
{
    public class OfficialsHelper
    {
        public static List<string> CertificationEnumTypes
        {
            get
            {
                return Enum.GetValues(typeof(CertificationLevelEnum)).Cast<CertificationLevelEnum>().Select(v => v.ToString().Replace("_", " ")).OrderBy(x => x).ToList();
            }
        }

        public  static List<string> NsoEnumTypes
        {
            get
            {
                return Enum.GetValues(typeof(NSOTypeEnum)).Cast<NSOTypeEnum>().Select(v => v.ToString().Replace("_", " ")).OrderBy(x => x).ToList();
            }
        }

        public static List<string> RefereeEnumTypes
        {
            get
            {
                return Enum.GetValues(typeof(RefereeTypeEnum)).Cast<RefereeTypeEnum>().Select(v => v.ToString().Replace("_", " ")).OrderBy(x => x).ToList();
            }
        }
    }
}
