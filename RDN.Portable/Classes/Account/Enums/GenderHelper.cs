using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Account.Enums
{
    public class GenderHelper
    {
        public static List<string> GenderTypes
        {
            get
            {
                return Enum.GetValues(typeof(GenderEnum)).Cast<GenderEnum>().Select(v => v.ToString().Replace("_", " ")).OrderBy(x => x).ToList();
            }
        }
    }
    public class PositionHelper
    {
        public static List<string> PositionTypes
        {
            get
            {
                return Enum.GetValues(typeof(DefaultPositionEnum)).Cast<DefaultPositionEnum>().Select(v => v.ToString().Replace("_", " ")).OrderBy(x => x).ToList();
            }
        }
    }
}
