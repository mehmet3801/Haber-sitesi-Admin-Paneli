using projeDeneme23.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace projeDeneme23.Controllers
{
    public class KategoriController : Controller
    {
        // Örnek için hafızada tutuyorum; normalde veritabanı kullanılmalı
        private static List<Kategori> kategoriler = new List<Kategori>();

        [HttpGet]
        public ActionResult KategoriEkle()
        {
            var model = Tuple.Create(new Kategori(), kategoriler);
            return View(model);
        }

        [HttpPost]
        public ActionResult KategoriEkle(Kategori kategori)
        {
            // Basit bir ID atama
            kategori.Id = kategoriler.Count + 1;
            kategoriler.Add(kategori);

            // Tekrar boş model ve güncel liste ile döneriz
            var model = Tuple.Create(new Kategori(), kategoriler);
            return View(model);
        }
    }
}
