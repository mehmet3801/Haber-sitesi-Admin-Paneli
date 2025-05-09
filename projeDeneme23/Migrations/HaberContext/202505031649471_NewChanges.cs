namespace projeDeneme23.Migrations.HaberContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HaberResims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResimYolu = c.String(),
                        HaberId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Habers", t => t.HaberId, cascadeDelete: true)
                .Index(t => t.HaberId);
            
            AddColumn("dbo.Habers", "AnaResimYolu", c => c.String());
            DropColumn("dbo.Habers", "ResimYolu");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Habers", "ResimYolu", c => c.String());
            DropForeignKey("dbo.HaberResims", "HaberId", "dbo.Habers");
            DropIndex("dbo.HaberResims", new[] { "HaberId" });
            DropColumn("dbo.Habers", "AnaResimYolu");
            DropTable("dbo.HaberResims");
        }
    }
}
