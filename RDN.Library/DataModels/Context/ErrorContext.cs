using System;
using System.Data;
using System.Data.Entity;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.EmailServer;

namespace RDN.Library.DataModels.Context
{
    [Obsolete("Use RDN.Core.*")]
    internal class ErrorContext : DbContext
    {
        // **************** Error **************** \\
        public DbSet<Exception.Exception> ErrorExceptions { get; set; }

        public ErrorContext()
            : base("DB")
        { 
        }
    }
}
