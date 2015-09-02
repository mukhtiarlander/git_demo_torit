using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDN.Library.Classes.Error;
using RDN.Library.DataModels.Context;

namespace RDN.Library.Classes.Admin.League.Classes
{
    public class LeagueJoinCodes
    {
        public Guid LeagueId { get; set; }

        public string Name { get; set; }

        public Guid LeagueJoinCode { get; set; }

       
        public static List<LeagueJoinCodes> GetAllLeagueJoinCodes()
        {
            try
            {
                var dc = new ManagementContext();
                return dc.Leagues.Select(league => new LeagueJoinCodes
                {
                    LeagueId = league.LeagueId,
                    Name = league.Name,
                    LeagueJoinCode = league.LeagueJoinCode
                }).ToList();
            }
            catch (Exception exception)
            {
                ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return new List<LeagueJoinCodes>();
        }
    }
}
