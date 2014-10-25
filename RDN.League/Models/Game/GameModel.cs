using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scoreboard.Library.ViewModel;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.Web.Caching;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Scoreboard.Library.ViewModel.Members;

namespace RDN.League.Models.Game
{
    public class GameModel : GameViewModel
    {

        public void CreateNewGameForAddingToDb()
        {
            this.GameId = Guid.NewGuid();
            this.GameName = "Scrimmage";
            this.ScoreboardMode = ScoreboardModeEnum.AddingOldGame;
            this.IsGameOnline = GameViewModelIsOnlineEnum.IsOnline;
            this.PublishGameOnline = false;
            this.HasGameEnded = true;
            this.HasGameStarted = true;
            this.GameDate = DateTime.UtcNow;
            this.Advertisements = new List<AdvertisementViewModel>();
            this.Team1.TeamName = "Home";
            this.Team1.TeamId = Guid.NewGuid();
            this.Team2.TeamName = "Away";
            this.Team2.TeamId = Guid.NewGuid();
            this.TimeOuts = new List<TimeOutViewModel>();
            this.ScoresTeam1 = new List<ScoreViewModel>();
            this.ScoresTeam2 = new List<ScoreViewModel>();
            this.BlocksForTeam1 = new List<BlockViewModel>();
            this.BlocksForTeam2 = new List<BlockViewModel>();
            this.AssistsForTeam1 = new List<AssistViewModel>();
            this.AssistsForTeam2 = new List<AssistViewModel>();
            this.GameLinks = new List<GameLinkViewModel>();
        }
        public SelectList ListOfTeams { get; set; }

        /// <summary>
        /// temp list of team members so we can move these members to the real list of team members that played in the game
        /// </summary>
        public ObservableCollection<TeamMembersViewModel> TempTeam1Members { get; set; }
        /// <summary>
        /// temp list of team members so we can move these members to the real list of team members that played in the game
        /// </summary>
        public ObservableCollection<TeamMembersViewModel> TempTeam2Members { get; set; }

        public TeamViewModel Team1AttachedToGame { get; set; }
        public TeamViewModel Team2AttachedToGame { get; set; }

    }
}