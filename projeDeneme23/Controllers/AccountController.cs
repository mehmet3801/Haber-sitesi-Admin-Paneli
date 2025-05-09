using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using projeDeneme23.Models;

namespace projeDeneme23.Controllers
{
    public class AccountController : Controller
    {
        private UygulamaDbContext db = new UygulamaDbContext();

        public ActionResult Kayit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Kayit(string email, string parola)
        {
            var userManager = new UserManager<UygulamaKullanici>(new UserStore<UygulamaKullanici>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Eğer "Admin" rolü yoksa, oluştur
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            // Kullanıcı nesnesini oluştur
            var user = new UygulamaKullanici
            {
                UserName = email,
                Email = email
            };

            var result = userManager.Create(user, parola); // Parola güvenli şekilde kaydedilir
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, "Admin"); // Kullanıcıya "Admin" rolü ver
                return RedirectToAction("Giris");
            }

            ViewBag.Hata = "Kayıt başarısız! " + string.Join(", ", result.Errors);
            return View();
        }

        public ActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Giris(string email, string parola)
        {
            var userManager = new UserManager<UygulamaKullanici>(new UserStore<UygulamaKullanici>(db));
            var user = userManager.Find(email, parola);

            if (user != null)
            {
                var authManager = HttpContext.GetOwinContext().Authentication;
                var identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties(), identity);

                return RedirectToAction("Haberler", "Haber"); // adminden sonra bıraya gidicek
            }

            ViewBag.Hata = "Email veya parola hatalı!";
            return View();
        }

        public ActionResult Cikis()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Giris");
        }
    }
}
