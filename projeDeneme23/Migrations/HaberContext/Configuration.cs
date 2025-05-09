namespace projeDeneme23.Migrations.HaberContext
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<projeDeneme23.Models.HaberContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\HaberContext";
            ContextKey = "projeDeneme23.Models.HaberContext";
        }

        protected override void Seed(projeDeneme23.Models.HaberContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
