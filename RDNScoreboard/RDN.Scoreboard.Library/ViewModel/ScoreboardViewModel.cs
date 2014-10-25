using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Network;
using RDN.Utilities.Config;


namespace Scoreboard.Library.ViewModel
{
    public class ScoreboardViewModel
    {
   
        /// <summary>
        /// sends the id of the nic card to the server to help keep stats on active installs.
        /// </summary>
        public static void sendActiveInstallIdToServer()
        {
            Task<bool>.Factory.StartNew(
                      () =>
                      {
                          if (!Scoreboard.Library.Network.Internet.CheckConnection())
                              return false;
                          WebClient client = new WebClient();
                          try
                          {
                              client.UploadStringAsync(new Uri(ScoreboardConfig.SCOREBOARD_MACHINE_ID_URL_NEW + "&mac=" + Client.GetMacAddress() + "&version=" + ScoreboardConfig.SCOREBOARD_VERSION_NUMBER), "");
                              return true;
                          }
                          catch
                          { }

                          return true;
                      });
        }



    }
}
