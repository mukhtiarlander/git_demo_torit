using System.IO;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Federation.Enums;
using RDN.Library.DataModels.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RDN.Library.DataModels.Imports.Rinxter;
using System.Net;
using RDN.Portable.Classes.Federation.Enums;

namespace RDN.Library.Classes.Imports.Rinxter
{
    public class BusinessLogic
    {
        private string IamgeFolder = "";
        private bool IsFemale = true;
        FederationsEnum federationUrl = FederationsEnum.None;
        public BusinessLogic(FederationsEnum organization, string iamgeFolder)
        {
            federationUrl = organization;
            IamgeFolder = iamgeFolder;
            IsFemale = federationUrl == FederationsEnum.WFTDA;
        }

        public static Dictionary<int, string> GetSeasons()
        {
            var db = new ManagementContext();
            // Display all Seasons from the database 
            var query = from b in db.RSeasons
                        orderby b.Value
                        select b;
            Dictionary<int, string> dicSessions = new Dictionary<int, string>();

            foreach (var item in query)
            {
                dicSessions.Add(Convert.ToInt32(item.SeasonIds), item.Value);
            }
            return dicSessions;

        }

        public static Dictionary<int, string> GetRegions()
        {
            var db = new ManagementContext();
            // Display all Regions from the database 
            var query = from b in db.RRegions
                        orderby b.Name
                        select b;
            Dictionary<int, string> dicSessions = new Dictionary<int, string>();

            foreach (var item in query)
            {
                dicSessions.Add(Convert.ToInt32(item.RegionIds), item.Name);
            }
            return dicSessions;

        }

        public static Dictionary<int, string> GetTeams(FederationsEnum federationUrl)
        {
            bool isFemale = federationUrl == FederationsEnum.WFTDA;

            var db = new ManagementContext();

            // Display all Teams from the database 
            var getTeamsByFederation = from b in db.RTeams
                                       where b.IsFemale == isFemale && b.IsDeleted == false
                                       orderby b.Name
                                       select b;
            Dictionary<int, string> dicSessions = new Dictionary<int, string>();

            foreach (var item in getTeamsByFederation)
            {
                if (dicSessions.Where(x => x.Key == item.ID).FirstOrDefault().Key != item.ID)
                    dicSessions.Add(item.ID, item.Name);
            }
            return dicSessions;

        }

        public static Dictionary<int, string> GetBouts(FederationsEnum federationUrl)
        {
            bool isFemale = federationUrl == FederationsEnum.WFTDA;

            var db = new ManagementContext();

            // get all Bouts by federation from the database 
            var query = from b in db.RBouts
                        where b.IsFemale == isFemale && b.IsDeleted == false
                        orderby b.ID
                        select b;

            Dictionary<int, string> dicBouts = new Dictionary<int, string>();

            foreach (var item in query)
            {
                dicBouts.Add(item.ID, item.Venue);
            }
            return dicBouts;

        }

        public static DataTable GetBouts(bool isTable, FederationsEnum federationUrl)
        {
            bool isFemale = federationUrl == FederationsEnum.WFTDA;

            var db = new ManagementContext();

            // Display all Bouts from the database 
            var getBouts = from b in db.RBouts
                           where b.IsFemale == isFemale && b.IsDeleted == false
                           orderby b.ID
                           select b;

            DataTable dtBouts = new DataTable();
            dtBouts.Columns.Add("BoutID");
            dtBouts.Columns.Add("Name");
            dtBouts.Columns.Add("Team1ID");
            dtBouts.Columns.Add("Team2ID");

            if (isTable)
            {
                foreach (var item in getBouts)
                {
                    DataRow dr = dtBouts.NewRow();

                    dr["BoutID"] = item.ID;
                    dr["Name"] = item.Venue;
                    dr["Team1ID"] = item.Team1ID;
                    dr["Team2ID"] = item.Team2ID;
                    dtBouts.Rows.Add(dr);
                }
            }

            return dtBouts;

        }

        public static ArrayList GetTournamentsIDs(FederationsEnum federationUrl)
        {
            bool isFemale = federationUrl == FederationsEnum.WFTDA;

            var db = new ManagementContext();
            // geting all TournamentsID from the database 
            var getTournaments = from b in db.RTournaments
                                 where b.IsFemale == isFemale && b.IsDeleted == false
                                 orderby b.ID
                                 select b;
            ArrayList arrTournamentsID = new ArrayList();

            foreach (var item in getTournaments)
            {
                arrTournamentsID.Add(item.ID);
            }
            return arrTournamentsID;

        }



