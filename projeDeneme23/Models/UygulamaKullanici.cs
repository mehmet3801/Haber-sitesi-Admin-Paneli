using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projeDeneme23.Models
{
    public class UygulamaKullanici : IdentityUser
    {
    }

    public class UygulamaDbContext : IdentityDbContext<UygulamaKullanici>
    {
        public UygulamaDbContext() : base("HaberPortalDB")
        {
        }
    }
}