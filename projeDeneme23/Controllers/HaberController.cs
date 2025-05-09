using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using projeDeneme23.Models;
using System.Web;
using System.Data.Entity;
using System.Globalization;
using System.Collections.Generic;

namespace projeDeneme23.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HaberController : Controller
    {
        private HaberContext db = new HaberContext();

        public ActionResult Detay(int id)
        {
            var haber = db.Haberler
                .Include(h => h.Resimler) // Resimleri de yükle
                .FirstOrDefault(h => h.Id == id);

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

        public ActionResult KategoriEkle()
        {
            var kategoriler = db.Kategoriler.ToList();
            return View(kategoriler);
        }

        public ActionResult KategoriDuzenle(int id)
        {
            var kategori = db.Kategoriler.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }

        [HttpPost]
        public async Task<ActionResult> KategoriDuzenle(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kategori).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("KategoriEkle");
            }
            return View(kategori);
        }

        public async Task<ActionResult> KategoriSil(int id)
        {
            var kategori = await db.Kategoriler.FindAsync(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }

            // Kategorinin kullanılıp kullanılmadığını kontrol et
            if (db.Haberler.Any(h => h.Kategori == kategori.Ad))
            {
                TempData["Hata"] = "Bu kategoride haberler bulunduğu için silinemez!";
                return RedirectToAction("KategoriEkle");
            }

            db.Kategoriler.Remove(kategori);
            await db.SaveChangesAsync();

            return RedirectToAction("KategoriEkle");
        }

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

            ViewBag.Kategoriler = db.Kategoriler.Select(k => k.Ad).Distinct().ToList();
            ViewBag.SeciliKategori = kategori;

            return View(haberler.OrderByDescending(h => h.YayınTarihi).ToList());
        }

        public ActionResult Ekle()
        {
            ViewBag.Kategoriler = new SelectList(db.Kategoriler.ToList(), "Ad", "Ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ekle(Haber haber, HttpPostedFileBase AnaResim, IEnumerable<HttpPostedFileBase> Resimler)
        {
            if (ModelState.IsValid)
            {
                // Ana resmi kaydet
                if (AnaResim != null && AnaResim.ContentLength > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AnaResim.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/HaberResimleri"), fileName);
                    AnaResim.SaveAs(path);
                    haber.AnaResimYolu = "/Content/HaberResimleri/" + fileName;
                }

                haber.YayınTarihi = DateTime.Now;
                db.Haberler.Add(haber);
                db.SaveChanges();

                // Diğer resimleri kaydet
                if (Resimler != null && Resimler.Any(r => r != null && r.ContentLength > 0))
                {
                    foreach (var file in Resimler.Where(r => r != null && r.ContentLength > 0))
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/HaberResimleri"), fileName);
                        file.SaveAs(path);

                        var haberResim = new HaberResim
                        {
                            ResimYolu = "/Content/HaberResimleri/" + fileName,
                            HaberId = haber.Id
                        };

                        db.HaberResimler.Add(haberResim);
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            ViewBag.Kategoriler = new SelectList(db.Kategoriler.ToList(), "Ad", "Ad");
            return View(haber);
        }

        public ActionResult Duzenle(int id)
        {
            var haber = db.Haberler
                .Include(h => h.Resimler)
                .FirstOrDefault(h => h.Id == id);

            if (haber == null)
            {
                return HttpNotFound();
            }

            ViewBag.Kategoriler = new SelectList(db.Kategoriler.ToList(), "Ad", "Ad", haber.Kategori);
            return View(haber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duzenle(Haber haber, HttpPostedFileBase AnaResim, IEnumerable<HttpPostedFileBase> Resimler, List<int> SilinecekResimler)
        {
            if (ModelState.IsValid)
            {
                var dbHaber = db.Haberler
                    .Include(h => h.Resimler)
                    .FirstOrDefault(h => h.Id == haber.Id);

                if (dbHaber == null)
                {
                    return HttpNotFound();
                }

                // Ana resmi güncelle
                if (AnaResim != null && AnaResim.ContentLength > 0)
                {
                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(dbHaber.AnaResimYolu))
                    {
                        var eskiResimYolu = Server.MapPath(dbHaber.AnaResimYolu);
                        if (System.IO.File.Exists(eskiResimYolu))
                        {
                            System.IO.File.Delete(eskiResimYolu);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(AnaResim.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/HaberResimleri"), fileName);
                    AnaResim.SaveAs(path);
                    dbHaber.AnaResimYolu = "/Content/HaberResimleri/" + fileName;
                }

                // Silinecek resimleri işle
                if (SilinecekResimler != null && SilinecekResimler.Any())
                {
                    foreach (var resimId in SilinecekResimler)
                    {
                        var resim = dbHaber.Resimler.FirstOrDefault(r => r.Id == resimId);
                        if (resim != null)
                        {
                            var resimYolu = Server.MapPath(resim.ResimYolu);
                            if (System.IO.File.Exists(resimYolu))
                            {
                                System.IO.File.Delete(resimYolu);
                            }
                            db.HaberResimler.Remove(resim);
                        }
                    }
                }

                // Yeni resimleri ekle
                if (Resimler != null && Resimler.Any(r => r != null && r.ContentLength > 0))
                {
                    foreach (var file in Resimler.Where(r => r != null && r.ContentLength > 0))
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/HaberResimleri"), fileName);
                        file.SaveAs(path);

                        var haberResim = new HaberResim
                        {
                            ResimYolu = "/Content/HaberResimleri/" + fileName,
                            HaberId = dbHaber.Id
                        };

                        db.HaberResimler.Add(haberResim);
                    }
                }

                // Diğer alanları güncelle
                dbHaber.Baslik = haber.Baslik;
                dbHaber.Icerik = haber.Icerik;
                dbHaber.Kategori = haber.Kategori;
                dbHaber.Yazar = haber.Yazar;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Kategoriler = new SelectList(db.Kategoriler.ToList(), "Ad", "Ad", haber.Kategori);
            return View(haber);
        }

        public ActionResult Haberler()
        {
            var haberler = db.Haberler.ToList();
            return View(haberler);
        }

        public async Task<ActionResult> Sil(int id)
        {
            var haber = await db.Haberler
                .Include(h => h.Resimler)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (haber != null)
            {
                // Ana resmi sil
                if (!string.IsNullOrEmpty(haber.AnaResimYolu))
                {
                    var resimYolu = Server.MapPath(haber.AnaResimYolu);
                    if (System.IO.File.Exists(resimYolu))
                    {
                        System.IO.File.Delete(resimYolu);
                    }
                }

                // Diğer resimleri sil
                foreach (var resim in haber.Resimler)
                {
                    var resimYolu = Server.MapPath(resim.ResimYolu);
                    if (System.IO.File.Exists(resimYolu))
                    {
                        System.IO.File.Delete(resimYolu);
                    }
                    db.HaberResimler.Remove(resim);
                }

                db.Haberler.Remove(haber);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}