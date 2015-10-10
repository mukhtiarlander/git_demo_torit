using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Classes.Account.Enums
{
    /// <summary>
    /// these are privacy settings to turn on and off settings per user.
    /// </summary>
    [Flags]
    public enum MemberPrivacySettingsEnum
    {
        Hide_Phone_Number_From_League = 1,
        Hide_Email_From_League = 2,
        Hide_DOB_From_League = 4,
        Hide_DOB_From_Public = 8,
        Do_You_Derby = 16,
        Hide_Address_From_League=32
    }
}
