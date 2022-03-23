using crawldataweb.Models;
using DCCovid.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace crawldataweb.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private crawlDbContext db = new crawlDbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Crawl()
        {
            return View();
        }
        public ActionResult Category()
        {
            var listCate = db.Categories.ToList();

            return View(listCate);
        }
        public ActionResult Manga()
        {
            var listManga = db.mangas.ToList();

            return View(listManga);
        }
    }
}