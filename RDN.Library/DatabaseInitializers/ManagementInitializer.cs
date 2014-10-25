using System.Data.Entity;
using RDN.Library.DataModels.Context;

namespace RDN.Library.DatabaseInitializers
{
    public static class ManagementInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer<ManagementContext>(null);
        }
    }
}
