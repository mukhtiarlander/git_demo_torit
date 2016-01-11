using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scoreboard.Library.Models;
using System.Net;
using RDN.Utilities.Config;
using System.IO;
using Scoreboard.Library.Network;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using RDN.Portable.Classes.Team;
using System.Runtime;
namespace Scoreboard.Library.ViewModel
{
    public class LogoViewModel
    {
        static LogoViewModel instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LogoViewModel()
        { }

        public static LogoViewModel Instance
        {
            get
            {
                // DoubleLock
                if (instance == null)
                {
                    lock (m_lock)
                    {
                        if (instance == null)
                        {
                            instance = new LogoViewModel();
                        }
                    }
                }
                return instance;
            }
        }
        //Helper for Thread Safety
        private static object m_lock = new object();

        private ObservableCollection<TeamLogo> _directoryLogos;
        public ObservableCollection<TeamLogo> DirectoryLogos
        {
            get
            {
                if (_directoryLogos == null)
                {
                    _directoryLogos = new ObservableCollection<TeamLogo>();
                }
                return _directoryLogos;
            }
        }
        private ObservableCollection<TeamLogo> _weblogos;
        public ObservableCollection<TeamLogo> WebLogos
        {
            get
            {
                if (_weblogos == null)
                {
                    _weblogos = new ObservableCollection<TeamLogo>();
                }
                return _weblogos;
            }
        }
        public void LoadLogos(bool loadLogosFromInternet)
        {

            GetLogosForAllTeams(loadLogosFromInternet);

        }

        private void GetLogosFromDirectory()
        {try{
            instance.DirectoryLogos.Clear();
            DirectoryInfo dir = new DirectoryInfo(ScoreboardConfig.SAVE_LOGOS_FOLDER);
            if (dir.Exists)
            {
                var files = dir.GetFiles();
                int fileCount = files.Count();
                for (int i = 0; i < fileCount; i++)
                {
                    try
                    {
                        if (files[i].ToString().Contains(".jpg") || files[i].ToString().Contains(".png"))
                        {
                            TeamLogo logo = new TeamLogo();
                            logo.SaveLocation = ScoreboardConfig.SAVE_LOGOS_FOLDER + files[i].ToString();
                            logo.ImageUrl = ScoreboardConfig.SAVE_LOGOS_FOLDER + files[i].ToString();
                            instance.DirectoryLogos.Add(logo);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorViewModel.Save(exception, exception.GetType());
                    }
                }
            }
        }
        catch (Exception exception)
        {
            ErrorViewModel.Save(exception, exception.GetType());
        }
        }

        /// <summary>
        /// gets all the logos both in the directory and on the internet for the teams.
        /// </summary>
        /// <returns></returns>
        private void GetLogosForAllTeams(bool loadLogosFromNet)
        {
            try
            {

                GetLogosFromDirectory();
                if (loadLogosFromNet)
                    LoadLogosFromInternet();
            }
            catch (Exception exception)
            {
                ErrorViewModel.Save(exception, exception.GetType());
            }
        }

        private void LoadLogosFromInternet()
        {
            if (Internet.CheckConnection())
            {
                instance.WebLogos.Clear();
                try
                {
                    WebClient client = new WebClient();
                    string json = client.DownloadString(ScoreboardConfig.ALL_LOGOS_JSON);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    LogoCollection logosJson = serializer.Deserialize<LogoCollection>(json);
                    foreach (var log in logosJson.logos)
                    {
                        string fileName = System.IO.Path.GetFileName(log.ImageUrl);
                        //if the file was already downloaded earlier.
                        if (DirectoryLogos.Where(x => x.SaveLocation == (ScoreboardConfig.SAVE_LOGOS_FOLDER + fileName)).FirstOrDefault() == null)
                        instance.    WebLogos.Add(log);
                        else if (!String.IsNullOrEmpty(fileName))
                        {
                            try
                            {
                                var logoTemp = DirectoryLogos.Where(x => x.ImageUrl != null && x.ImageUrl.Contains(fileName)).FirstOrDefault();
                                if (logoTemp != null)
                                {
                                    logoTemp.TeamLogoId = log.TeamLogoId;
                                    logoTemp.TeamName = log.TeamName;
                                }
                            }
                            catch (Exception exception)
                            {
                                ErrorViewModel.Save(exception, exception.GetType(), additionalInformation: fileName);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ErrorViewModel.Save(exception, exception.GetType());
                }
            }
        }

    }
}
