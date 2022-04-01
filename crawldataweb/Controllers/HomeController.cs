using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xNet;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using crawldataweb.Models;

namespace crawldataweb.Controllers
{
    public class HomeController : Controller
    {

        private crawlDbContext db = new crawlDbContext();
        public ActionResult Index()
        {
            var manga = db.mangas.Take(6).OrderByDescending(d => d.views).ToList();
            ViewBag.manga = manga;
            return View();

        }
        public ActionResult Category(long id)
        {
            var manga = db.mangas.Where(d => d.category_id == id).ToList();
            return View(manga);
        }
        public ActionResult Manga(long id)
        {
            var manga = db.mangas.Find(id);
            manga.views += 1;
            db.SaveChanges();
            return View(manga);
        }
        
        public JsonResult web()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var manga = db.mangas.Take(6).OrderByDescending(d => d.views).ToList();
            ViewBag.manga = manga;
            return Json(new { manga = manga  }, JsonRequestBehavior.AllowGet);
        }
    }
}