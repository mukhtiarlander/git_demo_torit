using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using RDN.Library.DataModels.Context;

namespace RDN.Library.DatabaseInitializers
{
    public static class EmailServerInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer<EmailServerContext>(null);
        }
    }
}
