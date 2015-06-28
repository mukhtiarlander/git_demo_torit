using RDN.Library.Classes.Config;
using RDN.Library.Classes.Federation.Enums;
using RDN.Portable.Classes.Federation.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace RDN.Library.Classes.Imports.Rinxter
{
    public class RinxterImportFactory
    {
        BusinessLogic bs = null;
        FederationsEnum federationUrl = FederationsEnum.None;

        private DateTime StartedTime;

        public RinxterImportFactory()
        {
            StartedTime = DateTime.UtcNow;
        }
        public RinxterImportFactory Initialize(FederationsEnum organization)
        {
            StartedTime = DateTime.UtcNow;
            string ImageFolder = LibraryConfig.ImagesBaseSaveLocation+ @"\rx\" + StartedTime.Year + @"\" + StartedTime.Month + @"\" + StartedTime.Day + @"\";
            if (!Directory.Exists(ImageFolder))
                Directory.CreateDirectory(ImageFolder);
            federationUrl = organization;
            bs = new BusinessLogic(federationUrl, ImageFolder);

            return this;
        }

        public RinxterImportFactory RunRinxterImports()
        {
            bool isSuccess = false;
            int k = 0;

            #region Seasons
            string urlSeasons = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=seasonList&output=obj&tournamentId=0";


            isSuccess = bs.SaveSeasonsData(urlSeasons);
            if (isSuccess)
            {
                Console.WriteLine("Seasons successfully updated!" + Environment.NewLine);
            }
            else
            {
                Console.WriteLine("Seasons unsuccessfully updated!" + Environment.NewLine);
            }

            #endregion


            #region Regions
            string urlregions = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=regionList&output=obj";

            if (federationUrl == FederationsEnum.WFTDA)
            {
                isSuccess = bs.SaveRegionsData(urlregions);
                if (isSuccess)
                {
                    Console.WriteLine("Regions successfully updated!" + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Regions unsuccessfully updated!" + Environment.NewLine);
                }
            }
            else
            {
                //No need
            }

            #endregion


            #region Tournaments

            string urlTour = "";
            var dicSeasons = BusinessLogic.GetSeasons();

            foreach (var item in dicSeasons)
            {
                urlTour = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=tournamentList&season=" + item.Value + "&images=1&output=obj";

                isSuccess = bs.SaveTournamentsData(urlTour, item.Key);
                if (isSuccess)
                {
                    Console.WriteLine(String.Format("Tournaments successfully updated for {0} session!", item.Value));
                }
                else
                {
                    Console.WriteLine(String.Format("Unsuccessfully updating for Tournaments {0} session!", item.Value));
                }
            }

     
            #endregion


            #region League

            string urlLeague = "";

            if (federationUrl == FederationsEnum.WFTDA)
            {
                var dicRegions = BusinessLogic.GetRegions();

                foreach (var item in dicRegions)
                {
                    urlLeague = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=leagueList&images=1&output=obj&region=" + item.Value;

                    isSuccess = bs.SaveLeaguesData(urlLeague, item.Key);
                    if (isSuccess)
                    {
                        Console.WriteLine("Leagues successfully updated for {0} region!", item.Value);
                    }
                    else
                    {
                        Console.WriteLine("Unsuccessfully updating for Leagues {0} region!", item.Value);
                    }
                }
            }
            else
            {
                urlLeague = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=leagueList&images=1&output=obj";

                isSuccess = bs.SaveLeaguesData(urlLeague, 0); // regionId = 0  for MRDA
                if (isSuccess)
                {
                    Console.WriteLine("Leagues successfully updated!");
                }
                else
                {
                    Console.WriteLine("Leagues Unsuccessfully updated!");
                }
            }



            #endregion


            #region Team
            string urlTeam = "";
            var arrTeam = BusinessLogic.GetTournamentsIDs(federationUrl);

            Console.WriteLine("Saveing Teams by Tournaments:    Tournaments Count = {0}", arrTeam.Count);
            for (int i = 0; i < arrTeam.Count; i++)
            {
                urlTeam = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=teamList&images=1&output=obj&tournamentId=" + arrTeam[i];

                isSuccess = bs.SaveTeamsData(urlTeam, int.Parse(arrTeam[i].ToString()));
                if (isSuccess)
                {
                    Console.WriteLine("Team successfully updated for {0} tournamen!", arrTeam[i]);
                }
                else
                {
                    Console.WriteLine("Unsuccessfully Team updating for this {0} tournamen!", arrTeam[i]);
                }
            }

            #endregion


            #region Bouts
            string boutsUrl = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=boutList&startDate=2000-11-10&endDate=2020-02-10&output=tab&columns=date,outcome,venueS";

            Console.WriteLine("Saveing Bout List:  startDate=2000-11-10     endDate=2020-02-10");

            isSuccess = bs.SaveBoutsData(boutsUrl);
            if (isSuccess)
            {
                Console.WriteLine("Bouts successfully updated!");
            }
            else
            {
                Console.WriteLine("Bouts unsuccessfully  updated!");
            }

            #endregion


            #region Skaters
            string skatersUrl = "";
            var dicTeams = BusinessLogic.GetTeams(federationUrl);

            Console.WriteLine("Saveing Skaters by the team:    Teams count = {0}", dicTeams.Count);
            foreach (var item in dicTeams)
            {
                k++;
                skatersUrl = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=skaterList&images=1&output=obj&teamId=" + item.Key;

                isSuccess = bs.SaveSkatersData(skatersUrl, item.Key);
                if (isSuccess)
                {
                    Console.WriteLine("{0}. Skaters successfully updated for the {1} team!", k, item.Value);
                }
                else
                {
                    Console.WriteLine("{0}. Skaters unsuccessfully updated for this {1} team!", k, item.Value);
                }
            }


            #endregion


            #region Scores
            string scoresUrl = "";
            var dicBouts = BusinessLogic.GetBouts(federationUrl);

            Console.WriteLine("Saveing Scores by the bout:    Bouts count = {0}", dicBouts.Count);
            k = 0;
            foreach (var item in dicBouts)
            {
                k++;
                scoresUrl = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=boutScores&output=tab&boutId=" + item.Key;

                isSuccess = bs.SaveScoreData(scoresUrl, item.Key);
                if (isSuccess)
                {
                    Console.WriteLine("{0}. Scores successfully updated for the {1} bout!", k, item.Value);
                }
                else
                {
                    Console.WriteLine("{0}. Scores unsuccessfully updated for this {1} bout!", k, item.Value);
                }
            }

            Console.WriteLine(Environment.NewLine);

            #endregion


            #region Penalty

            DataTable dtBouts = BusinessLogic.GetBouts(true, federationUrl);

            Console.WriteLine("Saveing Penalties by the bout:    Bouts count = {0}", dtBouts.Rows.Count);
            k = 0;
            foreach (DataRow item in dtBouts.Rows)
            {
                k++;
                string penaltiesUrl = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=boutPenalties&boutId=" + item["BoutID"] + "&teamId=";

                //Thread tMain = new Thread(unused => bs.SavePenaltiesData(penaltiesUrl, Convert.ToInt32(item["BoutID"].ToString()), Convert.ToInt32(item["Team1ID"].ToString())));
                //tMain.Start();


                isSuccess = bs.SavePenaltiesData(penaltiesUrl, Convert.ToInt32(item["BoutID"].ToString()), Convert.ToInt32(item["Team1ID"].ToString()), Convert.ToInt32(item["Team2ID"].ToString()));
                Console.WriteLine(
                    isSuccess
                        ? "{0}. Penalties successfully updated for the {1} bout and {2} team!"
                        : "{0}. Penalties unsuccessfully updated for the {1} bout and {2} team!", k, item["Name"],
                    item["Team1ID"]);


            }

            Console.WriteLine(Environment.NewLine);

            #endregion


            #region LineUps

            Console.WriteLine("Saveing LineUps by the bout:    Bouts count = {0}", dtBouts.Rows.Count);
            k = 0;
            foreach (DataRow item in dtBouts.Rows)
            {
                k++;
                string lineupsUrl = "http://www.rinxter.net/" + federationUrl.ToString().ToLower() + "/ds?type=boutLineups&boutId=" + item["BoutID"];

                isSuccess = bs.SaveLineUpsData(lineupsUrl, Convert.ToInt32(item["BoutID"].ToString()), federationUrl);
                Console.WriteLine(
                    isSuccess
                        ? "{0}. LineUps successfully updated for the {1} bout!"
                        : "{0}. LineUps unsuccessfully updated for the {1} bout!", k, item["Name"]);


            }

            Console.WriteLine(Environment.NewLine);

            #endregion


            return this;
        }

    }
}
