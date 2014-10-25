using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using RDN.Library.DataModels.Context;
using RDN.Library.DataModels.Game;
using RDN.Library.Classes.Account.Classes;
using Scoreboard.Library.ViewModel;
using Scoreboard.Library.Static.Enums;
using Scoreboard.Library.ViewModel.Members;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Game.Tournaments;
using RDN.Library.DataModels.ContactCard;
using System.IO;
using System.Drawing;
using RDN.Library.Classes.Game.Enums;
using RDN.Library.Classes.Payment.Paywall;
using RDN.Library.Classes.Payment.Enums.Paywall;
using RDN.Library.Classes.Payment.Enums;
using RDN.Library.Classes.Payment.Classes.Display;
using RDN.Library.Classes.Communications;
using RDN.Library.Classes.Game.Tournaments;
using TournamentApi = Tournaments;
using RDN.Portable.Classes.Imaging;

namespace RDN.Library.Classes.Game
{
    public class TournamentJson
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartDateDisplay { get; set; }
        public string EndDateDisplay { get; set; }
        public Guid Id { get; set; }
        public string TournamentWebsite { get; set; }
        public string Country { get; set; }
        public int CountryId { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public List<TeamViewModel> TeamsForTournament { get; set; }
        public List<TournamentApi.TournamentRanking> Rankings { get; set; }
        public List<TournamentRoundsClass> TournamentRounds { get; set; }
        public List<TournamentApi.TournamentRanking> RankingsPerformance { get; set; }
        public List<TournamentRoundsClass> TournamentRoundsPerformance { get; set; }
        public List<Game> Games { get; set; }
        public List<PhotoItem> Photos { get; set; }
        public TournamentClassEnum TournamentClass { get; set; }
        public string TournamentClassType { get; set; }
        public string RenderBracketUrl { get; set; }
        public string RenderPerformanceBracketUrl { get; set; }
        public string TournamentType { get; set; }
        public string TournamentTypePerformance { get; set; }

        public List<TeamMembersViewModel> AllSkaters { get; set; }

        public TournamentJson()
        {
            Games = new List<Game>();
            AllSkaters = new List<TeamMembersViewModel>();
            Photos = new List<PhotoItem>();
            TournamentRounds = new List<TournamentRoundsClass>();
            TournamentRoundsPerformance = new List<TournamentRoundsClass>();
            TeamsForTournament = new List<TeamViewModel>();
            RankingsPerformance = new List<TournamentApi.TournamentRanking>();
        }

        public TournamentJson(Tournament tournament)
        {
            Name = tournament.Name;
            StartDate = tournament.StartDate;
            EndDate = tournament.EndDate;
            StartDateDisplay = tournament.StartDateDisplay;
            EndDateDisplay = tournament.EndDateDisplay;
            Id = tournament.Id;
            TournamentWebsite = tournament.TournamentWebsite;
            Country = tournament.Country;
            CountryId = tournament.CountryId;
            State = tournament.State;
            Address = tournament.Address;
            Address2 = tournament.Address2;
            City = tournament.City;
            ZipCode = tournament.ZipCode;
            Games = tournament.Games;
            Photos = tournament.Photos;
            TournamentClass = tournament.TournamentClass;
            TournamentClassType = tournament.TournamentClass.ToString();
            AllSkaters = tournament.AllSkaters;
            TournamentRounds = tournament.TournamentRounds;
            Rankings = tournament.Rankings;
            RankingsPerformance = tournament.RankingsForSeededRounds;
            TournamentRoundsPerformance = tournament.TournamentRoundsForSeedingGameplay;
            TeamsForTournament = tournament.TeamsForTournament;
            RenderBracketUrl = tournament.RenderUrl;
            RenderPerformanceBracketUrl = tournament.RenderPerformanceUrl;
            TournamentType = tournament.TournamentType.ToString();
            TournamentTypePerformance = tournament.TouramentTypeForSeedingEnum.ToString();
        }


    }
}
