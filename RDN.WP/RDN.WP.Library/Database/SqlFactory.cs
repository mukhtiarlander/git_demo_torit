using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using RDN.WP.Library.Database.PublicProfile;
using RDN.WP.Library.Database.Calendar;
using RDN.WP.Library.Database.Account;
using System.IO;
using RDN.Portable.Classes.Account;

namespace RDN.WP.Library.Database
{
    public class SqlFactory
    {
        private SQLiteConnection database { get; set; }

        public SqlFactory()
        {
            // Create our connection

            string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "rdnation.db");
            if (!File.Exists("rdnation.db"))
            {
                database = new SQLiteConnection(dbPath);
                CreateTables();
            }

        }

        public SqlFactory CreateTables()
        {
            database.CreateTable<SqlSkaterProfile>();
            database.CreateTable<SqlLeagueProfile>();
            database.CreateTable<SqlCalendarEvent>();
            database.CreateTable<SqlAccount>();
            database.CreateTable<SqlAccountSettings>();
            database.CreateTable<NotificationMobileJson>();
            return this;
        }

        public SqlFactory DropTables()
        {
            database.DropTable<SqlSkaterProfile>();
            database.DropTable<SqlLeagueProfile>();
            database.DropTable<SqlCalendarEvent>();
            database.DropTable<SqlAccountSettings>();
            database.DropTable<SqlAccount>();
            database.DropTable<NotificationMobileJson>();
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
        public SqlFactory DeleteAccountSettings()
        {
            database.DeleteAll<SqlAccountSettings>();
            return this;
        }
        public SqlAccountSettings GetAccountSettings()
        {
            return database.Table<SqlAccountSettings>().Where(x => x.Id > 0).FirstOrDefault();
        }

        public SqlFactory InsertAccountSettings(SqlAccountSettings profile)
        {
            database.Insert(profile);
            return this;
        }
        public SqlFactory DeleteProfile()
        {
            database.DeleteAll<SqlAccount>();
            return this;
        }
        public NotificationMobileJson GetNotificationSettings()
        {
            return database.Table<NotificationMobileJson>().FirstOrDefault();
        }

        public SqlFactory InsertNotificationSettings(NotificationMobileJson profile)
        {
            database.Insert(profile);
            return this;
        }
        public SqlFactory DeleteNotificationSettings()
        {
            database.DeleteAll<NotificationMobileJson>();
            return this;
        }
    }
}
