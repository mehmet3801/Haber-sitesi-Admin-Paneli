using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using projeDeneme23.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace projeDeneme23.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UygulamaDbContext db = new UygulamaDbContext();

        public ActionResult AdminListesi()
        {
            var userManager = new UserManager<UygulamaKullanici>(new UserStore<UygulamaKullanici>(db));

            var adminRole = db.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole == null)
            {
                return View(new List<UygulamaKullanici>()); // Admin rolü yoksa boş liste döndür.
            }

            var adminUsers = userManager.Users
                .Where(u => u.Roles.Any(r => r.RoleId == adminRole.Id))
                .ToList() // 🔹 Önce veriyi çekiyoruz.
                .Select(u => new UygulamaKullanici
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .ToList(); // 🔹 Sonrasında listeye çeviriyoruz.

            return View(adminUsers);
        }

        // ✅ Şifre Sıfırlama Metodu
        [HttpPost]
        public ActionResult ResetPassword(string userId)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(userId);

            if (user != null)
            {
                string newPassword = "Admin123!";
                string resetToken = userManager.GeneratePasswordResetToken(userId);
                IdentityResult result = userManager.ResetPassword(userId, resetToken, newPassword);

                if (result.Succeeded)
                {
                    return Content($"Şifre sıfırlandı. Yeni şifre: {newPassword}");
                }
                else
                {
                    return Content("Şifre sıfırlama başarısız.");
                }
            }

            return HttpNotFound();
        }


        // ✅ Yeni Admin Kullanıcı Oluştur
        public string CreateAdmin()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<UygulamaKullanici>(new UserStore<UygulamaKullanici>(db));

            // Eğer Admin rolü yoksa, ekle
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            // Kullanıcı zaten var mı kontrol et
            if (userManager.Users.Any(u => u.UserName == "admin@site.com"))
            {
                return "Admin kullanıcı zaten mevcut!";
            }

            // Yeni admin kullanıcısını oluştur
            var user = new UygulamaKullanici { UserName = "admin@site.com", Email = "admin@site.com" };
            var result = userManager.Create(user, "Admin123!");

            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, "Admin");
                return "Admin kullanıcı başarıyla oluşturuldu!";
            }
            else
            {
                return "Hata: " + string.Join(", ", result.Errors);
            }
        }
    }
}
