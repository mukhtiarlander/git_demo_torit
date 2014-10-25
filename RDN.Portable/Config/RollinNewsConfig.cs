using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDN.Portable.Config
{
    public class RollinNewsConfig
    {
        public static int VERSION = 2;
        public static string DEFAULT_EMAIL = "info@rollinnews.com";
        public static string DEFAULT_ADMIN_EMAIL = "info@rollinnews.com";
        public static string DEFAULT_EMAIL_FROM_NAME = "RollinNews";
        public static string DEFAULT_MRX_EMAIL_ADMIN = "info@rollinnews.com";

        public static readonly string WEBSITE_DEFAULT_LOCATION = "http://rollinnews.com";
        public static readonly string DEFAULT_LOGIN_URL = "http://rollinnews.com/Account/login.aspx";

        public static Guid MERCHANT_ID = new Guid("7b0c3da2-b58a-4b1a-b9a2-92d253ce0100");

    }
}

