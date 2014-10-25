using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace RDN.WP.Library.Classes.RSS
{
    public class SyndicationItemModel 
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string InitialImage { get; set; }
        public string MainImage { get; set; }
    }
}
