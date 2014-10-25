using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using RDN.Portable.Models.Json;

namespace RDN.WP.Library.Database.PublicProfile
{
    public class SqlSkaterProfile : SkaterJson
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public string MemberId { get; set; }
        
    }
}
