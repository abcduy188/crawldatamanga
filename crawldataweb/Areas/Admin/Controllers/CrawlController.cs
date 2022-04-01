using crawldataweb.Common;
using crawldataweb.Models;
using DCCovid.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        [HttpPost]
        public async Task<ActionResult> ChapIndex()
        {
            await Chap();
            return Json(new { IsComplete = true });
        }

        [HttpPost]
        public async Task<ActionResult> Manga()
        {

           
             await Mangad();
           
            return Json(new { IsComplete = true});
        }

        public async Task Mangad()
        {

            string url = "https://sstruyen.com/danh-sach/";
            List<Category> cate = db.Categories.ToList();
            await Task.Run(() =>
            {
                foreach (var item in cate)
                {


                    var crawlManga = new CrawlManga();
                    string urlcate = url + item.url; //https://sstruyen.com/danh-sach/truyen1 ,id1
                    string html1 = crawlManga.xnethtmlManga(urlcate);// html of mange 1

                    crawlManga.getPage(html1, item.id, urlcate);

                }
            });


        }
        [HttpPost]
        public async Task<ActionResult> Category()
        {
            bool isComplete = false;
            bool isCancel = false;
            string errMessage = "";

            try
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    var crawl = new CrawlCategory();
                    string html = crawl.getbyCate("https://sstruyen.com/");

                });
                isComplete = true;
            }
            catch (Exception e)
            {
                errMessage = e.Message;
            }


            return Json(new { IsComplete = isComplete, ErrorMessage = errMessage, IsCancel = isCancel });
        }
        [HttpPost]
        public async Task Chap()
        {
            string url = "https://sstruyen.com";
            List<manga> man = db.mangas.ToList();
            var crawlChap = new CrawlChap();

            foreach (var item in man)
            {
                await Task.Run(() =>
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


                });
            }

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