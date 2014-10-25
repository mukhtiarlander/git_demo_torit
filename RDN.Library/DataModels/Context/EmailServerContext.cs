using System;
using System.Data;
using System.Data.Entity;
using RDN.Library.DataModels.Base;
using RDN.Library.DataModels.EmailServer;

namespace RDN.Library.DataModels.Context
{
    internal class EmailServerContext : DbContext
    {
        public DbSet<EmailSendItem> EmailServer { get; set; }
        
        // **************** Error **************** \\
        public DbSet<Exception.Exception> ErrorExceptions { get; set; }

        public DbSet<Admin.Email.AdminEmailMessages> EmailMessages{ get; set; }  

        public EmailServerContext()
            : base("DB")
        { 
        }

        public override int SaveChanges()
        {
            try
            {
                foreach (var item in ChangeTracker.Entries())
                {
                    if (item.State == EntityState.Unchanged || item.State == EntityState.Detached || item.State == EntityState.Deleted) continue;

                    var hasInterfaceInheritDb = item.Entity as InheritDb;
                    if (hasInterfaceInheritDb == null) continue;

                    if (item.State == EntityState.Added)
                    {
                        var created = item.Property("Created");
                        if (created != null)
                        {
                            created.CurrentValue = DateTime.Now;
                        }
                    }

                    if (item.State == EntityState.Modified)
                    {
                        var modified = item.Property("LastModified");
                        if (modified != null)
                        {
                            modified.CurrentValue = DateTime.Now;
                        }
                    }
                }
                return base.SaveChanges();

            }
            catch (System.Exception e)
            {
                var ex = e;
                throw;
            }
        }
    }
}
