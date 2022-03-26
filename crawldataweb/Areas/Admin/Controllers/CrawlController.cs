using crawldataweb.Common;
using crawldataweb.Models;
using DCCovid.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using xNet;

namespace crawldataweb.Areas.Admin.Controllers
{
    public class CrawlController : BaseController
    {
        private crawlDbContext db = new crawlDbContext();
        // GET: Admin/Crawl
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Category()
        {
            var crawl = new CrawlCategory();
            string html = crawl.getbyCate("https://sstruyen.com/");
            SetAlert("Đã Lấy dữ liệu danh mục", "success");
            return Redirect("/admin/home");
        }
        public ActionResult Manga()
        {
            string url = "https://sstruyen.com/danh-sach/";
            List<Category> cate = db.Categories.ToList();
            var crawlManga = new CrawlManga();
            foreach (var item in cate)
            {
                string urlcate = url + item.url; //https://sstruyen.com/danh-sach/truyen1 ,id1
                string html1 = crawlManga.xnethtmlManga(urlcate);// html of mange 1
                crawlManga.getPage(html1, item.id, urlcate);
            }
            SetAlert("Đã Lấy dữ liệu truyện", "success");
            return Redirect("/admin/home");
        }
        public ActionResult Chap()
        {
            string url = "https://sstruyen.com";
            List<manga> man = db.mangas.ToList();
            var crawlChap = new CrawlChap();
            foreach (var item in man)
            {

                int number = (int)item.chap; //get chap

                #region lấy 15 chap demo
                var chaps = db.Chaps.Where(d => d.manga_id == item.id).OrderByDescending(d => d.chapNumber).ToList();
                if (chaps.Count == 0) //Chua co chap, add chap moi
                {
                    if (number <= 15)
                    {
                        for (int i = 1; i <= number; i++)
                        {
                            runcode(url, item.url, i, item.id);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= 15; i++)
                        {
                            runcode(url, item.url, i, item.id);
                        }
                    }

                    #region Lấy tất cả các chap
                    //for (int i = 1; i <= number; i++)
                    //{
                    // runcode(url, item.url, i, item.id);
                    //}
                    #endregion
                }
                else //co chap ton tai => kiem tra update
                {
                    var chap = chaps.First();
                    if (number > chap.chapNumber)
                    {
                        for (int i = chap.chapNumber + 1; i <= 15; i++) //bat dat duyet chap tiep theo
                        {

                            runcode(url, item.url, i, item.id);
                        }
                        #region Lấy tất cả các chap
                        //for (int i = chap.chapNumber + 1; i <= 15; i++) //bat dat duyet chap tiep theo
                        //{
                        //    runcode(url, item.url, i, item.id);
                        //}
                        #endregion
                    }
                }
                #endregion

            }
            return Redirect("/admin/home");
        }
        private void runcode(string url, string urlitem, int i, long iditem)
        {
            var crawlChap = new CrawlChap();
            string urlcate = url + urlitem + "chuong-" + i; //https://sstruyen.com/thoi-thanh-xuan-tuoi-dep-nhat/chuong-1/ ,id1
            string html1 = crawlChap.xnethtml(urlcate);// html of mange 1
            string urlchap = "chuong-" + i;
            crawlChap.gethtml(html1, iditem, urlchap, i);
        }
    }
}