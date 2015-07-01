using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Account;
using RDN.Library.Classes.Account.Enums;
using RDN.Library.Classes.Team.Classes;
using RDN.Library.Classes.Team.Enums;
using RDN.Library.DataModels.Context;
using System.Web;
using System.IO;
using RDN.Utilities.Config;
using RDN.Library.DataModels.Team;
using RDN.Utilities.Error;
using System.Data.Entity;
using System.Drawing;
using RDN.Portable.Classes.Team;
using RDN.Library.Classes.Config;

namespace RDN.Library.Classes.Team
{
    public class TeamFactory
    {


        public List<RDN.Portable.Classes.Team.TeamLogo> GetAllScoreboardLogos(TeamDisplay team)
        {
            try
            {
                if (team.ScoreboardLogos == null)
                {
                    var dc = new ManagementContext();
                    var logos = dc.TeamLogos.AsParallel().OrderBy(x => x.TeamName).ToList();
                    for (int i = 0; i < logos.Count; i++)
                    {
                        team.ScoreboardLogos.Add(new RDN.Portable.Classes.Team.TeamLogo()
                        {
                            Height = logos[i].Height.ToString(),
                            ImageUrl = logos[i].ImageUrl,
                            ImageUrlThumb = logos[i].ImageUrlThumb,
                            SaveLocation = logos[i].SaveLocation,
                            TeamLogoId = logos[i].TeamLogoId,
                            TeamName = logos[i].TeamName,
                            Width = logos[i].Width.ToString()

                        });
                    }
                }

                return team.ScoreboardLogos;
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }

        /// <summary>
        /// gets all the logos in the DB.
        /// </summary>
        /// <returns></returns>
        public List<RDN.Portable.Classes.Team.TeamLogo> GetAllLogos(TeamDisplay team)
        {
            try
            {
                if (team.LeagueLogos == null || team.LeagueLogos.Count == 0)
                {
                    var dc = new ManagementContext();
                    if (team.ScoreboardLogos == null || team.ScoreboardLogos.Count == 0)
                        team.ScoreboardLogos = GetAllScoreboardLogos(team);

                    var leagues = dc.Leagues.Where(x => x.Logo != null).AsParallel().ToList();
                    foreach (var league in leagues)
                    {
                        RDN.Portable.Classes.Team.TeamLogo logo = new RDN.Portable.Classes.Team.TeamLogo();
                        logo.ImageUrl = league.Logo.ImageUrl;
                        logo.TeamName = league.Name;
                        logo.TeamLogoId = league.Logo.LeaguePhotoId;
                        team.LeagueLogos.Add(logo);
                    }
                    team.LeagueLogos = team.LeagueLogos.OrderBy(x => x.TeamName).ToList();
                }
                return team.LeagueLogos;
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
            return null;
        }
        /// <summary>
        /// updates the team name for the logo
        /// </summary>
        /// <param name="logoId"></param>
        /// <param name="teamName"></param>
        public static void UpdateTeamNameLogo(Guid logoId, string teamName)
        {
            try
            {
                var dc = new ManagementContext();
                var logos = dc.TeamLogos.Where(x => x.TeamLogoId == logoId).FirstOrDefault();
                logos.TeamName = teamName;
                dc.SaveChanges();
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
        }
        public static void MergeLogos(IEnumerable<string> logoIds)
        {
            try
            {
                string mainLogo = logoIds.FirstOrDefault();
                Guid logoId = new Guid(mainLogo);
                var dc = new ManagementContext();
                var main = dc.TeamLogos.Where(x => x.TeamLogoId == logoId).FirstOrDefault();

                foreach (var l in logoIds)
                {
                    Guid lId = new Guid(l);
                    if (lId != logoId)
                    {
                        var games = dc.GameTeam.Where(x => x.Logo.TeamLogoId == lId);
                        foreach (var game in games)
                        {
                            game.Logo = main;
                        }

                        int c = dc.SaveChanges();
                        if (c > 0 || games.Count() == 0)
                            DeleteLogoFromLogos(lId);
                    }
                }


                dc.SaveChanges();
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
        }
        public static void DeleteLogoFromLogos(Guid logoId)
        {
            try
            {
                var dc = new ManagementContext();
                var logos = dc.TeamLogos.Where(x => x.TeamLogoId == logoId).FirstOrDefault();
                if (logos != null)
                {
                    if (!String.IsNullOrEmpty(logos.SaveLocation))
                    {
                        FileInfo file = new FileInfo(logos.SaveLocation);
                        if (file.Exists)
                            file.Delete();
                    }
                }
                dc.TeamLogos.Remove(logos);
                dc.SaveChanges();
            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
        }

        /// <summary>
        /// saves the posted file to the DB and the directory.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="context"></param>
        /// <returns>The Id of the logo</returns>
        public static Guid SaveLogoToDbForTeam(HttpPostedFileBase file, Guid teamId, string teamName)
        {
            try
            {
                //time stamp for the save location
                DateTime timeOfSave = DateTime.UtcNow;
                FileInfo info = new FileInfo(file.FileName);
                //the file name when we save it
                string url = LibraryConfig.ImagesBaseUrl + "/leagues/" + timeOfSave.Year + "/" + timeOfSave.Month + "/" + timeOfSave.Day + "/";
                string imageLocationToSave = LibraryConfig.ImagesBaseSaveLocation + @"\leagues\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";
                //creates the directory for the image
                if (!Directory.Exists(imageLocationToSave))
                    Directory.CreateDirectory(imageLocationToSave);

                string fileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(teamName + " roller derby logo-") + timeOfSave.ToFileTimeUtc() + info.Extension;
                string fileNameThumb = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(teamName + " roller derby logo-thumb-") + timeOfSave.ToFileTimeUtc() + info.Extension;

                string urlMain = url + fileName;
                string urlThumb = url + fileNameThumb;
                string imageLocationToSaveMain = imageLocationToSave + fileName;
                string imageLocationToSaveThumb = imageLocationToSave + fileNameThumb;

                string dtSaved = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var dc = new ManagementContext();
                RDN.Library.DataModels.Team.Team team = null;
                if (teamId != new Guid())
                    team = dc.Teams.Where(x => x.TeamId == teamId).FirstOrDefault();


                var teamLogo = new RDN.Library.DataModels.Team.TeamLogo
                     {
                         TeamName = teamName,
                         SaveLocation = imageLocationToSaveMain,
                         ImageUrl = urlMain,
                         Created = DateTime.UtcNow,
                         ImageUrlThumb = urlThumb,
                         SaveLocationThumb = imageLocationToSaveThumb,
                         LastModified = DateTime.UtcNow,
                     };

                if (team != null)
                    team.Logos.Add(teamLogo);
                else
                {
                    if (teamId != new Guid())
                    {
                        var teamGame = dc.GameTeam.Where(x => x.TeamId == teamId).FirstOrDefault();
                        if (teamGame != null)
                            teamGame.Logo = teamLogo;
                    }
                }

                dc.TeamLogos.Add(teamLogo); // Assign the add back to the variables. Once you have saved this, it will be filled with the id of the team logo.
                var result = dc.SaveChanges();

                using (var newfileStream = new FileStream(imageLocationToSaveMain, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    file.InputStream.CopyTo(newfileStream);
                }
                Image thumbImg = Image.FromStream(file.InputStream);
                Image thumb = RDN.Utilities.Drawing.Images.ScaleDownImage(thumbImg, 300, 300);
                thumb.Save(imageLocationToSaveThumb);

                return teamLogo.TeamLogoId;

            }
            catch (Exception e)
            {
                Library.Classes.Error.ErrorDatabaseManager.AddException(e, e.GetType(), ErrorGroupEnum.Database);
            }
            return new Guid();
        }


    }
}
