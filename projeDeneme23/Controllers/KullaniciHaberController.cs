using System;
using System.Linq;
using System.Web.Mvc;
using projeDeneme23.Models;
using System.Data.Entity;
using System.Globalization;

namespace projeDeneme23.Controllers
{
    public class KullaniciHaberController : Controller
    {
        private HaberContext db = new HaberContext();

        public ActionResult Index(string kategori = null, string tarih = null)
        {
            var haberler = db.Haberler.AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
            {
                haberler = haberler.Where(h => h.Kategori == kategori);
            }

            if (!string.IsNullOrEmpty(tarih))
            {
                try
                {
                    DateTime seciliTarih = DateTime.ParseExact(tarih, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    haberler = haberler.Where(h => DbFunctions.TruncateTime(h.YayınTarihi) == seciliTarih);
                    ViewBag.SeciliTarih = seciliTarih;
                }
                catch (FormatException)
                {
                    ViewBag.SeciliTarih = null;
                }
            }

            ViewBag.Kategoriler = db.Haberler.Select(h => h.Kategori).Distinct().ToList();
            ViewBag.SeciliKategori = kategori;

            return View(haberler.ToList());
        }

        public ActionResult Detay(int id)
        {
            var haber = db.Haberler.Find(id);
            if (haber == null)
            {
                return HttpNotFound();
            }

            var digerHaberler = db.Haberler
                .Where(h => h.Id != id && h.Kategori == haber.Kategori)
                .OrderByDescending(h => h.YayınTarihi)
                .ToList();

            ViewBag.DigerHaberler = digerHaberler;
            return View(haber);
        }
    }
}
