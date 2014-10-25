using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using RDN.Portable.Models.Json.Public;

namespace RDN.Mobile.Database.PublicProfile
{
    public class SqlLeagueProfile : LeagueJsonDataTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string LeagueId { get; set; }
        
    }
}
