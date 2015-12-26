namespace RDN.DBUpdate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20122015 : DbMigration
    {
        public override void Up()
        {
            AddColumn("RDN_Federation_Leagues", "IsRemoved ", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("RDN_Federation_Leagues", "IsRemoved");
        }
    }
}
