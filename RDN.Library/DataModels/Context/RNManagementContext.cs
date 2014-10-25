using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Validation;
using System.Diagnostics;
using RDN.Library.DataModels.RN.Posts;
using RDN.Library.DataModels.RN.Financials;
using RDN.Library.DataModels.Base;

namespace RN.Library.DataModels.Context
{
    public class RNManagementContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<FundsForWriter> Funds { get; set; }
        public DbSet<MonthlyStatement> MonthlyStatements { get; set; }
        public DbSet<RSSFeed> RssFeeds { get; set; }
        public DbSet<Author> Authors { get; set; }

        public RNManagementContext()
            : base("RN")
        {
        }

        // Automatically add the times the entity got created/modified
        public override int SaveChanges()
        {
            string tempInfo = String.Empty;
            try
            {
                try
                {
                    var entries = ChangeTracker.Entries().ToList();
                    for (int i = 0; i < entries.Count; i++)
                    {
                        if (entries[i].State == EntityState.Unchanged || entries[i].State == EntityState.Detached || entries[i].State == EntityState.Deleted) continue;

                        var hasInterfaceInheritDb = entries[i].Entity as InheritDb;
                        if (hasInterfaceInheritDb == null) continue;

                        if (entries[i].State == EntityState.Added)
                        {
                            var created = entries[i].Property("Created");
                            if (created != null)
                            {
                                created.CurrentValue = DateTime.UtcNow;
                            }
                        }
                        if (entries[i].State == EntityState.Modified)
                        {
                            var modified = entries[i].Property("LastModified");
                            if (modified != null)
                            {
                                modified.CurrentValue = DateTime.UtcNow;
                            }
                        }
                    }
                }
                catch (System.Exception dbEx)
                {
                    string additionalInfo = string.Empty;


                    //ErrorDatabaseManager.AddException(dbEx, dbEx.GetType(), additionalInformation: additionalInfo);
                }
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                string additionalInfo = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        additionalInfo += "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage + "";
                    }
                }
                //ErrorDatabaseManager.AddException(dbEx, dbEx.GetType(), additionalInformation: additionalInfo);
                return 0;
            }

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {




            //modelBuilder.Entity<Group>().HasMany(x => x.Members).WithMany(c => c.Groups)
            //    .Map(y =>
            //             {
            //                 y.MapLeftKey("GroupId");
            //                 y.MapRightKey("MemberId");
            //                 y.ToTable("RDN_Team_Group_to_Member");
            //             });            
        }
    }
}
