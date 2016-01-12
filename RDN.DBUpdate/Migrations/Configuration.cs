namespace RDN.DBUpdate.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using RDN.Library.DataModels.Context;
    using RN.Library.DataModels.Context;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using Common.AutomatedTasks.Library.Datamodels.Context;

    //internal sealed class Configuration : DbMigrationsConfiguration<RNManagementContext>
    internal sealed class Configuration : DbMigrationsConfiguration<ManagementContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;

        }

        protected override void Seed(ManagementContext context)
        {
            var colors = context.Colors.ToList();
            foreach (var league in context.LeagueColors.Include("League").Include("Color"))
            {
                if(league.League != null && league.Color != null) // if one of this is null there is no point in adding a row to color table
                {
                    //check if there is this row in Color table
                    if (colors.FirstOrDefault(db => db.LeagueOwner == league.League.LeagueId && db.ColorName == league.ColorName) == null)
                    {
                        var newLeagueColor = new RDN.Library.DataModels.Color.Color();
                        newLeagueColor.ColorName = league.ColorName;
                        newLeagueColor.ColorIdCSharp = league.Color.ColorIdCSharp;
                        newLeagueColor.LeagueOwner = league.League.LeagueId; // this is a league color

                        context.Colors.Add(newLeagueColor);
                    }
                }
            }
            context.SaveChanges();
        }
    }

    internal sealed class CEmail : DbMigrationsConfiguration<Common.EmailServer.Library.Database.Context.EmailContext>
    {
        public CEmail()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;

        }

    }

    internal sealed class CEmailServer : DbMigrationsConfiguration<Common.EmailServer.Library.Database.Context.EmailServerContext>
    {
        public CEmailServer()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;

        }

    }

    internal sealed class CError : DbMigrationsConfiguration<Common.Site.DataModels.Context.ErrorContext>
    {
        public CError()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;

        }

    }


    internal sealed class CSite : DbMigrationsConfiguration<Common.Site.DataModels.Context.SiteContext>
    {
        public CSite()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;

        }

    }


    //internal sealed class CRN : DbMigrationsConfiguration<RNManagementContext>
    //{
    //    public CRN()
    //    {
    //        AutomaticMigrationsEnabled = true;    
    //        AutomaticMigrationDataLossAllowed = true;
    //    }

    //}

    //internal sealed class CTask : DbMigrationsConfiguration<TaskContext>
    //{
    //    public CTask()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //        AutomaticMigrationDataLossAllowed = true;
    //    }
    //}
}
