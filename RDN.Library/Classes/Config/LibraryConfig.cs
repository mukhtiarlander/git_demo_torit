using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RDN.Library.Classes.Config
{
    public static class LibraryConfig
    {
        public static string PublicSite
        {
            get
            {
                return ConfigurationManager.AppSettings["PublicSite1"];
            }
        }


    }
}
