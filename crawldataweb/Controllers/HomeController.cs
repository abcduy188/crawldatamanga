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
using PagedList;

namespace crawldataweb.Controllers
{
    public class HomeController : Controller
    {

        private crawlDbContext db = new crawlDbContext();
        public ActionResult Index()
        {
            var manga = db.mangas.Take(6).OrderByDescending(d => d.views).ToList();
            ViewBag.highView = db.mangas.OrderByDescending(d => d.views).Take(5).ToList();
            ViewBag.manga = manga;
            return View();

        }
        public ActionResult Category(long id, string sort, int page = 1, int pageSize = 12)
        {
            IEnumerable<manga> manga;
            switch (sort)
            {
                case "name":
                    manga = db.mangas.Where(d => d.category_id == id).OrderBy(d => d.name);
                    break;
                case "-name":
                     manga = db.mangas.Where(d => d.category_id == id).OrderByDescending(d => d.name);
                    break;
                case "views":
                    manga = db.mangas.Where(d => d.category_id == id).OrderByDescending(d => d.views);
                    break;
                default:
                    manga = db.mangas.Where(d => d.category_id == id).OrderBy(d => d.id);
                    break;
            }
           
            var cate = db.Categories.Find(id);
            ViewBag.cate = cate.name;
            ViewBag.sort = sort;
            ViewBag.highView = db.mangas.OrderByDescending(d => d.views).Take(5).ToList();
            var result = manga.ToPagedList(page, pageSize);
            return View(result);
        }
        public ActionResult Manga(long id, int page = 1, int pageSize = 6)
        {
            var manga = db.mangas.Find(id);
            manga.views += 1;
            db.SaveChanges();
            IEnumerable<Chap> chap = db.Chaps.Where(d => d.manga_id == id).OrderBy(d => d.id);
            ViewBag.manga = manga;
            var result = chap.ToPagedList(page, pageSize);
            ViewBag.updatenew = db.Chaps.Where(d => d.manga_id == id).OrderByDescending(d => d.id).Take(3).ToList();
            return View(result);
        }
        public ActionResult Detail(long id, long idm)
        {
            var chap = db.Chaps.FirstOrDefault(d => d.id == id && d.manga_id == idm);
            ViewBag.listChap = db.Chaps.Where(d=>d.manga_id == idm).ToList();
            return View(chap);
        }

        public ActionResult Search(string KeyWord, int? page, int pageSize = 6)
        {
            IQueryable<manga> manga = db.mangas;
            if (!string.IsNullOrEmpty(KeyWord))
            {
                manga = db.mangas.Where(d => d.name.Contains(KeyWord) || d.author.Contains(KeyWord));
            }
            ViewBag.KeyWord = KeyWord;
            int pageNum = (page ?? 1);
            return View(manga.OrderBy(d => d.name).ToPagedList(pageNum, pageSize));

        }
        [ChildActionOnly]
        public PartialViewResult CatePartial()
        {
            ViewBag.List = db.Categories.ToList();
           
            return PartialView();
        }
    }
}