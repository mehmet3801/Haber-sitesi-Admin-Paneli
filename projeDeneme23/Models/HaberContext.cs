using System.Data.Entity;
using projeDeneme23.Models;

namespace projeDeneme23.Models
{
    public class HaberContext : DbContext
    {
        public HaberContext() : base("name=HaberPortalDB")
        {
        }

        public DbSet<Haber> Haberler { get; set; }
        public DbSet<HaberResim> HaberResimler { get; set; } // Yeni eklenen
        public DbSet<Kategori> Kategoriler { get; set; }
    }
}