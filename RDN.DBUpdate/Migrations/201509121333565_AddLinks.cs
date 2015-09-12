namespace RDN.DBUpdate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLinks : DbMigration
    {
        public override void Up()
        {   
            CreateTable(
                "dbo.RDN_League_Links",
                c => new
                    {
                        LinkId = c.Long(nullable: false, identity: true),
                        Link = c.String(),
                        Notes = c.String(),
                        IsRemoved = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        LastModified = c.DateTime(),
                        LinksAddByMember_MemberId = c.Guid(),
                        LinksForLeague_LeagueId = c.Guid(),
                    })
                .PrimaryKey(t => t.LinkId)
                .ForeignKey("dbo.RDN_Members", t => t.LinksAddByMember_MemberId)
                .ForeignKey("dbo.RDN_Leagues", t => t.LinksForLeague_LeagueId)
                .Index(t => t.LinksAddByMember_MemberId)
                .Index(t => t.LinksForLeague_LeagueId);            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RDN_League_Links", "LinksForLeague_LeagueId", "dbo.RDN_Leagues");
            DropForeignKey("dbo.RDN_League_Links", "LinksAddByMember_MemberId", "dbo.RDN_Members");
           
            DropIndex("dbo.RDN_League_Links", new[] { "LinksForLeague_LeagueId" });
            DropIndex("dbo.RDN_League_Links", new[] { "LinksAddByMember_MemberId" });
            
            DropTable("dbo.RDN_League_Links");         
        }
    }
}
