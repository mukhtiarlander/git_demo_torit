using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.ViewModel;
using Scoreboard.Library.ViewModel;
using RDN.Library.Classes.Error;
using System.Web;

namespace RDN.Library.Cache
{
    public class GameCache : CacheLock
    {

        /// <summary>
        /// All Games for roller derby should be placed into cache if they don't already exist.
        /// </summary>
        private List<GameViewModel> GamesForRollerDerby { get; set; }

        /// <summary>
        /// used as a cache for games being added to the DB via the web interface.
        /// </summary>
        private List<GameViewModel> CurrentGamesGettingAdded { get; set; }
        private List<GameViewModel> CurrentLiveGames { get; set; }

        private List<GameViewModel> CurrentLiveDebuggingGames { get; set; }


        public static void ClearGameFromCache(Guid gameId)
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                var game = cache.CurrentLiveGames.Where(x => x.GameId == gameId).FirstOrDefault();
                if (game != null)
                {
                    cache.CurrentLiveGames.Remove(game);
                    UpdateCache(cache);
                }
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
        }

        public static List<GameViewModel> GetCurrentLiveGames()
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                return cache.CurrentLiveGames;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<GameViewModel>();
        }
        public static GameViewModel GetCurrentLiveGame(Guid id)
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                return cache.CurrentLiveGames.Where(x => x.GameId == id).FirstOrDefault();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static GameViewModel GetGameFromRollerDerby(Guid id)
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                return cache.GamesForRollerDerby.Where(x => x.GameId == id).FirstOrDefault();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static GameViewModel GetCurrentGamesGettingAdded(Guid id)
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                return cache.CurrentGamesGettingAdded.Where(x => x.GameId == id).FirstOrDefault();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static List<GameViewModel> GetCurrentLiveDebuggingGames()
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                return cache.CurrentLiveDebuggingGames;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<GameViewModel>();
        }

        /// <summary>
        /// saves the game to the web cache and returns the old cache if there is one.
        /// </summary>
        /// <param name="game"></param>
        public static GameViewModel saveGameToCurrentLiveGamesCache(GameViewModel game)
        {
            try
            {
                if (game != null)
                {
                    var cache = GetCache(HttpContext.Current.Cache);
                    var cachedGame = cache.CurrentLiveGames.Where(x => x.GameId == game.GameId).FirstOrDefault();

                    if (cachedGame != null)
                        cache.CurrentLiveGames.Remove(cachedGame);

                    cache.CurrentLiveGames.Add(game);
                    UpdateCache(cache);
                }
                return game;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static GameViewModel saveGameToGamesGettingAddedCache(GameViewModel game)
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                var cachedGame = cache.CurrentGamesGettingAdded.Where(x => x.GameId == game.GameId).FirstOrDefault();

                if (cachedGame != null)
                    cache.CurrentGamesGettingAdded.Remove(cachedGame);

                cache.CurrentGamesGettingAdded.Add(game);
                UpdateCache(cache);
                return game;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static GameViewModel saveGameToGamesGettingAddedCache(Guid gameId)
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                var cachedGame = cache.GamesForRollerDerby.Where(x => x.GameId == gameId).FirstOrDefault();

                if (cachedGame == null)
                {
                    cachedGame = GameServerViewModel.getGameFromDb(gameId);
                    cache.GamesForRollerDerby.Add(cachedGame);
                    UpdateCache(cache);
                    return cachedGame;
                }

                return cachedGame;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        public static GameViewModel saveGameToDebuggingGamesCache(GameViewModel game)
        {
            try
            {
                var cache = GetCache(HttpContext.Current.Cache);
                var cachedGame = cache.CurrentLiveDebuggingGames.Where(x => x.GameId == game.GameId).FirstOrDefault();

                if (cachedGame != null)
                    cache.CurrentLiveDebuggingGames.Remove(cachedGame);

                cache.CurrentLiveDebuggingGames.Add(game);
                UpdateCache(cache);
                return game;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }


        private static GameCache GetCache(System.Web.Caching.Cache cache)
        {
            try
            {
                GameCache dataObject = (GameCache)cache["GameCache"];
                if (dataObject == null)
                {
                    lock (ThisLock)
                    {
                        dataObject = (GameCache)cache["GameCache"];
                        if (dataObject == null)
                        {
                            dataObject = new GameCache();
                            dataObject.GamesForRollerDerby = new List<GameViewModel>();
                            dataObject.CurrentGamesGettingAdded = new List<GameViewModel>();
                            dataObject.CurrentLiveGames = new List<GameViewModel>();
                            dataObject.CurrentLiveDebuggingGames = new List<GameViewModel>();

                            cache["GameCache"] = dataObject;
                        }
                    }

                }
                return dataObject;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
        private static GameCache UpdateCache(GameCache siteCache)
        {
            try
            {
                lock (ThisLock)
                {
                    HttpContext.Current.Cache["GameCache"] = siteCache;
                }
                return siteCache;
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return null;
        }
    }
}
