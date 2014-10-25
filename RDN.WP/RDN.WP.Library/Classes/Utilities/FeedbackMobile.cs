using RDN.Portable.Classes.Utilities;
using RDN.Portable.Config;
using RDN.Portable.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.WP.Library.Classes.Utilities
{
    public class FeedbackMobile
    {
        public static Feedback SendFeedback(Feedback feedback)
        {
            var response = Network.SendPackage(Network.ConvertObjectToStream(feedback), MobileConfig.UTILITIES_SEND_FEEDBACK);
            var stream = response.GetResponseStream();
            StreamReader read = new StreamReader(stream);
            string json = read.ReadToEnd();
            return Json.DeserializeObject<Feedback>(json);
        }
    }
}
