namespace projeDeneme23.Migrations.HaberContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Habers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Baslik = c.String(nullable: false, maxLength: 255),
                        Icerik = c.String(nullable: false),
                        Kategori = c.String(nullable: false),
                        Yazar = c.String(nullable: false),
                        YayınTarihi = c.DateTime(nullable: false),
                        ResimYolu = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Kategoris",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ad = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Kategoris");
            DropTable("dbo.Habers");
        }
    }
}
