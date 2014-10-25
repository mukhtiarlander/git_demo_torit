using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Mobile.Database.Calendar;
using RDN.Mobile.Database.PublicProfile;
using SQLite;
using RDN.Mobile.Database.Account;

namespace RDN.Mobile.Database
{
    public class SqlFactory
    {
        private SQLiteConnection database { get; set; }

        public SqlFactory()
        {
            // Create our connection
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            database = new SQLiteConnection(System.IO.Path.Combine(folder, "rdnation.db"));


        }

        public SqlFactory CreateTables()
        {
            database.CreateTable<SqlSkaterProfile>();
            database.CreateTable<SqlLeagueProfile>();
            database.CreateTable<SqlCalendarEvent>();
            database.CreateTable<SqlAccount>();
            return this;
        }

        public SqlFactory DropTables()
        {
            database.DropTable<SqlSkaterProfile>();
            database.DropTable<SqlLeagueProfile>();
            database.DropTable<SqlCalendarEvent>();
            database.DropTable<SqlAccount>();
            return this;
        }

        public SqlSkaterProfile GetSkaterProfile(string memberId)
        {
            return database.Table<SqlSkaterProfile>().Where(v => v.MemberId == memberId).FirstOrDefault();
        }

        public SqlFactory InsertSkaterProfile(SqlSkaterProfile skater)
        {
            database.Insert(skater);
            return this;
        }
        public SqlFactory InsertLeagueProfile(SqlLeagueProfile league)
        {
            database.Insert(league);
            return this;
        }
        public SqlLeagueProfile GetLeagueProfile(string leagueId)
        {
            return database.Table<SqlLeagueProfile>().Where(v => v.LeagueId == leagueId).FirstOrDefault();
        }
        public SqlFactory InsertCalendarEvent(SqlCalendarEvent ev)
        {
            database.Insert(ev);
            return this;
        }
        public SqlCalendarEvent GetCalendarEvent(string leagueId, string eventId)
        {
            return database.Table<SqlCalendarEvent>().Where(v => v.LeagueId == leagueId && v.CalendarItemId == eventId).FirstOrDefault();
        }
        public SqlCalendarEvent GetCalendarEvent(string eventId)
        {
            return database.Table<SqlCalendarEvent>().Where(v => v.CalendarItemId == eventId).FirstOrDefault();
        }
        public List<SqlCalendarEvent> GetCalendarEvents(string leagueId)
        {
            return database.Table<SqlCalendarEvent>().Where(v => v.LeagueId == leagueId).ToList();
        }
        public SqlAccount GetProfile()
        {
            return database.Table<SqlAccount>().Where(x => x.Id > 0).FirstOrDefault();
        }

        public SqlFactory InsertProfile(SqlAccount profile)
        {
            database.Insert(profile);
            return this;
        }
        public SqlFactory DeleteProfile()
        {
            database.DeleteAll<SqlAccount>();
            return this;
        }
    }
}
