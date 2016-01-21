using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Sponsors.DataModels.Context;

namespace RDN.Library.DatabaseInitializers
{
    public class SponsorsIntializer
    {
        public static void Initialize()
        {
            System.Data.Entity.Database.SetInitializer<SponsorContext>(null);
        }
    }
}
