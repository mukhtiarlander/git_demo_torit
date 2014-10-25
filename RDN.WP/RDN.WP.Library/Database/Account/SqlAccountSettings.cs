using RDN.Portable.Classes.Account;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDN.WP.Library.Database.Account
{
    
    public class SqlAccountSettings : AccountSettingsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public SqlAccountSettings()
        { }

        public SqlAccountSettings(AccountSettingsModel u)
        {
            this.CalendarId = u.CalendarId;
            this.ChallengeCount = u.ChallengeCount;
            this.IsApartOfLeague = u.IsApartOfLeague;
            this.IsDuesManagementLockedDown = u.IsDuesManagementLockedDown;
            this.IsEventsCoordinatorOrBetter = u.IsEventsCoordinatorOrBetter;
            this.IsManagerOrBetter = u.IsManagerOrBetter;
            this.IsSubscriptionActive = u.IsSubscriptionActive;
            this.IsTreasurer = u.IsTreasurer;
            this.LeagueLogo = u.LeagueLogo;
            this.OfficialsRequestCount = u.OfficialsRequestCount;
            this.ShopEndUrl = u.ShopEndUrl;
            this.UnreadMessagesCount = u.UnreadMessagesCount;
        }

    }
}
