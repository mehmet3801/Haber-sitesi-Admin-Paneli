using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using projeDeneme23.Models; // Doğru namespace'i eklemeyi unutma
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security;


public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext()
        : base("HaberPortalDB") // Web.config içindeki bağlantı adı
    {
    }

    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }


    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            return manager;
        }


        public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
        {
            public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
                : base(userManager, authenticationManager)
            {
            }

            public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
            {
                return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
            }
        }

    }

}