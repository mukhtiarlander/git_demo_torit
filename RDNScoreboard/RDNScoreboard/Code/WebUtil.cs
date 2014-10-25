using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using RDN.Utilities.Config;
using Scoreboard.Library.Network;
using System.Threading.Tasks;
using Scoreboard.Library.ViewModel;
using RDN.Utilities.Error;
using Scoreboard.Library.Models;
using RDN.Utilities.Util;
using RDN.Portable.Classes.Team;


namespace RDNScoreboard.Code
{
    public class WebUtil
    {



        /// <summary>
        /// downloads the logo of the team if its not already on users computer
        /// </summary>
        /// <param name="urlOfLogo"></param>
        /// <returns></returns>
        public static string getLogoOfTeam(TeamLogo logo, Guid teamId)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_LOGOS_FOLDER);
                if (!dir.Exists)
                    dir.Create();

                string fileLocation = logo.ImageUrl;
                string fileName = Path.GetFileName(fileLocation);

                FileInfo file = new FileInfo(ScoreboardConfig.SAVE_LOGOS_FOLDER + fileName);
                if (!file.Exists)
                {
                    WebClient client = new WebClient();
                    client.DownloadFile(logo.ImageUrl, ScoreboardConfig.SAVE_LOGOS_FOLDER + fileName);
                    waitOnDownload(client);
                }

                return ScoreboardConfig.SAVE_LOGOS_FOLDER + fileName;
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType(), null, null, null, Logger.Instance.getLoggedMessages());
            }
            return string.Empty;
        }
        /// <summary>
        /// if client is still downloading, we will wait for the file.
        /// </summary>
        /// <param name="client"></param>
        private static void waitOnDownload(WebClient client)
        {
            if (client.IsBusy)
            {
                Thread.Sleep(500);
                waitOnDownload(client);
            }
        }

    }
}
