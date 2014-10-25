using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Scoreboard.Library.Network
{
    public class WebClientRDN : WebClient
    {
        public WebClientRDN()
        {

        }
        public int TimeOut { get; set; }
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            if (TimeOut > 0)
                w.Timeout = TimeOut;
            else
                w.Timeout = 20 * 60 * 1000;
            return w;
        }
       
    }
}
