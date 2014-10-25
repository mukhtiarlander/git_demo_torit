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
using TournamentApi = Tournaments;
using RDN.Library.Classes.Game.Tournaments;
using Tournaments.Standard;
using RDN.Portable.Config;
using RDN.Portable.Classes.Account.Classes;
using RDN.Portable.Classes.Payment.Enums;
using RDN.Portable.Classes.Imaging;
using RDN.Portable.Classes.ContactCard.Enums;
using RDN.Portable.Classes.Games.Tournament;
//using SvgNet.SvgGdi;

namespace RDN.Library.Classes.Game
{
    public class Tournament : TournamentBase
    {
        
        public string StartDateDisplay { get; set; }
        public string EndDateDisplay { get; set; }
        public bool IsPublished { get; set; }
        public bool AreBracketsPublished { get; set; }
        
        public Guid PrivateKey { get; set; }
        public string PassCodeForGames { get; set; }
        public string EmbedVideoString { get; set; }
        public long PaywallId { get; set; }
        public Paywall Paywall { get; set; }
        public string SelectedShop { get; set; }
        public List<OverviewMerchant> AvailableShops { get; set; }

        [Obsolete("User Owners")]
        public MemberDisplayBasic Owner { get; set; }
        public List<MemberDisplayBasic> Owners { get; set; }
        public string TournamentWebsite { get; set; }
        public string Country { get; set; }
        public int CountryId { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }

        public List<Game> Games { get; set; }
        public List<PhotoItem> Photos { get; set; }
        public TournamentClassEnum TournamentClass { get; set; }
        public TournamentTypeEnum TournamentType { get; set; }
        public string StripeKey { get; set; }
        public List<TeamViewModel> TeamsForTournament { get; set; }
        public List<TournamentApi.TournamentTeam> TeamsForTournamentApi { get; set; }
        public List<TournamentApi.TournamentRanking> Rankings { get; set; }
        public List<TournamentRoundsClass> TournamentRounds { get; set; }
        public List<TournamentApi.TournamentRound> TournamentRoundsApi { get; set; }
        public bool HasTournamentStarted { get; set; }

        public bool AreSeedingBracketsRequired { get; set; }

        public List<TournamentApi.TournamentRanking> RankingsForSeededRounds { get; set; }
        public List<TournamentRoundsClass> TournamentRoundsForSeedingGameplay { get; set; }
        public List<TournamentApi.TournamentRound> TournamentRoundsApiForSeeding { get; set; }
        public TournamentTypeEnum TouramentTypeForSeedingEnum { get; set; }
        public bool HasSeedingStartedForTournament { get; set; }
        public bool HasSeedingFinishForTournament { get; set; }


        public List<AssistViewModel> AssistLeaders { get; set; }
        public List<BlockViewModel> BlockLeaders { get; set; }
        public List<PenaltyViewModel> PenaltyLeaders { get; set; }
        public List<TeamMembersViewModel> AllSkaters { get; set; }
        public TournamentApi.ITournamentVisualizer VisualizedBrackets { get; set; }
        public TournamentApi.ITournamentVisualizer VisualizedBracketsSeeded { get; set; }
        public bool IsTournamentFinished { get; set; }
        public bool CanRenderView { get; set; }
        public Size? RenderedSize { get; set; }
        public string RenderUrl { get; set; }
        public string RenderPerformanceUrl { get; set; }
        public Tournament()
        {
            Owners = new List<MemberDisplayBasic>();
            Paywall = new Paywall();
            Games = new List<Game>();
            AssistLeaders = new List<AssistViewModel>();
            BlockLeaders = new List<BlockViewModel>();
            PenaltyLeaders = new List<PenaltyViewModel>();
            AllSkaters = new List<TeamMembersViewModel>();
            Photos = new List<PhotoItem>();
            TeamsForTournament = new List<TeamViewModel>();
            TournamentRounds = new List<TournamentRoundsClass>();
            Rankings = new List<TournamentApi.TournamentRanking>();
            TeamsForTournamentApi = new List<TournamentApi.TournamentTeam>();
            TournamentRoundsApi = new List<TournamentApi.TournamentRound>();
            RankingsForSeededRounds = new List<TournamentApi.TournamentRanking>();
            TournamentRoundsForSeedingGameplay = new List<TournamentRoundsClass>();
            TournamentRoundsApiForSeeding = new List<TournamentApi.TournamentRound>();
        }