        public bool SaveTournamentsData(string tournamentsJsonUrl, int seassionId)
        {
            ArrayList arrUrl = WebProvider.GetTournamentsUrl(tournamentsJsonUrl, federationUrl);

            DataModels.Imports.Rinxter.RTournaments[] tours = new DataModels.Imports.Rinxter.RTournaments[arrUrl.Count];

            for (int i = 0; i < arrUrl.Count; i++)
            {
                tours[i] = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RTournaments>(arrUrl[i].ToString());
            }

            try
            {
                var db = new ManagementContext();

                foreach (var item in tours)
                {
                    var imagePath = IamgeFolder + new FileInfo(item.Image).Name;

                    // get Tournament by ID from the database 
                    var getTournament = (from b in db.RTournaments
                                         where b.ID == item.ID && b.IsFemale == IsFemale
                                         select b).FirstOrDefault();


                    if (getTournament == null)
                    {
                        item.SeasonID = seassionId;
                        item.IsFemale = IsFemale;
                        item.LastModified = DateTime.UtcNow;

                        db.RTournaments.Add(item);
                        db.SaveChanges();


                        //Add Tournament Image 
                        DataModels.Imports.Rinxter.RTournamentPhoto turPhoto = new RTournamentPhoto();
                        turPhoto.AlternativeText = item.Name;
                        turPhoto.ImageUrl = item.Image;
                        turPhoto.IsPrimaryPhoto = true;
                        turPhoto.SaveLocation = imagePath;
                        turPhoto.SaveLocationThumb = imagePath;
                        turPhoto.TournamentId = item.TournamentsId;

                        db.RTournamentPhoto.Add(turPhoto);
                        if (!File.Exists(imagePath))
                        {
                            WebClient client = new WebClient();
                            client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                        }
                    }
                    else
                    {

                        getTournament.Name = item.Name;
                        getTournament.Address = item.Address;
                        getTournament.Abbr = item.Abbr;
                        getTournament.Url = item.Url;
                        getTournament.LastModified = DateTime.UtcNow;
                        getTournament.City = item.City;
                        getTournament.Description = item.Description;
                        getTournament.EndDate = item.EndDate;
                        getTournament.PostCode = item.PostCode;
                        getTournament.SeasonID = seassionId;
                        getTournament.StartDate = item.StartDate;
                        getTournament.Venue = item.Venue;
                        getTournament.IsFemale = IsFemale;
                        getTournament.IsDeleted = false;


                        //Update Tournament Image table
                        var oldTournamentPhoto = db.RTournamentPhoto.SingleOrDefault(tp => tp.TournamentId == getTournament.TournamentsId);
                        if (oldTournamentPhoto == null)
                        {

                            DataModels.Imports.Rinxter.RTournamentPhoto tournamentPhoto = new RTournamentPhoto();
                            tournamentPhoto.AlternativeText = item.Name;
                            tournamentPhoto.ImageUrl = item.Image;
                            tournamentPhoto.IsPrimaryPhoto = true;
                            tournamentPhoto.SaveLocation = imagePath;
                            tournamentPhoto.SaveLocationThumb = imagePath;
                            tournamentPhoto.TournamentId = getTournament.TournamentsId;

                            db.RTournamentPhoto.Add(tournamentPhoto);
                            if (!File.Exists(imagePath))
                            {
                                WebClient client = new WebClient();
                                client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                            }

                        }
                    }



                    int c = db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());

                return false; ;
            }

            return true;
        }




