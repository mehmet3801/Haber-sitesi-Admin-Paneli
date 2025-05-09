using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

[assembly: OwinStartup(typeof(projeDeneme23.Startup))]

namespace projeDeneme23
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Kimlik doğrulama yapılandırması
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Giris"),
                ExpireTimeSpan = TimeSpan.FromMinutes(30),
                SlidingExpiration = true,
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest
            });

            // Roller oluşturuluyor
            CreateRoles();
        }

        private void CreateRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                string[] roleNames = { "Admin", "User" };

                foreach (var roleName in roleNames)
                {
                    if (!roleManager.RoleExists(roleName))
                    {
                        roleManager.Create(new IdentityRole(roleName));
                    }
                }
            }
        }
    }
}
