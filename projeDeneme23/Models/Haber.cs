// Haber.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace projeDeneme23.Models
{
    public class Haber
    {
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Baslik { get; set; }

        [Required]
        public string Icerik { get; set; }

        [Required]
        public string Kategori { get; set; }

        [Required]
        public string Yazar { get; set; }

        public DateTime YayınTarihi { get; set; } = DateTime.Now;

        // Ana resim (kapak fotoğrafı)
        public string AnaResimYolu { get; set; }

        // Habere ait diğer resimler
        public virtual ICollection<HaberResim> Resimler { get; set; }
    }

    public class HaberResim
    {
        public int Id { get; set; }
        public string ResimYolu { get; set; }
        public int HaberId { get; set; }
        public virtual Haber Haber { get; set; }
    }
}