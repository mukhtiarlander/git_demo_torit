using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.Classes.Game.Enums;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game;
using RDN.Library.DataModels.Game.Officials;

using Scoreboard.Library.ViewModel;
using Scoreboard.Library.ViewModel.Officials.Enums;

namespace RDN.Library.Classes.Game.Officials
{
    internal class GameOfficialsClass
    {

        public static int DeepCompareOfficialsToDb(GameViewModel game, ManagementContext db, DataModels.Game.Game gameNew)
        {
            try
            {
                if (game.Officials != null)
                {
                    foreach (var member in game.Officials.Nsos)
                    {
                        UpdateOfficialToDb(game, db, gameNew, member.SkaterId, member.SkaterName, OfficialTypeEnum.NSO, Convert.ToInt32(member.RefereeType), member.Cert);
                    }
                    foreach (var member in game.Officials.Referees)
                    {
                        UpdateOfficialToDb(game, db, gameNew, member.SkaterId, member.SkaterName, OfficialTypeEnum.Referee, Convert.ToInt32(member.RefereeType), member.Cert);
                    }

                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return db.SaveChanges();
        }

        private static void UpdateOfficialToDb(GameViewModel game, ManagementContext db, DataModels.Game.Game gameNew, Guid skaterId, string skaterName, OfficialTypeEnum officialType, int refereeType, CertificationLevelEnum cert)
        {
            try
            {
                var off = db.GameOfficials.Where(x => x.GameOfficialId == skaterId).FirstOrDefault();
                if (off != null && !String.IsNullOrEmpty(skaterName))
                {
                    off.Game = gameNew;
                    off.MemberName = skaterName;
                    off.OfficialTypeEnum = Convert.ToInt32(officialType);
                    off.RefereeType = refereeType;
                    off.CertificationLevelEnum = (byte)cert;
                }
                else
                {
                    AddOfficialToGame(game, db, gameNew, skaterId, skaterName, officialType, refereeType, cert);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static void AddOfficialToGame(GameViewModel game, ManagementContext db, DataModels.Game.Game gameNew, Guid skaterId, string skaterName, OfficialTypeEnum officialType, int refereeType, CertificationLevelEnum cert)
        {
            try
            {
                if (!String.IsNullOrEmpty(skaterName))
                {
                    GameOfficial o = new GameOfficial();
                    o.Game = gameNew;
                    o.GameOfficialId = skaterId;
                    o.MemberName = skaterName;
                    o.OfficialTypeEnum = Convert.ToInt32(officialType);
                    o.RefereeType = refereeType;
                    o.CertificationLevelEnum = (byte)cert;
                    db.GameOfficials.Add(o);
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }


        public static int AddOfficialsToDb(GameViewModel game, ManagementContext db, DataModels.Game.Game gameNew)
        {
            try
            {
                foreach (var member in game.Officials.Referees)
                {
                    GameOfficialsClass.AddOfficialToGame(game, db, gameNew, member.SkaterId, member.SkaterName, OfficialTypeEnum.Referee, Convert.ToInt32(member.RefereeType), member.Cert);
                }
                foreach (var member in game.Officials.Nsos)
                {
                    GameOfficialsClass.AddOfficialToGame(game, db, gameNew, member.SkaterId, member.SkaterName, OfficialTypeEnum.NSO, Convert.ToInt32(member.RefereeType), member.Cert);
                }
                return db.SaveChanges();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return 0;
        }
    }
}