        public bool SaveLeaguesData(string leagueJsonUrl, int regionId)
        {
            ArrayList arrUrl = WebProvider.GetLeagueUrl(leagueJsonUrl.Replace(" ", "%20"), federationUrl);

            DataModels.Imports.Rinxter.RLeague[] leagues = new DataModels.Imports.Rinxter.RLeague[arrUrl.Count];

            for (int i = 0; i < arrUrl.Count; i++)
            {
                leagues[i] = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RLeague>(arrUrl[i].ToString());
            }

            try
            {
                var db = new ManagementContext();
                {
                    foreach (var item in leagues)
                    {
                        var imagePath = IamgeFolder + new FileInfo(item.Image).Name;


                        // get league by ID from the database 
                        var getLeague = (from b in db.RLeague
                                         where b.ID == item.ID
                                         select b).FirstOrDefault();


                        if (getLeague == null)
                        {
                            item.RegionID = regionId;
                            item.IsFemale = IsFemale;
                            item.LastModified = DateTime.UtcNow;
                            db.RLeague.Add(item);

                            db.SaveChanges();


                            //Add Legua Image 
                            DataModels.Imports.Rinxter.RLeaguePhoto leaguePhoto = new RLeaguePhoto();
                            leaguePhoto.AlternativeText = item.Name;
                            leaguePhoto.ImageUrl = item.Image;
                            leaguePhoto.IsPrimaryPhoto = true;
                            leaguePhoto.SaveLocation = imagePath;
                            leaguePhoto.IsVisibleToPublic = true;
                            leaguePhoto.SaveLocationThumb = imagePath;
                            leaguePhoto.LeagueId = item.LeagueId;

                            db.RLeaguePhoto.Add(leaguePhoto);
                            if (!File.Exists(imagePath))
                            {
                                WebClient client = new WebClient();
                                client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                            }
                        }
                        else
                        {

                            getLeague.Name = item.Name;
                            getLeague.RegionID = item.RegionID;
                            getLeague.Abbr = item.Abbr;
                            getLeague.Url = item.Url;
                            getLeague.LastModified = DateTime.UtcNow;
                            getLeague.Location = item.Location;
                            getLeague.JoinDate = item.JoinDate;
                            getLeague.IsFemale = IsFemale;

                            //Update Legua Image table
                            var oldLeguaPhoto = db.RLeaguePhoto.SingleOrDefault(lp => lp.LeagueId == getLeague.LeagueId);
                            if (oldLeguaPhoto == null)
                            {

                                DataModels.Imports.Rinxter.RLeaguePhoto leaguePhoto = new RLeaguePhoto();
                                leaguePhoto.AlternativeText = item.Name;
                                leaguePhoto.ImageUrl = item.Image;
                                leaguePhoto.IsPrimaryPhoto = true;
                                leaguePhoto.SaveLocation = imagePath;
                                leaguePhoto.SaveLocationThumb = imagePath;
                                leaguePhoto.LeagueId = getLeague.LeagueId;

                                db.RLeaguePhoto.Add(leaguePhoto);


                                if (!File.Exists(imagePath))
                                {
                                    WebClient client = new WebClient();
                                    client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                                }
                            }
                        }



                        int c = db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());
                return false; ;
            }

            return true;
        }




        public bool SaveTeamsData(string teamJsonUrl, int tournamentId)
        {
            ArrayList arrUrl = WebProvider.GetTeamUrl(teamJsonUrl, federationUrl);

            DataModels.Imports.Rinxter.RTeams[] teams = new DataModels.Imports.Rinxter.RTeams[arrUrl.Count];

            for (int i = 0; i < arrUrl.Count; i++)
            {
                teams[i] = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RTeams>(arrUrl[i].ToString());
            }

            try
            {
                var db = new ManagementContext();

                foreach (var item in teams)
                {
                    try
                    {
                        var imagePath = IamgeFolder + new FileInfo(item.Image).Name;


                        // get team by ID and by tournamentId from the database 
                        var getTeam = (from t in db.RTeams
                                       where t.ID == item.ID && t.TournamentsID == tournamentId && t.IsFemale == IsFemale
                                       select t).FirstOrDefault();

                        item.TournamentsID = tournamentId;

                        var getLeague = (from l in db.RLeague
                                         where l.ID == item.PrimaryLeagueID && l.IsFemale == IsFemale
                                         select l).FirstOrDefault();

                        DataModels.Imports.Rinxter.RTeamTournaments tt = new DataModels.Imports.Rinxter.RTeamTournaments();
                        tt.TeamID = item.ID;
                        tt.TournamentsID = tournamentId;

                        if (getLeague != null)
                        {
                            if (getTeam == null)
                            {
                                // get team by ID 
                                var getGlobalTeam = (from t in db.RTeams
                                                     where t.ID == item.ID && t.IsFemale == IsFemale
                                                     select t).FirstOrDefault();

                                if (getGlobalTeam == null)
                                {
                                    item.LastModified = DateTime.UtcNow;
                                    item.IsFemale = IsFemale;
                                    db.RTeams.Add(item);

                                    int c = db.SaveChanges();


                                    //Add Team Image 
                                    DataModels.Imports.Rinxter.RTeamPhoto teamPhoto = new RTeamPhoto();
                                    teamPhoto.AlternativeText = item.Name;
                                    teamPhoto.ImageUrl = item.Image;
                                    teamPhoto.IsPrimaryPhoto = true;
                                    teamPhoto.SaveLocation = imagePath;
                                    teamPhoto.IsVisibleToPublic = true;
                                    teamPhoto.SaveLocationThumb = imagePath;
                                    teamPhoto.TeamId = item.TeamId;

                                    db.RTeamPhoto.Add(teamPhoto);

                                    if (!File.Exists(imagePath))
                                    {
                                        WebClient client = new WebClient();
                                        client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                                    }
                                }
                                else
                                {
                                    //Update Team Image table or add it

                                    getGlobalTeam.Name = item.Name;
                                    getGlobalTeam.Color = item.Color;
                                    getGlobalTeam.Letter = item.Letter;
                                    getGlobalTeam.Abbr = item.Abbr;
                                    getGlobalTeam.LastModified = DateTime.UtcNow;
                                    getGlobalTeam.IsFemale = IsFemale;
                                    getGlobalTeam.IsDeleted = false;

                                    var oldPhotoOfTeam = db.RTeamPhoto.Where(x => x.TeamId == getGlobalTeam.TeamId).FirstOrDefault();
                                    if (oldPhotoOfTeam == null)
                                    {
                                        DataModels.Imports.Rinxter.RTeamPhoto teamPhoto = new RTeamPhoto();
                                        teamPhoto.AlternativeText = item.Name;
                                        teamPhoto.ImageUrl = item.Image;
                                        teamPhoto.IsPrimaryPhoto = true;
                                        teamPhoto.SaveLocation = imagePath;
                                        teamPhoto.SaveLocationThumb = imagePath;
                                        teamPhoto.TeamId = getGlobalTeam.TeamId;

                                        db.RTeamPhoto.Add(teamPhoto);
                                        if (!File.Exists(imagePath))
                                        {
                                            WebClient client = new WebClient();
                                            client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                                        }
                                    }
                                }

                                //check if the team connected with Tournament
                                var getTeamTournament = (from l in db.RTeamTournaments
                                                         where l.TeamID == item.ID && l.TournamentsID == item.TournamentsID
                                                         select l).FirstOrDefault();

                                if (getTeamTournament == null)
                                {
                                    db.RTeamTournaments.Add(tt);
                                    int c = db.SaveChanges();

                                }

                            }
                            else
                            {
                                //Update data for the team <


                                getTeam.Name = item.Name;
                                getTeam.Abbr = item.Abbr;
                                getTeam.Bouts1 = item.Bouts1;
                                getTeam.Bouts = item.Bouts;
                                getTeam.League = item.League;
                                getTeam.LastModified = DateTime.UtcNow;
                                getTeam.Color = item.Color;
                                getTeam.Letter = item.Letter;
                                getTeam.Type = item.Type;
                                getTeam.IsFemale = IsFemale;
                                getTeam.IsDeleted = false;

                                //check if the team connected with Tournament <
                                var getTeamTournament = (from l in db.RTeamTournaments
                                                         where l.TeamID == item.ID && l.TournamentsID == item.TournamentsID
                                                         select l).FirstOrDefault();

                                if (getTeamTournament == null)
                                {
                                    db.RTeamTournaments.Add(tt);
                                }

                                //Update Team Image table
                                var oldTeamPhoto = db.RTeamPhoto.Where(x => x.TeamId == getTeamTournament.TeamID).FirstOrDefault();
                                if (oldTeamPhoto == null)
                                {

                                    DataModels.Imports.Rinxter.RTeamPhoto teamPhoto = new RTeamPhoto();
                                    teamPhoto.AlternativeText = item.Name;
                                    teamPhoto.ImageUrl = item.Image;
                                    teamPhoto.IsPrimaryPhoto = true;
                                    teamPhoto.SaveLocation = imagePath;
                                    teamPhoto.SaveLocationThumb = imagePath;
                                    teamPhoto.TeamId = getTeamTournament.TeamID;

                                    db.RTeamPhoto.Add(teamPhoto);
                                    if (!File.Exists(imagePath))
                                    {
                                        WebClient client = new WebClient();
                                        client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                                    }
                                }
                            }


                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorDatabaseManager.AddException(ex, GetType());
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());
                return false;
            }

            return true;
        }



        public bool SaveLeague(int leagueId)
        {
            DataModels.Imports.Rinxter.RLeague league = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RLeague>(WebProvider.GetLeagueUrl(leagueId, federationUrl));

            try
            {
                var db = new ManagementContext();

                var queryLeague = (from l in db.RLeague
                                   where l.ID == league.ID && l.IsFemale == IsFemale
                                   select l).FirstOrDefault();

                if (queryLeague == null)
                {
                    league.IsFemale = IsFemale;
                    league.LastModified = DateTime.UtcNow;
                    db.RLeague.Add(league);
                }

                int c = db.SaveChanges();

            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());
                return false;
            }

            return true;
        }


        public bool SaveTeam(int teamId)
        {

            DataModels.Imports.Rinxter.RTeams team = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RTeams>(WebProvider.GetTeamUrl(teamId, federationUrl));

            try
            {
                var db = new ManagementContext();
                {
                    if (String.IsNullOrEmpty(team.Type))
                        team.Type = " ";

                    var queryLeague = (from l in db.RLeague
                                       where l.ID == team.PrimaryLeagueID && l.IsFemale == IsFemale
                                       select l).FirstOrDefault();

                    bool isOK = true;
                    if (queryLeague == null)
                    {
                        isOK = SaveLeague(team.PrimaryLeagueID);
                    }
                    if (isOK)
                    {
                        team.IsFemale = IsFemale;
                        team.LastModified = DateTime.UtcNow;
                        db.RTeams.Add(team);

                        int c = db.SaveChanges();

                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType(), additionalInformation: "   TeamID=" + team.ID);
                return false; ;
            }


        }


        public bool SaveBoutsData(string boutsJsonUrl)
        {
            ArrayList arrUrl = WebProvider.GetBoutUrl(boutsJsonUrl, federationUrl);

            DataModels.Imports.Rinxter.RBouts[] bouts = new DataModels.Imports.Rinxter.RBouts[arrUrl.Count];

            try
            {
                var db = new ManagementContext();

                for (int i = 0; i < bouts.Length; i++)
                {
                    bouts[i] = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RBouts>(arrUrl[i].ToString());
                    var item = bouts[i];

                    if (item.ID == 0) continue;

                    var getBouts = (from b in db.RBouts
                                    where b.ID == item.ID && b.IsFemale == IsFemale
                                    select b).FirstOrDefault();

                    var getTeam1 = (from t in db.RTeams
                                    where t.ID == item.Team1ID && t.IsFemale == IsFemale
                                    select t).FirstOrDefault();

                    var getTeam2 = (from t in db.RTeams
                                    where t.ID == item.Team2ID && t.IsFemale == IsFemale
                                    select t).FirstOrDefault();

                    bool isOk = true;
                    if (getBouts == null)
                    {
                        if (getTeam1 == null)
                        {
                            isOk = SaveTeam(item.Team1ID);
                        }
                        if (isOk && getTeam2 == null)
                        {
                            isOk = SaveTeam(item.Team2ID);
                        }

                        if (isOk)
                        {
                            item.IsFemale = IsFemale;
                            item.LastModified = DateTime.UtcNow;
                            db.RBouts.Add(item);

                            int c = db.SaveChanges();

                        }
                    }
                    else
                    {
                        getBouts.TournamentsID = item.TournamentsID;
                        getBouts.Date = item.Date;
                        getBouts.Location = item.Location;
                        getBouts.Sanction = item.Sanction;
                        getBouts.Season = item.Season;
                        getBouts.State = item.State;
                        getBouts.Status = item.Status;
                        getBouts.TimeStart = item.TimeStart;
                        getBouts.LastModified = DateTime.UtcNow;
                        getBouts.IsFemale = IsFemale;

                        int c = db.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());

                return false; ;
            }

            // SaveBoutsAll(5000);

            return true;
        }




        private bool SaveBoutsAll(int count)
        {
            ArrayList arrUrl = new ArrayList();

            for (int i = 0; i < count; i++)
            {
                arrUrl.Add(WebProvider.GetBoutUrl(i, federationUrl));
            }

            DataModels.Imports.Rinxter.RBouts[] bouts = new DataModels.Imports.Rinxter.RBouts[arrUrl.Count];

            var db = new ManagementContext();
            {
                for (int i = 1; i < bouts.Length; i++) //
                {
                    bouts[i] = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RBouts>(arrUrl[i].ToString());

                    try
                    {

                        var item = bouts[i];
                        if (item != null && item.ID != 0)
                        {
                            var queryB = (from b in db.RBouts
                                          where b.ID == item.ID
                                          select b).FirstOrDefault();

                            var queryT1 = (from t in db.RTeams
                                           where t.ID == item.Team1ID
                                           select t).FirstOrDefault();

                            var queryT2 = (from t in db.RTeams
                                           where t.ID == item.Team2ID
                                           select t).FirstOrDefault();

                            bool isOk = true;
                            if (queryB == null)
                            {
                                if (queryT1 == null)
                                {
                                    isOk = SaveTeam(item.Team1ID);
                                }
                                if (isOk && queryT2 == null)
                                {
                                    isOk = SaveTeam(item.Team2ID);
                                }
                                if (isOk)
                                {
                                    item.IsFemale = IsFemale;
                                    db.RBouts.Add(item);

                                    int c = db.SaveChanges();

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        ErrorDatabaseManager.AddException(ex, GetType());

                        return false;
                    }
                }
            }
            return true;
        }


        public bool SaveSkatersData(string skatersJsonUrl, int teamId)
        {
            ArrayList arrUrl = WebProvider.GetSkaterUrl(skatersJsonUrl, federationUrl);

            DataModels.Imports.Rinxter.RSkaters[] skaters = new DataModels.Imports.Rinxter.RSkaters[arrUrl.Count];

            try
            {
                var db = new ManagementContext();
                {
                    for (int i = 0; i < skaters.Length; i++)
                    {
                        try
                        {
                            skaters[i] = WebProvider.Download_serialized_json_data<DataModels.Imports.Rinxter.RSkaters>(arrUrl[i].ToString());
                        }
                        catch (Exception ex)
                        {

                            ErrorDatabaseManager.AddException(ex, GetType());

                            continue;
                        }

                        var item = skaters[i];

                        if (item.ID == 0) continue;

                        var imagePath = IamgeFolder + new FileInfo(item.Image).Name;

                        var getSkater = (from b in db.RSkaters
                                         where b.ID == item.ID && b.IsFemale == IsFemale
                                         select b).FirstOrDefault();


                        DataModels.Imports.Rinxter.RSkaterTeam skTeam = new DataModels.Imports.Rinxter.RSkaterTeam();
                        skTeam.SkaterID = item.ID;
                        skTeam.TeamID = teamId;

                        if (getSkater == null)
                        {
                            item.TeamID = teamId;
                            item.IsFemale = IsFemale;
                            item.LastModified = DateTime.UtcNow;

                            db.RSkaters.Add(item);

                            try
                            {
                                int c = db.SaveChanges();

                                var newSkaterId = db.RSkaters.Max(s => s.SkaterId);

                                //Add Skaer  Image <
                                DataModels.Imports.Rinxter.RSkaterPhoto skaterPhoto = new RSkaterPhoto();
                                skaterPhoto.AlternativeText = item.Name;
                                skaterPhoto.ImageUrl = item.Image;
                                skaterPhoto.IsPrimaryPhoto = true;
                                skaterPhoto.SaveLocation = imagePath;
                                skaterPhoto.IsVisibleToPublic = true;
                                skaterPhoto.SaveLocationThumb = imagePath;
                                skaterPhoto.SkaterId = newSkaterId;

                                db.RSkaterPhoto.Add(skaterPhoto);
                                if (!File.Exists(imagePath))
                                {
                                    WebClient client = new WebClient();
                                    client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                                }
                                //add relation between Skater and team
                                db.RSkaterTeams.Add(skTeam);

                                c = db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                ErrorDatabaseManager.AddException(ex, GetType(), additionalInformation: "\n   Skater=" + item.ID);
                                db.RSkaters.Remove(item);
                            }


                        }
                        else
                        {
                            //Update data for the skater <
                            getSkater.Name = item.Name;
                            getSkater.Position = item.Position;
                            getSkater.LastModified = DateTime.UtcNow;
                            getSkater.Number = item.Number;
                            getSkater.IsFemale = IsFemale;
                            // >


                            //check if the skater connected with team <
                            var querySkTeam = (from b in db.RSkaterTeams
                                               where b.SkaterID == skTeam.SkaterID && b.TeamID == skTeam.TeamID
                                               select b).FirstOrDefault();

                            if (querySkTeam == null)
                            {
                                db.RSkaterTeams.Add(skTeam);

                                int c = db.SaveChanges();

                            }
                            // >


                            //Update Skater Image table <
                            var oldSkaterPhoto = db.RSkaterPhoto.SingleOrDefault(tp => tp.SkaterId == getSkater.SkaterId);
                            if (oldSkaterPhoto == null)
                            {

                                DataModels.Imports.Rinxter.RSkaterPhoto skaterPhoto = new RSkaterPhoto();
                                skaterPhoto.AlternativeText = item.Name;
                                skaterPhoto.ImageUrl = item.Image;
                                skaterPhoto.IsPrimaryPhoto = true;
                                skaterPhoto.SaveLocation = imagePath;
                                skaterPhoto.SaveLocationThumb = imagePath;
                                skaterPhoto.SkaterId = getSkater.SkaterId;

                                db.RSkaterPhoto.Add(skaterPhoto);

                                if (!File.Exists(imagePath))
                                {
                                    WebClient client = new WebClient();
                                    client.DownloadFile("http://www.rinxter.net/" + item.Image, imagePath);
                                }
                            }
                            // >

                        }

                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());

                return false; ;
            }

            return true;
        }


        public bool SaveScoreData(string scoreJsonUrl, int boutId)
        {
            Dictionary<string, string>[] dicScores = WebProvider.Download_serialized_json_scores_data(scoreJsonUrl);

            try
            {
                var db = new ManagementContext();
                if (dicScores != null)
                {
                    foreach (var dicitem in dicScores)
                    {
                        DataModels.Imports.Rinxter.RScores score = new DataModels.Imports.Rinxter.RScores();

                        string tempLD1 = dicitem["LD1"];
                        int tempScore1 = Convert.ToInt32(dicitem["Score1"]);
                        int tempScore2 = Convert.ToInt32(dicitem["Score2"]);
                        var getScores = (from b in db.RScores
                                         where b.BoutID == boutId && b.LD1 == tempLD1 && b.Score1 == tempScore1 && b.Score2 == tempScore2
                                         select b).FirstOrDefault();


                        if (getScores == null)
                        {
                            score.BoutID = boutId;
                            score.LD1 = dicitem["LD1"];
                            score.LD2 = dicitem["LD2"];
                            score.Points1 = dicitem["Points1"];
                            score.Points2 = dicitem["Points2"];
                            score.Score1 = Convert.ToInt32(dicitem["Score1"]);
                            score.Score2 = Convert.ToInt32(dicitem["Score2"]);
                            score.LastModified = DateTime.UtcNow;

                            db.RScores.Add(score);
                        }
                        else
                        {
                            getScores.Points1 = dicitem["Points1"];
                            getScores.Points2 = dicitem["Points2"];
                            getScores.LastModified = DateTime.UtcNow;
                        }


                        int c = db.SaveChanges();

                    }
                }


            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType(), additionalInformation: scoreJsonUrl + ":" + boutId);

                return false; ;
            }

            return true;
        }


        public bool SavePenaltiesData(string penaltyJsonUrl, int boutId, int team1Id, int team2Id)
        {

            try
            {
                var db = new ManagementContext();

                Dictionary<string, string>[] dicPenalties = WebProvider.Download_serialized_json_penalties_data(penaltyJsonUrl + team1Id);
                if (dicPenalties != null)
                {
                    foreach (var dicitem in dicPenalties)
                    {
                        DataModels.Imports.Rinxter.RPenalties pn = new DataModels.Imports.Rinxter.RPenalties();
                        int skID = Convert.ToInt32(dicitem["SkaterID"]);

                        var getPenalties = (from b in db.RPenalties
                                            where b.TeamID == team1Id && b.BoutID == boutId && b.SkaterID == skID
                                            select b).FirstOrDefault();


                        if (getPenalties == null)
                        {
                            pn.BoutID = boutId;
                            pn.TeamID = team1Id;
                            pn.SkaterID = skID;
                            pn.MinorA1 = dicitem["MinorA1"];
                            pn.MinorA2 = dicitem["MinorA2"];
                            pn.MinorA3 = dicitem["MinorA3"];
                            pn.MinorA4 = dicitem["MinorA4"];

                            pn.MinorB1 = dicitem["MinorB1"];
                            pn.MinorB2 = dicitem["MinorB2"];
                            pn.MinorB3 = dicitem["MinorB3"];
                            pn.MinorB4 = dicitem["MinorB4"];

                            pn.MinorC1 = dicitem["MinorC1"];
                            pn.MinorC2 = dicitem["MinorC2"];
                            pn.MinorC3 = dicitem["MinorC3"];
                            pn.MinorC4 = dicitem["MinorC4"];

                            pn.MinorD1 = dicitem["MinorD1"];
                            pn.MinorD2 = dicitem["MinorD2"];
                            pn.MinorD3 = dicitem["MinorD3"];
                            pn.MinorD4 = dicitem["MinorD4"];

                            pn.Major1 = dicitem["Major1"];
                            pn.Major2 = dicitem["Major2"];
                            pn.Major3 = dicitem["Major3"];
                            pn.Major4 = dicitem["Major4"];
                            pn.Major5 = dicitem["Major5"];
                            pn.Major6 = dicitem["Major6"];
                            pn.Major7 = dicitem["Major7"];
                            pn.LastModified = DateTime.UtcNow;

                            db.RPenalties.Add(pn);
                            try
                            {
                                int c = db.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                                ErrorDatabaseManager.AddException(ex, GetType(), additionalInformation: "\n   Bout=" + boutId);

                                db.RPenalties.Remove(pn);
                            }
                        }
                        else
                        {
                            getPenalties.MinorA1 = dicitem["MinorA1"];
                            getPenalties.MinorA2 = dicitem["MinorA2"];
                            getPenalties.MinorA3 = dicitem["MinorA3"];
                            getPenalties.MinorA4 = dicitem["MinorA4"];

                            getPenalties.MinorB1 = dicitem["MinorB1"];
                            getPenalties.MinorB2 = dicitem["MinorB2"];
                            getPenalties.MinorB3 = dicitem["MinorB3"];
                            getPenalties.MinorB4 = dicitem["MinorB4"];

                            getPenalties.MinorC1 = dicitem["MinorC1"];
                            getPenalties.MinorC2 = dicitem["MinorC2"];
                            getPenalties.MinorC3 = dicitem["MinorC3"];
                            getPenalties.MinorC4 = dicitem["MinorC4"];

                            getPenalties.MinorD1 = dicitem["MinorD1"];
                            getPenalties.MinorD2 = dicitem["MinorD2"];
                            getPenalties.MinorD3 = dicitem["MinorD3"];
                            getPenalties.MinorD4 = dicitem["MinorD4"];

                            getPenalties.Major1 = dicitem["Major1"];
                            getPenalties.Major2 = dicitem["Major2"];
                            getPenalties.Major3 = dicitem["Major3"];
                            getPenalties.Major4 = dicitem["Major4"];
                            getPenalties.Major5 = dicitem["Major5"];
                            getPenalties.Major6 = dicitem["Major6"];
                            getPenalties.Major7 = dicitem["Major7"];
                            getPenalties.LastModified = DateTime.UtcNow;
                            getPenalties.IsDeleted = false;


                            int c = db.SaveChanges();

                        }
                    }
                }
                dicPenalties = WebProvider.Download_serialized_json_penalties_data(penaltyJsonUrl + team2Id);
                if (dicPenalties != null)
                {
                    foreach (var dicitem in dicPenalties)
                    {
                        try
                        {
                            DataModels.Imports.Rinxter.RPenalties pn = new DataModels.Imports.Rinxter.RPenalties();
                            int skID = Convert.ToInt32(dicitem["SkaterID"]);

                            var getPenalties = (from b in db.RPenalties
                                                where b.TeamID == team2Id && b.BoutID == boutId && b.SkaterID == skID
                                                select b).FirstOrDefault();




                            if (getPenalties == null)
                            {
                                pn.BoutID = boutId;
                                pn.TeamID = team2Id;
                                pn.SkaterID = skID;
                                pn.MinorA1 = dicitem["MinorA1"];
                                pn.MinorA2 = dicitem["MinorA2"];
                                pn.MinorA3 = dicitem["MinorA3"];
                                pn.MinorA4 = dicitem["MinorA4"];

                                pn.MinorB1 = dicitem["MinorB1"];
                                pn.MinorB2 = dicitem["MinorB2"];
                                pn.MinorB3 = dicitem["MinorB3"];
                                pn.MinorB4 = dicitem["MinorB4"];

                                pn.MinorC1 = dicitem["MinorC1"];
                                pn.MinorC2 = dicitem["MinorC2"];
                                pn.MinorC3 = dicitem["MinorC3"];
                                pn.MinorC4 = dicitem["MinorC4"];

                                pn.MinorD1 = dicitem["MinorD1"];
                                pn.MinorD2 = dicitem["MinorD2"];
                                pn.MinorD3 = dicitem["MinorD3"];
                                pn.MinorD4 = dicitem["MinorD4"];

                                pn.Major1 = dicitem["Major1"];
                                pn.Major2 = dicitem["Major2"];
                                pn.Major3 = dicitem["Major3"];
                                pn.Major4 = dicitem["Major4"];
                                pn.Major5 = dicitem["Major5"];
                                pn.Major6 = dicitem["Major6"];
                                pn.Major7 = dicitem["Major7"];
                                pn.LastModified = DateTime.UtcNow;

                                db.RPenalties.Add(pn);

                                int c = db.SaveChanges();

                            }
                            else
                            {
                                getPenalties.MinorA1 = dicitem["MinorA1"];
                                getPenalties.MinorA2 = dicitem["MinorA2"];
                                getPenalties.MinorA3 = dicitem["MinorA3"];
                                getPenalties.MinorA4 = dicitem["MinorA4"];

                                getPenalties.MinorB1 = dicitem["MinorB1"];
                                getPenalties.MinorB2 = dicitem["MinorB2"];
                                getPenalties.MinorB3 = dicitem["MinorB3"];
                                getPenalties.MinorB4 = dicitem["MinorB4"];

                                getPenalties.MinorC1 = dicitem["MinorC1"];
                                getPenalties.MinorC2 = dicitem["MinorC2"];
                                getPenalties.MinorC3 = dicitem["MinorC3"];
                                getPenalties.MinorC4 = dicitem["MinorC4"];

                                getPenalties.MinorD1 = dicitem["MinorD1"];
                                getPenalties.MinorD2 = dicitem["MinorD2"];
                                getPenalties.MinorD3 = dicitem["MinorD3"];
                                getPenalties.MinorD4 = dicitem["MinorD4"];

                                getPenalties.Major1 = dicitem["Major1"];
                                getPenalties.Major2 = dicitem["Major2"];
                                getPenalties.Major3 = dicitem["Major3"];
                                getPenalties.Major4 = dicitem["Major4"];
                                getPenalties.Major5 = dicitem["Major5"];
                                getPenalties.Major6 = dicitem["Major6"];
                                getPenalties.Major7 = dicitem["Major7"];
                                getPenalties.LastModified = DateTime.UtcNow;
                                getPenalties.IsDeleted = false;


                                int c = db.SaveChanges();

                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorDatabaseManager.AddException(ex, GetType(), additionalInformation: "\n   Bout=" + boutId + team2Id + ":" + team1Id);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType(), additionalInformation: penaltyJsonUrl + team2Id + ":" + team1Id);
                return false; ;
            }

            return true;
        }


        public bool SaveLineUpsData(string lineUpsJsonUrl, int boutId, FederationsEnum federationUrl)
        {
            bool isFemale = federationUrl == FederationsEnum.WFTDA;

            try
            {
                var db = new ManagementContext();

                Dictionary<string, string>[] dicLineUps = WebProvider.Download_serialized_json_lineups_data(lineUpsJsonUrl);
                foreach (var dicitem in dicLineUps)
                {
                    try
                    {
                        DataModels.Imports.Rinxter.RLineUps lUp = new DataModels.Imports.Rinxter.RLineUps();
                        string jam = dicitem["Jam"];
                        string teamName = dicitem["TeamName"];

                        var getLineups = (from b in db.RLineUps
                                          where b.BoutID == boutId && b.IsFemale == isFemale && b.Jam == jam && b.TeamName == teamName
                                          select b).FirstOrDefault();

                        if (getLineups == null)
                        {
                            lUp.Jam = dicitem["Jam"];
                            lUp.BoutID = boutId;
                            lUp.TeamName = dicitem["TeamName"];
                            lUp.Jammer = dicitem["Jammer"];
                            lUp.PivotBlocker = dicitem["PivotBlocker"];
                            lUp.Blocker = dicitem["Blocker"];
                            lUp.Blocker1 = dicitem["Blocker1"];
                            lUp.Blocker2 = dicitem["Blocker2"];
                            lUp.LastModified = DateTime.UtcNow;
                            lUp.IsFemale = isFemale;

                            db.RLineUps.Add(lUp);

                            int c = db.SaveChanges();

                        }
                        else
                        {
                            getLineups.Jam = dicitem["Jam"];
                            getLineups.TeamName = dicitem["TeamName"];
                            getLineups.Jammer = dicitem["Jammer"];
                            getLineups.PivotBlocker = dicitem["PivotBlocker"];
                            getLineups.Blocker = dicitem["Blocker"];
                            getLineups.Blocker1 = dicitem["Blocker1"];
                            getLineups.Blocker2 = dicitem["Blocker2"];
                            getLineups.LastModified = DateTime.UtcNow;
                            getLineups.IsDeleted = false;
                            lUp.IsFemale = isFemale;


                            int c = db.SaveChanges();

                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorDatabaseManager.AddException(ex, GetType(), additionalInformation: "\n   Bout=" + boutId);
                    }
                }

            }
            catch (Exception ex)
            {

                ErrorDatabaseManager.AddException(ex, GetType());

                return false; ;
            }

            return true;
        }


        public bool SaveSeasonsData(string seasonsJsonUrl)
        {

            DataModels.Imports.Rinxter.RSeasons seasion = new DataModels.Imports.Rinxter.RSeasons();

            Dictionary<string, string>[] seasions = WebProvider.Download_serialized_json_seasion_data(seasonsJsonUrl);

            try
            {
                var db = new ManagementContext();

                foreach (var item in seasions)
                {
                    string seasionValue = item["Value"];

                    // get Seasion by value from the database 
                    var sesaionByValue = db.RSeasons.Where(x => x.Value == seasionValue).FirstOrDefault();

                    //Checking if that seasion already exists in the database
                    if (sesaionByValue == null)
                    {
                        seasion.Value = seasionValue;
                        seasion.Text = item["Text"];
                        db.RSeasons.Add(seasion);

                    }

                    int c = db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());

                return false; ;
            }

            return true;
        }


        public bool SaveRegionsData(string regionsJsonUrl)
        {

            DataModels.Imports.Rinxter.RRegions region = new DataModels.Imports.Rinxter.RRegions();

            Dictionary<string, string>[] dicRegions = WebProvider.Download_serialized_json_regions_data(regionsJsonUrl);

            try
            {
                var db = new ManagementContext();

                foreach (var item in dicRegions)
                {
                    string regionValue = item["Value"];

                    // get Regions by value from the database 
                    var regionsByValue = db.RRegions.Where(x => x.Name == regionValue).FirstOrDefault();

                    //Checking if that region already exists in the database
                    if (regionsByValue == null)
                    {
                        region.Name = regionValue;
                        db.RRegions.Add(region);

                        int c = db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDatabaseManager.AddException(ex, GetType());

                return false; ;
            }

            return true;
        }
    }
}
