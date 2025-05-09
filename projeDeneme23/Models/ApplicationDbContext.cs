using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace projeDeneme23.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("HaberPortalDB") // Web.config içindeki bağlantı adı
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
