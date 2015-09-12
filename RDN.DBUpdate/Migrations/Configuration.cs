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

    }

    internal sealed class CEmail : DbMigrationsConfiguration<Common.EmailServer.Library.Database.Context.EmailServerContext>
    {
        public CEmail()
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
