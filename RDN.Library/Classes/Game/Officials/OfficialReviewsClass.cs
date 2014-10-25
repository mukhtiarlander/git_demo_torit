using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game.Officials;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.ViewModel.Officials;

namespace RDN.Library.Classes.Game.Officials
{
    internal class OfficialReviewsClass
    {
        public static void AddOfficialReview(GameViewModel game, DataModels.Game.Game gameNew, OfficialReviewViewModel review)
        {
            try
            {
                GameOfficialReview or = new GameOfficialReview();
                or.CurrentDateTimeReviewed = review.CurrentDateTimeReviewed;
                or.Details = review.Details;
                or.Game = gameNew;
                or.JamId = review.JamId;
                or.JamNumber = review.JamNumber;
                or.OfficialReviewIdFromGame = review.OfficialReviewId;
                or.Period = review.Period;
                or.PeriodTimeRemaining = review.PeriodTimeRemaining;
                or.Result = review.Result;
                or.TeamNumber = (byte)review.TeamNumber;
                gameNew.OfficialReviews.Add(or);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static int DeepCompareOfficialReviewsToDb(GameViewModel game, ManagementContext db, DataModels.Game.Game gameNew)
        {
            try
            {
                if (game.OfficialReviews != null)
                {
                    foreach (var member in game.OfficialReviews)
                    {
                        var review = gameNew.OfficialReviews.Where(x => x.OfficialReviewIdFromGame == member.OfficialReviewId).FirstOrDefault();
                        if (review == null)
                            AddOfficialReview(game, gameNew, member);
                        else
                        {
                            review.TeamNumber = (byte)member.TeamNumber;
                            review.Details = member.Details;
                            review.Result = member.Result;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return db.SaveChanges();
        }
    }
}