        public static List<Conversation> GetConversation(Guid tournamentId)
        {
            try
            {
                var dc = new ManagementContext();
                var convo = (from xx in dc.TournamentConversations
                             where xx.Tournament.TournamentId == tournamentId
                             select new Conversation
                             {
                                 Chat = xx.Text,
                                 Id = xx.ConversationId,
                                 MemberName = xx.Owner == null ? "Anonymous" : xx.Owner.DerbyName,
                                 Created = xx.Created,
                                 OwnerId = tournamentId
                             }).OrderByDescending(x => x.Id).Take(30).AsParallel().ToList();
                foreach (var con in convo)
                {
                    con.Time = RDN.Utilities.Dates.DateTimeExt.RelativeDateTime(con.Created);
                }
                return convo;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<Conversation>();
        }

        public static Conversation PostConversationText(Guid tournamentId, string text, Guid memberId)
        {
            try
            {
                var dc = new ManagementContext();
                var game = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                TournamentConversation sation = new TournamentConversation();
                sation.Tournament = game;
                sation.Text = text;
                if (memberId != new Guid())
                    sation.Owner = dc.Members.Where(x => x.MemberId == memberId).FirstOrDefault();
                dc.TournamentConversations.Add(sation);
                dc.SaveChanges();
                Conversation s = new Conversation();
                s.Chat = text;
                s.Created = sation.Created;
                s.OwnerId = tournamentId;
                s.Id = sation.ConversationId;
                if (sation.Owner != null)
                    s.MemberName = sation.Owner.DerbyName;
                else
                    s.MemberName = "Anonymous";
                return s;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        /// <summary>
        /// creates a new tournament.
        /// </summary>
        /// <param name="tournamentName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="passCode"></param>
        /// <returns></returns>
        public static Tournament CreateTournament(string tournamentName, DateTime startDate, DateTime endDate, string passCode, Guid ownerMemberId)
        {
            try
            {
                var dc = new ManagementContext();
                GameTournament tourny = new GameTournament();
                tourny.PrivateTournamentId = Guid.NewGuid();
                tourny.StartDate = startDate;
                tourny.TournamentName = tournamentName;
                tourny.TournamentPasscode = passCode;
                tourny.EndDate = endDate;
                tourny.OwnerOfTournament = dc.Members.Where(x => x.MemberId == ownerMemberId).FirstOrDefault();
                dc.GameTournaments.Add(tourny);
                dc.SaveChanges();

                Tournament to = new Tournament();
                to.EndDate = endDate;
                to.StartDate = startDate;
                to.Id = tourny.TournamentId;
                to.PrivateKey = tourny.PrivateTournamentId;
                to.Name = tourny.TournamentName;
                return to;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        public static List<Tournament> GetCurrentTournaments()
        {

            List<Tournament> toReturn = new List<Tournament>();
            try
            {
                var dc = new ManagementContext();
                DateTime start = DateTime.UtcNow.AddDays(-1);
                DateTime end = DateTime.UtcNow.AddDays(1);
                var tournies = dc.GameTournaments.Where(x => x.EndDate > start && x.StartDate < end).OrderBy(x => x.StartDate).AsParallel().Take(3).ToList();
                if (tournies == null)
                    return null;

                foreach (var tourny in tournies)
                {
                    toReturn.Add(DisplayTournament(tourny));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return toReturn;

        }


        public static List<Tournament> GetAllOwnedTournaments(Guid ownerMemberId)
        {

            List<Tournament> toReturn = new List<Tournament>();
            try
            {
                var dc = new ManagementContext();
                var tournies = dc.GameTournaments.Where(x => x.OwnerOfTournament.MemberId == ownerMemberId).ToList();
                var tournies2 = dc.TournamentOwners.Where(x => x.Owner.MemberId == ownerMemberId).ToList();
                if (tournies == null)
                    return null;

                foreach (var tourny in tournies)
                {
                    toReturn.Add(DisplayTournament(tourny));
                }
                foreach (var tourny in tournies2)
                {
                    if (toReturn.Where(x => x.Id == tourny.Tournament.TournamentId).FirstOrDefault() == null)
                        toReturn.Add(DisplayTournament(tourny.Tournament));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return toReturn;

        }
        public static List<Tournament> GetLastCountTournaments(int count)
        {

            List<Tournament> toReturn = new List<Tournament>();
            try
            {
                var dc = new ManagementContext();
                var tournies = dc.GameTournaments.Where(x => x.IsRemoved == false && x.IsPublished == true).OrderBy(x => x.StartDate).Take(count).ToList();
                if (tournies == null)
                    return null;

                foreach (var tourny in tournies)
                {
                    toReturn.Add(DisplayTournament(tourny));
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return toReturn;

        }
        public static Tournament GetTournament(Guid tournId)
        {

            Tournament toReturn = new Tournament();
            try
            {
                var dc = new ManagementContext();
                var tournies = dc.GameTournaments.Where(x => x.TournamentId == tournId && x.IsRemoved == false).FirstOrDefault();
                if (tournies == null)
                    return null;
                return DisplayTournament(tournies);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return toReturn;

        }

        public static bool CheckTournamentPaywallIsPaid(Guid tournamentId, string password)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return false;
                if (tourny.Paywall != null && tourny.Paywall.PaywallInvoices.Count > 0)
                {
                    foreach (var wall in tourny.Paywall.PaywallInvoices)
                    {
                        if (wall.GeneratedPassword == password)
                        {
                            wall.TimesUsedPassword += 1;
                            wall.Invoice = wall.Invoice;
                            dc.SaveChanges();
                            return true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static Tournament GetPublicTournament(Guid tournamentId)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return null;

                return DisplayTournament(tourny);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;

        }
        public static Bitmap RenderTournamentBrackets(Tournament tourny, bool isPerformanceRound)
        {
            try
            {
                var teams = new Dictionary<Guid, string>();
                foreach (var team in from t in tourny.TeamsForTournament
                                     select new KeyValuePair<Guid, string>(t.TeamId, t.TeamName))
                {
                    teams.Add(team.Key, team.Value);
                }
                Bitmap rendered = new Bitmap(1, 1);
                //var g = new SvgNet.SvgGdi.SvgGraphics();
                var grs = new SystemGraphics();
                if (isPerformanceRound)
                {
                    var size = tourny.VisualizedBracketsSeeded.Measure(grs, new TournamentApi.TournamentNameTable(teams));
                    rendered = new Bitmap((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
                    tourny.VisualizedBracketsSeeded.Render(new SystemGraphics(rendered), new TournamentApi.TournamentNameTable(teams));

                }
                else
                {
                    var size = tourny.VisualizedBrackets.Measure(grs, new TournamentApi.TournamentNameTable(teams));
                    rendered = new Bitmap((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
                    tourny.VisualizedBrackets.Render(new SystemGraphics(rendered), new TournamentApi.TournamentNameTable(teams));

                }
                return rendered;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="isPerformanceRound">rendedring the brackets for the performance rounds.</param>
        /// <returns></returns>
        public static Bitmap RenderTournamentBrackets(Guid tournamentId, bool isPerformanceRound)
        {
            try
            {
                var tourny = GetPublicTournament(tournamentId);

                return RenderTournamentBrackets(tourny, isPerformanceRound);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;

        }

        private static Tournament DisplayTournament(GameTournament tourny)
        {
            try
            {
                var memId = RDN.Library.Classes.Account.User.GetMemberId();
                Tournament to = new Tournament();
                to.TournamentClass = (TournamentClassEnum)tourny.TournamentClass;
                to.TournamentType = (TournamentTypeEnum)tourny.TournamentTypeEnum;
                to.EndDate = tourny.EndDate;
                if (tourny.SelectedShop != null)
                    to.SelectedShop = tourny.SelectedShop.MerchantId.ToString();
                to.EndDateDisplay = tourny.EndDate.ToShortDateString();
                to.StartDateDisplay = tourny.StartDate.ToShortDateString();
                to.StartDate = tourny.StartDate;
                to.Id = tourny.TournamentId;
                to.IsPublished = tourny.IsPublished;
                to.AreBracketsPublished = tourny.AreBracketsPublished;
                to.PrivateKey = tourny.PrivateTournamentId;
                to.Name = tourny.TournamentName;
                to.PassCodeForGames = tourny.TournamentPasscode;
                to.EmbedVideoString = tourny.EmbedVideoString;
                to.TournamentWebsite = tourny.TournamentWebsite;

                if (tourny.OwnerOfTournament != null)
                {
                    MemberDisplayBasic m = new MemberDisplayBasic();
                    m.DerbyName = tourny.OwnerOfTournament.DerbyName;
                    m.MemberId = tourny.OwnerOfTournament.MemberId;
                    m.UserId = tourny.OwnerOfTournament.AspNetUserId;
                    to.Owners.Add(m);
                }

                for (int i = 0; i < tourny.OwnersOfTournament.Count; i++)
                {
                    try
                    {
                        if (to.Owners.Where(x => x.MemberId == tourny.OwnersOfTournament[i].Owner.MemberId).FirstOrDefault() == null)
                        {
                            MemberDisplayBasic m = new MemberDisplayBasic();
                            m.DerbyName = tourny.OwnersOfTournament[i].Owner.DerbyName;
                            m.MemberId = tourny.OwnersOfTournament[i].Owner.MemberId;
                            m.UserId = tourny.OwnersOfTournament[i].Owner.AspNetUserId;
                            to.Owners.Add(m);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }


                if (tourny.Logo != null)
                    to.Photos.Add(new PhotoItem(tourny.Logo.ImageUrl, tourny.Logo.ImageUrlThumb, true, tourny.TournamentName));
                #region contact
                if (tourny.ContactCard != null)
                {
                    var add = tourny.ContactCard.Addresses.FirstOrDefault();
                    if (add != null)
                    {
                        to.Address = add.Address1;
                        to.Address2 = add.Address2;
                        to.City = add.CityRaw;
                        if (add.Country != null)
                        {
                            to.Country = add.Country.Code;
                            to.CountryId = add.Country.CountryId;
                        }
                        to.State = add.StateRaw;
                        to.ZipCode = add.Zip;
                    }
                }

                #endregion
                #region paywall
                if (tourny.Paywall != null)
                {
                    to.Paywall.DailyPrice = tourny.Paywall.DailyPrice;
                    to.Paywall.DescriptionOfPaywall = tourny.Paywall.DescriptionOfPaywall;
                    to.Paywall.EndDate = tourny.Paywall.EndDate;
                    if (tourny.Paywall.EndDate.HasValue)
                        to.Paywall.EndDateDisplay = tourny.Paywall.EndDate.Value.ToShortDateString();
                    to.Paywall.IsEnabled = tourny.Paywall.IsEnabled;
                    to.Paywall.IsRemoved = tourny.Paywall.IsRemoved;
                    to.Paywall.PaywallId = tourny.Paywall.PaywallId;
                    to.Paywall.StartDate = tourny.Paywall.StartDate;
                    if (tourny.Paywall.StartDate.HasValue)
                        to.Paywall.StartDateDisplay = tourny.Paywall.StartDate.Value.ToShortDateString();
                    to.Paywall.TimespanPrice = tourny.Paywall.TimespanPrice;
                    to.PaywallId = tourny.Paywall.PaywallId;
                    to.Paywall.AcceptPaypal = tourny.Paywall.Merchant.AcceptPaymentsViaPaypal;
                    to.Paywall.AcceptStripe = tourny.Paywall.Merchant.AcceptPaymentsViaStripe;
                    if (memId != new Guid())
                    {
                        var isPaid = tourny.Paywall.PaywallInvoices.Where(x => x.MemberPaidId == memId).OrderByDescending(x => x.Created).FirstOrDefault();
                        if (isPaid != null)
                        {
                            if (isPaid.ValidUntil > DateTime.UtcNow && (isPaid.Invoice.InvoiceStatus == (byte)InvoiceStatus.Payment_Successful || isPaid.Invoice.InvoiceStatus == (byte)InvoiceStatus.Pending_Payment_From_Paypal))
                            {
                                to.Paywall.IsPaid = true;
                            }
                        }
                        else
                            to.Paywall.IsPaid = false;
                    }
                    to.Paywall.MerchantId = tourny.Paywall.Merchant.MerchantId;
                }

                #endregion
                #region Games
                List<Game> games = new List<Game>();
                foreach (var game in tourny.Games)
                {
                    try
                    {
                        Game g = new Game();
                        g.GameId = game.GameId;
                        g.GameName = game.GameName;
                        g.GameDate = game.GameDate;
                        var teams = game.GameTeams.OrderByDescending(x => x.Created).Take(2);
                        var team1 = teams.OrderByDescending(x => x.TeamName).FirstOrDefault();
                        var team2 = teams.OrderBy(x => x.TeamName).FirstOrDefault();
                        g.Team1Name = team1.TeamName;
                        g.Team1LinkId = team1.TeamIdLink;
                        g.Team1Id = team1.TeamId;
                        g.Team2Name = team2.TeamName;
                        g.Team2LinkId = team2.TeamIdLink;
                        g.Team2Id = team2.TeamId;
                        g.Team1ScoreTotal = team1.GameScores.Select(x => x.Point).Sum();
                        g.Team2ScoreTotal = team2.GameScores.Select(x => x.Point).Sum();

                        foreach (var b in team1.GameMembers)
                        {
                            TeamMembersViewModel a = new TeamMembersViewModel();
                            a.SkaterId = b.GameMemberId;
                            a.SkaterLinkId = b.MemberLinkId;
                            a.SkaterName = b.MemberName;
                            a.SkaterNumber = b.MemberNumber;
                            if (to.AllSkaters.Where(x => x.SkaterLinkId == a.SkaterLinkId).FirstOrDefault() == null)
                                to.AllSkaters.Add(a);
                        }
                        foreach (var b in team2.GameMembers)
                        {
                            TeamMembersViewModel a = new TeamMembersViewModel();
                            a.SkaterId = b.GameMemberId;
                            a.SkaterLinkId = b.MemberLinkId;
                            a.SkaterName = b.MemberName;
                            a.SkaterNumber = b.MemberNumber;
                            if (to.AllSkaters.Where(x => x.SkaterLinkId == a.SkaterLinkId).FirstOrDefault() == null)
                                to.AllSkaters.Add(a);

                        }

                        foreach (var assist in game.GameMemberAssists)
                        {
                            AssistViewModel a = new AssistViewModel();
                            a.AssistId = assist.GameAssistId;
                            a.PlayerWhoAssisted = new TeamMembersViewModel();
                            a.PlayerWhoAssisted.SkaterId = assist.MemberWhoAssisted.GameMemberId;
                            a.PlayerWhoAssisted.SkaterName = assist.MemberWhoAssisted.MemberName;
                            a.PlayerWhoAssisted.SkaterLinkId = assist.MemberWhoAssisted.MemberLinkId;
                            to.AssistLeaders.Add(a);
                        }
                        foreach (var pen in game.GameMemberPenalties)
                        {
                            PenaltyViewModel a = new PenaltyViewModel();
                            a.PenaltyId = pen.GamePenaltyId;
                            a.PenaltyAgainstMember = new TeamMembersViewModel();
                            a.PenaltyAgainstMember.SkaterId = pen.MemberWhoPenaltied.GameMemberId;
                            a.PenaltyAgainstMember.SkaterName = pen.MemberWhoPenaltied.MemberName;
                            a.PenaltyAgainstMember.SkaterLinkId = pen.MemberWhoPenaltied.MemberLinkId;
                            a.PenaltyType = (PenaltiesEnum)Enum.Parse(typeof(PenaltiesEnum), pen.PenaltyType.ToString());
                            to.PenaltyLeaders.Add(a);
                        }
                        foreach (var blocks in game.GameMemberBlocks)
                        {
                            BlockViewModel a = new BlockViewModel();
                            a.BlockId = blocks.GameBlockId;
                            a.PlayerWhoBlocked = new TeamMembersViewModel();
                            a.PlayerWhoBlocked.SkaterId = blocks.MemberWhoBlocked.GameMemberId;
                            a.PlayerWhoBlocked.SkaterName = blocks.MemberWhoBlocked.MemberName;
                            a.PlayerWhoBlocked.SkaterLinkId = blocks.MemberWhoBlocked.MemberLinkId;
                            to.BlockLeaders.Add(a);
                        }
                        games.Add(g);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                to.Games = games;

                #endregion

                var ts = tourny.Teams.Where(x => x.IsRemoved == false);
                foreach (var team in ts)
                {
                    try
                    {
                        TeamViewModel t = new TeamViewModel();
                        t.PoolNumber = team.PoolNumber;
                        t.LeagueName = team.LeageName;
                        //t.TeamLinkId = team.TeamIdLink;
                        t.TeamName = team.TeamName;
                        t.TeamId = team.TeamId;
                        t.SeedRating = team.SeedRating;
                        to.TeamsForTournament.Add(t);
                        TournamentApi.TournamentTeam tt = new TournamentApi.TournamentTeam(team.TeamId, team.SeedRating, team.PoolNumber);
                        to.TeamsForTournamentApi.Add(tt);
                    }
                    catch (Exception exception)
                    {
                        ErrorDatabaseManager.AddException(exception, exception.GetType());
                    }
                }

                #region seededbracketsForPerformance
                try
                {
                    to.TouramentTypeForSeedingEnum = (TournamentTypeEnum)tourny.TournamentTypeEnumForSeedingRound;
                    if (to.TouramentTypeForSeedingEnum != TournamentTypeEnum.None)
                    {

                        TournamentApi.IPairingsGenerator pg = null;

                        switch (to.TouramentTypeForSeedingEnum)
                        {
                            case TournamentTypeEnum.Boil_Off:
                                pg = new BoilOffPairingsGenerator();
                                break;
                            case TournamentTypeEnum.Round_Robin:
                                pg = new RoundRobinPairingsGenerator();
                                break;
                            case TournamentTypeEnum.Double_Elimination:
                                pg = new EliminationTournament(2);
                                break;
                            case TournamentTypeEnum.Single_Elimination:
                                pg = new EliminationTournament(1);
                                break;
                            case TournamentTypeEnum.Round_Robin_Pool_Play:
                                pg = new RoundRobinPairingsGenerator(true);
                                break;
                        }
                        if (pg is TournamentApi.ITournamentVisualizer)
                        {
                            to.VisualizedBracketsSeeded = (TournamentApi.ITournamentVisualizer)pg;
                        }

                        foreach (var round in tourny.SeedingRounds)
                        {
                            try
                            {
                                TournamentRoundsClass r = new TournamentRoundsClass();
                                List<TournamentApi.TournamentPairing> ppList = new List<TournamentApi.TournamentPairing>();
                                r.RoundNumber = round.RoundNumber;
                                foreach (var pair in round.Pairings)
                                {
                                    List<TournamentApi.TournamentTeamScore> ttList = new List<TournamentApi.TournamentTeamScore>();
                                    TournamentPairingClass p = new TournamentPairingClass();
                                    p.Id = pair.PairingId;
                                    p.GroupId = pair.GroupId;
                                    p.TimeToStart = pair.StartTime.GetValueOrDefault();
                                    if (pair.StartTime.HasValue)
                                        p.TimeToStartDisplay = p.TimeToStart.ToString("HH:mm");
                                    p.TrackId = pair.TrackNumber;
                                    foreach (var team in pair.Teams)
                                    {
                                        TeamViewModel t = new TeamViewModel();
                                        t.TeamId = team.Team.TeamId;
                                        t.TeamName = team.Team.TeamName;
                                        t.TeamLinkId = team.TeamIdInGame;
                                        if (t.TeamLinkId != new Guid() && p.GameId == new Guid())
                                        {
                                            var g = to.Games.Where(x => x.Team1Id == t.TeamLinkId || x.Team2Id == t.TeamLinkId).FirstOrDefault();
                                            if (g != null)
                                                p.GameId = g.GameId;
                                        }
                                        t.Score = team.Score;
                                        p.Teams.Add(t);

                                        TournamentApi.TournamentTeamScore tt = new TournamentApi.TournamentTeamScore(new TournamentApi.TournamentTeam(t.TeamId, team.Team.SeedRating), new TournamentApi.HighestPointsScore(team.Score));
                                        ttList.Add(tt);

                                    }
                                    TournamentApi.TournamentPairing pp = new TournamentApi.TournamentPairing(ttList);
                                    pp.GroupId = p.GroupId;
                                    ppList.Add(pp);
                                    r.Pairings.Add(p);
                                }

                                TournamentApi.TournamentRound rr = new TournamentApi.TournamentRound(ppList);
                                to.TournamentRoundsApiForSeeding.Add(rr);
                                to.TournamentRoundsForSeedingGameplay.Add(r);
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }

                        if (pg != null)
                        {
                            pg.LoadState(to.TeamsForTournamentApi, to.TournamentRoundsApiForSeeding);
                            try
                            {
                                var nextRound = pg.CreateNextRound(null);

                                to.HasSeedingFinishForTournament = (nextRound == null) && (tourny.SeedingRounds.Count() > 1);

                                if (to.TournamentRoundsApiForSeeding.Any())
                                {
                                    var rankings = pg.GenerateRankings();

                                    to.RankingsForSeededRounds = new List<TournamentApi.TournamentRanking>(from rk in rankings
                                                                                                           select new TournamentApi.TournamentRanking
                                                                                                           {
                                                                                                               TeamName = to.TeamsForTournament.Where(tm => tm.TeamId == rk.team.TeamId).Single().TeamName,
                                                                                                               rank = rk.rank,
                                                                                                               Loses = rk.Loses,
                                                                                                               PointSpread = rk.PointSpread,
                                                                                                               scoreDescription = rk.scoreDescription,
                                                                                                               team = rk.team,
                                                                                                               TotalPoints = rk.TotalPoints,
                                                                                                               Wins = rk.Wins
                                                                                                           });
                                }

                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }

                #endregion


                #region brackets
                try
                {
                    to.TournamentType = (TournamentTypeEnum)tourny.TournamentTypeEnum;
                    if (to.TournamentType != TournamentTypeEnum.None)
                    {
                        if (to.TouramentTypeForSeedingEnum != TournamentTypeEnum.None && to.HasSeedingFinishForTournament)
                        {
                            foreach (var ranking in to.RankingsForSeededRounds)
                            {
                                var team = to.TeamsForTournamentApi.Where(x => x.TeamId == ranking.team.TeamId).FirstOrDefault();
                                team.Rating = (int)ranking.rank;
                            }
                        }


                        TournamentApi.IPairingsGenerator pg = null;

                        switch (to.TournamentType)
                        {
                            case TournamentTypeEnum.Boil_Off:
                                pg = new BoilOffPairingsGenerator();
                                break;
                            case TournamentTypeEnum.Round_Robin:
                                pg = new RoundRobinPairingsGenerator();
                                break;
                            case TournamentTypeEnum.Round_Robin_Pool_Play:
                                pg = new RoundRobinPairingsGenerator(true);
                                break;
                            case TournamentTypeEnum.Double_Elimination:
                                pg = new EliminationTournament(2);
                                break;
                            case TournamentTypeEnum.Single_Elimination:
                                pg = new EliminationTournament(1);
                                break;
                        }
                        try
                        {
                            if (pg is TournamentApi.ITournamentVisualizer)
                            {
                                to.VisualizedBrackets = (TournamentApi.ITournamentVisualizer)pg;
                            }
                        }
                        catch //(TournamentApi.InvalidTournamentStateException ex)
                        {
                            //Debug.WriteLine(ex);
                        }

                        foreach (var round in tourny.Rounds)
                        {
                            try
                            {
                                TournamentRoundsClass r = new TournamentRoundsClass();
                                List<TournamentApi.TournamentPairing> ppList = new List<TournamentApi.TournamentPairing>();
                                r.RoundNumber = round.RoundNumber;
                                foreach (var pair in round.Pairings)
                                {
                                    List<TournamentApi.TournamentTeamScore> ttList = new List<TournamentApi.TournamentTeamScore>();
                                    TournamentPairingClass p = new TournamentPairingClass();
                                    p.Id = pair.PairingId;
                                    p.GroupId = pair.GroupId;
                                    p.TimeToStart = pair.StartTime.GetValueOrDefault();
                                    if (pair.StartTime.HasValue)
                                        p.TimeToStartDisplay = p.TimeToStart.ToString("HH:mm");
                                    p.TrackId = pair.TrackNumber;
                                    foreach (var team in pair.Teams)
                                    {
                                        TeamViewModel t = new TeamViewModel();
                                        t.TeamId = team.Team.TeamId;
                                        t.TeamName = team.Team.TeamName;
                                        t.TeamLinkId = team.TeamIdInGame;
                                        if (t.TeamLinkId != new Guid() && p.GameId == new Guid())
                                        {
                                            var g = to.Games.Where(x => x.Team1Id == t.TeamLinkId || x.Team2Id == t.TeamLinkId).FirstOrDefault();
                                            if (g != null)
                                                p.GameId = g.GameId;
                                        }
                                        t.Score = team.Score;
                                        p.Teams.Add(t);

                                        TournamentApi.TournamentTeamScore tt = new TournamentApi.TournamentTeamScore(new TournamentApi.TournamentTeam(t.TeamId, team.Team.SeedRating), new TournamentApi.HighestPointsScore(team.Score));
                                        ttList.Add(tt);

                                    }
                                    TournamentApi.TournamentPairing pp = new TournamentApi.TournamentPairing(ttList);
                                    pp.GroupId = p.GroupId;
                                    ppList.Add(pp);
                                    r.Pairings.Add(p);
                                }

                                TournamentApi.TournamentRound rr = new TournamentApi.TournamentRound(ppList);
                                to.TournamentRoundsApi.Add(rr);
                                to.TournamentRounds.Add(r);
                            }
                            catch (Exception exception)
                            {
                                ErrorDatabaseManager.AddException(exception, exception.GetType());
                            }
                        }

                        if (pg != null)
                        {
                            pg.LoadState(to.TeamsForTournamentApi, to.TournamentRoundsApi);
                            try
                            {
                                var nextRound = pg.CreateNextRound(null);

                                to.IsTournamentFinished = (nextRound == null) && (tourny.Rounds.Count() > 1);

                                if (to.TournamentRoundsApi.Any())
                                {
                                    var rankings = pg.GenerateRankings();

                                    to.Rankings = new List<TournamentApi.TournamentRanking>(from rk in rankings
                                                                                            select new TournamentApi.TournamentRanking
                                                                                         {
                                                                                             TeamName = to.TeamsForTournament.Where(tm => tm.TeamId == rk.team.TeamId).Single().TeamName,
                                                                                             rank = rk.rank,
                                                                                             Loses = rk.Loses,
                                                                                             PointSpread = rk.PointSpread,
                                                                                             scoreDescription = rk.scoreDescription,
                                                                                             team = rk.team,
                                                                                             TotalPoints = rk.TotalPoints,
                                                                                             Wins = rk.Wins
                                                                                         });
                                }

                            }
                            catch (TournamentApi.InvalidTournamentStateException)
                            {
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ErrorDatabaseManager.AddException(exception, exception.GetType());
                }

                #endregion
                return to;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static bool RemoveTournament(Guid id)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == id).FirstOrDefault();
                if (tourny == null)
                    return false;
                tourny.IsRemoved = true;
                tourny.TournamentName = tourny.TournamentName;
                tourny.StartDate = tourny.StartDate;
                tourny.EndDate = tourny.EndDate;
                tourny.OwnerOfTournament = tourny.OwnerOfTournament;
                tourny.PrivateTournamentId = tourny.PrivateTournamentId;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool UpdateTournamentOwners(Tournament tourn)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tourn.Id && x.PrivateTournamentId == tourn.PrivateKey).FirstOrDefault();
                if (tourny == null)
                    return false;
                foreach (var mem in tourn.Owners)
                {
                    if (mem.MemberId != new Guid())
                    {
                        var member = tourny.OwnersOfTournament.Where(x => x.Owner.MemberId == mem.MemberId).FirstOrDefault();
                        if (member == null)
                        {
                            var memDb = dc.Members.Where(x => x.MemberId == mem.MemberId).FirstOrDefault();
                            if (memDb != null)
                            {
                                TournamentOwner m = new TournamentOwner();
                                m.Tournament = tourny;
                                m.Owner = memDb;
                                m.OwnerType = (byte)GameOwnerEnum.Manager;
                                tourny.OwnersOfTournament.Add(m);

                                if (memDb.AspNetUserId != new Guid())
                                {
                                    var user = System.Web.Security.Membership.GetUser((object)memDb.AspNetUserId);
                                    EmailAccountAddedToManageDerbyGame(tourny, memDb.DerbyName, user.Email);
                                }
                            }
                        }
                    }
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return false;
        }
        private static void EmailAccountAddedToManageDerbyGame(DataModels.Game.Tournaments.GameTournament tournament, string name, string emailToSendTo)
        {
            var emailData = new Dictionary<string, string> { 
                        { "derbyname", name}, 
                        { "gameName",tournament.TournamentName}, 
                        { "link",ServerConfig.WEBSITE_INTERNAL_DEFAULT_LOCATION +"/tournament/view/"+ tournament.PrivateTournamentId.ToString().Replace("-","") +"/"+tournament.TournamentId.ToString().Replace("-","") } };

            EmailServer.EmailServer.SendEmail(ServerConfig.DEFAULT_EMAIL, ServerConfig.DEFAULT_EMAIL_FROM_NAME, emailToSendTo, EmailServer.EmailServer.DEFAULT_SUBJECT + " Added To Manage Tournament", emailData, layout: EmailServer.EmailServerLayoutsEnum.TournamentMemberAddedToManage);
        }

        public static bool UpdateTournament(Tournament tourn)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tourn.Id && x.PrivateTournamentId == tourn.PrivateKey).FirstOrDefault();
                if (tourny == null)
                    return false;
                if (!String.IsNullOrEmpty(tourn.SelectedShop))
                {
                    tourny.SelectedShop = dc.Merchants.Where(x => x.MerchantId == new Guid(tourn.SelectedShop)).FirstOrDefault();
                }
                else
                    tourny.SelectedShop = null;

                tourny.TournamentClass = (byte)tourn.TournamentClass;
                tourny.TournamentTypeEnum = (byte)tourn.TournamentType;
                tourny.EndDate = tourn.EndDate;
                tourny.OwnerOfTournament = tourny.OwnerOfTournament;
                tourny.StartDate = tourn.StartDate;
                tourny.TournamentName = tourn.Name;
                tourny.TournamentPasscode = tourn.PassCodeForGames;
                tourny.EmbedVideoString = tourn.EmbedVideoString;
                tourny.TournamentWebsite = tourn.TournamentWebsite;
                tourny.TournamentTypeEnumForSeedingRound = (byte)tourn.TouramentTypeForSeedingEnum;
                if (tourny.ContactCard == null)
                    tourny.ContactCard = new DataModels.ContactCard.ContactCard();
                var add = tourny.ContactCard.Addresses.FirstOrDefault();

                if (add == null)
                {
                    ContactCard.ContactCardFactory.AddAddressToContact(tourn.Address, tourn.Address2, tourn.City, tourn.State, tourn.ZipCode, AddressTypeEnum.None, tourny.ContactCard, dc.Countries.Where(x => x.CountryId == tourn.CountryId).FirstOrDefault());
                }
                else
                {
                    ContactCard.ContactCardFactory.UpdateAddressToContact(tourn.Address, tourn.Address2, tourn.City, tourn.State, tourn.ZipCode, AddressTypeEnum.None, add, dc.Countries.Where(x => x.CountryId == tourn.CountryId).FirstOrDefault());
                }
                if (tourn.PaywallId > 0)
                    tourny.Paywall = dc.Paywalls.Where(x => x.PaywallId == tourn.PaywallId).FirstOrDefault();
                else
                    tourny.Paywall = null;

                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="isPerformanceRound">if the round is for performance or seeding, otherwise its a regular tournament round</param>
        /// <returns></returns>
        public static bool RollBackRound(Guid tournamentId, bool isPerformanceRound)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return false;
                if (isPerformanceRound)
                {
                    var rs = tourny.SeedingRounds.OrderByDescending(x => x.RoundNumber).FirstOrDefault();
                    dc.TournamentRounds.Remove(rs);
                }
                else
                {
                    var rs = tourny.Rounds.OrderByDescending(x => x.RoundNumber).FirstOrDefault();
                    dc.TournamentRounds.Remove(rs);
                }
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool PublishTournament(Guid tournamentId)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return false;
                tourny.IsPublished = true;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool PublishTournamentBrackets(Guid tournamentId)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return false;
                tourny.AreBracketsPublished = true;
                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool SavePairing(Guid tournamentId, long pairingId, Guid gameId, Guid team1Id, Guid team2Id, int team1Score, int team2Score, string trackNumber, DateTime trackTime)
        {
            try
            {
                var dc = new ManagementContext();
                var pairing = dc.TournamentPairings.Where(x => x.PairingId == pairingId && x.Round.Tournament.TournamentId == tournamentId).FirstOrDefault();
                if (pairing == null)
                    return false;
                pairing.Round = pairing.Round;
                pairing.TrackNumber = trackNumber;
                if (trackTime == new DateTime())
                    pairing.StartTime = null;
                else
                    pairing.StartTime = trackTime;

                var teams = pairing.Teams.OrderByDescending(x => x.Team.TeamName);
                int i = 0;
                foreach (var team in teams)
                {
                    team.Pairing = team.Pairing;
                    team.Team.Tournament = team.Team.Tournament;
                    if (i == 0)
                    {

                        team.Score = team1Score;
                        if (team1Id != new Guid())
                            team.TeamIdInGame = team1Id;
                    }
                    else
                    {
                        team.Score = team2Score;
                        if (team2Id != new Guid())
                            team.TeamIdInGame = team2Id;
                    }
                    i += 1;
                }

                int c = dc.SaveChanges();
                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="isPerformanceRound">if the round is for seeding, otherwise its for the actual tournament.</param>
        /// <returns></returns>
        public static bool StartNextRound(Guid tournamentId, bool isPerformanceRound)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = GetTournament(tournamentId);
                TournamentApi.IPairingsGenerator pg = null;
                TournamentTypeEnum tempType = tourny.TournamentType;
                if (isPerformanceRound)
                    tempType = tourny.TouramentTypeForSeedingEnum;
                switch (tempType)
                {
                    case TournamentTypeEnum.Boil_Off:
                        pg = new BoilOffPairingsGenerator();
                        break;
                    case TournamentTypeEnum.Round_Robin:
                        pg = new RoundRobinPairingsGenerator();
                        break;
                    case TournamentTypeEnum.Round_Robin_Pool_Play:
                        pg = new RoundRobinPairingsGenerator(true);
                        break;
                    case TournamentTypeEnum.Double_Elimination:
                        pg = new EliminationTournament(2);
                        break;
                    case TournamentTypeEnum.Single_Elimination:
                        pg = new EliminationTournament(1);
                        break;
                }
                if (isPerformanceRound)
                    pg.LoadState(tourny.TeamsForTournamentApi, tourny.TournamentRoundsApiForSeeding);
                else
                    pg.LoadState(tourny.TeamsForTournamentApi, tourny.TournamentRoundsApi);

                var tempRound = pg.CreateNextRound(null);

                bool tournamentFinished = (tempRound == null) && (tourny.TournamentRoundsApi.Count > 1);
                if (tournamentFinished)
                    return true;
                var tournament = dc.GameTournaments.Where(x => x.TournamentId == tourny.Id).FirstOrDefault();
                TournamentRound newRound = new TournamentRound();
                if (isPerformanceRound)
                    newRound.RoundNumber = tourny.TournamentRoundsApiForSeeding.Count + 1;
                else
                    newRound.RoundNumber = tourny.TournamentRoundsApi.Count + 1;
                newRound.Tournament = tournament;
                var teams = tournament.Teams.ToList();
                foreach (var pairing in tempRound.Pairings)
                {
                    var newPairing = new TournamentPairing
                    {
                        Round = newRound,
                        GroupId = pairing.GroupId
                    };

                    for (int i = 0; i < pairing.TeamScores.Count; i++)
                    {
                        var newTeamPairing = new TournamentPairingTeam
                        {
                            Team = teams.Where(t => t.TeamId == pairing.TeamScores[i].Team.TeamId).FirstOrDefault(),
                            Pairing = newPairing,
                            Score = 0
                        };
                        newPairing.Teams.Add(newTeamPairing);
                        //dc.TournamentTeams.Add(newTeamPairing);
                    }
                    newRound.Pairings.Add(newPairing);
                }
                //}

                if (isPerformanceRound)
                    tournament.SeedingRounds.Add(newRound);
                else
                    tournament.Rounds.Add(newRound);

                int c = dc.SaveChanges();
                return c > 0;


            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

        public static Guid AddTeamToTournament(Guid tournamentId, string team, int rating, int pool)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return new Guid();

                TournamentTeam t = new TournamentTeam();
                t.TeamName = team;
                t.Tournament = tourny;
                t.SeedRating = rating;
                t.PoolNumber = pool;
                if (!String.IsNullOrEmpty(team))
                    tourny.Teams.Add(t);

                int c = dc.SaveChanges();

                return t.TeamId;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new Guid();
        }
        public static bool RemoveTeamFromTournament(Guid tournamentId, Guid teamId)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return false;

                var t = tourny.Teams.Where(x => x.TeamId == teamId).FirstOrDefault();
                if (t == null)
                    return false;
                t.IsRemoved = true;

                int c = dc.SaveChanges();

                return true;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static bool AddTeamsToTournament(Guid tournamentId, List<string> teams)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                if (tourny == null)
                    return false;

                foreach (var team in teams)
                {
                    TournamentTeam t = new TournamentTeam();
                    t.TeamName = team;
                    t.Tournament = tourny;
                    if (!String.IsNullOrEmpty(team))
                        tourny.Teams.Add(t);
                }

                int c = dc.SaveChanges();

                return c > 0;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }
        public static Tournament GetTournamentToManage(Guid tournamentId, Guid privateId)
        {
            try
            {
                var dc = new ManagementContext();
                var tourny = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).Where(x => x.PrivateTournamentId == privateId).FirstOrDefault();
                if (tourny == null)
                    return null;

                return DisplayTournament(tourny);
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static string AddLogo(Guid tournamentId, Stream fileStream, string nameOfFile)
        {
            try
            {
                var dc = new ManagementContext();
                var memDb = dc.GameTournaments.Where(x => x.TournamentId == tournamentId).FirstOrDefault();
                //time stamp for the save location
                DateTime timeOfSave = DateTime.UtcNow;
                long saveTime = timeOfSave.ToFileTimeUtc();
                FileInfo info = new FileInfo(nameOfFile);

                //the file name when we save it
                string fileName = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memDb.TournamentName + " roller derby tournamnent-") + saveTime + info.Extension;
                string fileNameThumb = RDN.Utilities.Strings.StringExt.ToSearchEngineFriendly(memDb.TournamentName + " roller derby tournamnent-") + saveTime + "_thumb" + info.Extension;

                string url = "http://images.rdnation.com/tournaments/" + timeOfSave.Year + "/" + timeOfSave.Month + "/" + timeOfSave.Day + "/";
                string imageLocationToSave = @"C:\WebSites\images.rdnation.com\tournaments\" + timeOfSave.Year + @"\" + timeOfSave.Month + @"\" + timeOfSave.Day + @"\";
                //creates the directory for the image
                if (!Directory.Exists(imageLocationToSave))
                    Directory.CreateDirectory(imageLocationToSave);

                GameTournamentLogo image = new GameTournamentLogo();
                image.ImageUrl = url + fileName;
                image.SaveLocation = imageLocationToSave + fileName;
                image.ImageUrlThumb = url + fileNameThumb;
                image.SaveLocationThumb = imageLocationToSave + fileNameThumb;

                image.IsPrimaryPhoto = true;
                image.IsVisibleToPublic = true;
                image.Tournament = memDb;
                memDb.Logo = image;
                dc.GameTournamentsLogo.Add(image);

                using (var newfileStream = new FileStream(image.SaveLocation, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fileStream.CopyTo(newfileStream);
                }

                Image b = Image.FromFile(image.SaveLocation);
                Image thumb = b.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                thumb.Save(image.SaveLocationThumb);
                //saves the photo to the DB.
                int c = dc.SaveChanges();

                return url;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType(), additionalInformation: nameOfFile + " " + tournamentId);
            }
            return string.Empty;
        }


    }
}
