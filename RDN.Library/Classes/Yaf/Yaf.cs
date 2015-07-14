using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;

namespace RDN.Library.Classes.Yaf
{
    /// <summary>
    /// a class for modifications to the YAF database structure
    /// </summary>
    public class Yaf
    {
        public static bool SyncYafDisplayNamesWithDerbyNames()
        {
            try
            {
                var dc = new ManagementContext();
                var members = (from xx in dc.Members
                               select new
                               {
                                   userId = xx.AspNetUserId,
                                   derbyName = xx.DerbyName
                               }).ToList();
                YafDataContextDataContext db = new YafDataContextDataContext(System.Configuration.ConfigurationManager.ConnectionStrings["RDN"].ConnectionString);
                foreach (var mem in members)
                {
                    var user = db.yaf_Users.Where(x => x.ProviderUserKey == mem.userId.ToString()).FirstOrDefault();
                    if (user != null)
                    {
                        if (mem.derbyName != null)
                            user.DisplayName = mem.derbyName;
                        else
                            user.DisplayName = "No Nick Name";
                        if (user.DisplayName == user.Name)
                        {
                            user.DisplayName = "No Nick Name";
                        }
                    }
                }
                db.SubmitChanges();

                return true;
            }
            catch (Exception exception)
            {
                Error.ErrorDatabaseManager.AddException(exception, exception.GetType());
            }
            return false;
        }

    }
}
