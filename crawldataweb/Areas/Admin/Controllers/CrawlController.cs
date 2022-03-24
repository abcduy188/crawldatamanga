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
            string html = getbyCate("https://sstruyen.com/");
            SetAlert("Đã Lấy dữ liệu danh mục", "success");
            return Redirect("/admin/home");
        }
        public ActionResult Manga()
        {
            string url = "https://sstruyen.com/danh-sach/";
            List<Category> cate = db.Categories.ToList();
            foreach (var item in cate)
            {
                string urlcate = url + item.url; //https://sstruyen.com/danh-sach/truyen1 ,id1
                string html1 = xnethtmlManga(urlcate);// html of mange 1
                getPage(html1, item.id, urlcate);
            }
            SetAlert("Đã Lấy dữ liệu truyện", "success");
            return Redirect("/admin/home");
        }
        public ActionResult Chap()
        {
            string url = "https://sstruyen.com";
            List<manga> man = db.mangas.ToList();

            foreach (var item in man)
            {
                int number = Int32.Parse(item.chap);
                if (number <= 30)
                {
                    for (int i = 1; i <= number; i++)
                    {
                        string urlcate = url + item.url + "chuong-" + i; //https://sstruyen.com/thoi-thanh-xuan-tuoi-dep-nhat/chuong-1/ ,id1
                        string html1 = xnethtml(urlcate);// html of mange 1
                        string urlchap = "chuong-" + i;
                        gethtml(html1, item.id, urlchap);
                    }
                }
                else
                {
                    for (int i = 1; i <= 30; i++)
                    {
                        string urlcate = url + item.url + "chuong-" + i; //https://sstruyen.com/thoi-thanh-xuan-tuoi-dep-nhat/chuong-1/ ,id1
                        string html1 = xnethtml(urlcate);// html of mange 1
                        string urlchap = "chuong-" + i;
                        gethtml(html1, item.id, urlchap);
                    }
                }

            }
            return Redirect("/admin/home");
        }
        #region Category
        //Category
        public string getbyCate(string url)
        {
            //convert wweb to html
            xNet.HttpRequest http = new xNet.HttpRequest();
            string html = http.Get(url).ToString();



            //System.IO.File.WriteAllText(@"D:\res.html", html);

            //lay name and href cua tat ca the loai truyen
            string pattern = @"<ul class=""sub-menu sub-menu-cat"">(.*?)<\/ul>";
            string ul = "";
            int countUl = 0;
            foreach (Match m in Regex.Matches(html, pattern))
            {
                countUl++;
                ul += m.Groups[1];
                break;
            }
            GetUrlAndTitle(ul);
            //System.IO.File.WriteAllText(@"D:\Workspace\resul1.html", ul);
            return ul;

        }
        //lay url + name cua tung the loai truyen
        public void GetUrlAndTitle(string url)
        {

            string pattern = @"<li><a href=""danh-sach/(.*?)"" title=""(.*?)"">.*?<\/a><\/li>";
            var cate = new Category();
            //lay it truyen : khoi tao bien list=> it truyen 
            List<string> listcate = new List<string>();
            List<string> listurlcate = new List<string>();
            string urlr = "";
            int countWeb = 0;
            foreach (Match m in Regex.Matches(url, pattern))
            {
                countWeb++; //13
            }
            int countSQL = (db.Categories.ToList()).Count; //3
            if (countWeb <= countSQL)
            {
                return;
            }
            else
            {
                int newCount = countWeb - countSQL; //10
                int countInMatch = 0;

                foreach (Match m in Regex.Matches(url, pattern))
                {
                    countInMatch++; //13
                    listcate.Add(m.Groups[2].Value);
                    listurlcate.Add(m.Groups[1].Value);
                    //take all category
                    //var check = db.Categories.FirstOrDefault(d => d.url == urlr);
                    //if (check == null)
                    //{
                    //    cate.name = m.Groups[2].Value;
                    //    cate.url = m.Groups[1].Value;
                    //    db.Categories.Add(cate);
                    //    db.SaveChanges();
                    //}


                }
                //take 3 category from result

                if (countInMatch >= newCount) //always true
                {
                    int newCount1 = countInMatch - newCount; //3
                    for (int i = newCount1; i < 3; i++) //2
                    {
                        urlr = listurlcate[i];
                        var check = db.Categories.FirstOrDefault(d => d.url == urlr);
                        if (check == null)
                        {
                            cate.name = listcate[i];
                            cate.url = listurlcate[i];
                            db.Categories.Add(cate);
                            db.SaveChanges();
                        }
                    }
                    //create function to with name domain + href to redirect categorycontroller
                }
            }

        }
        #endregion

        #region Manga

        public string xnethtmlManga(string html)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            http.Cookies = new CookieDictionary();
            string html1 = http.Get(html).ToString();
            return html1;
        }
        //get page category :
        public void getPage(string html, long idcate, string urlcate)
        {

            string pattern = @"<div class=""pagination pc""><ul>(.*?)<\/ul><\/div>";
            string htmlul = ""; //li pages
            //lay html cua page
            foreach (Match m in Regex.Matches(html, pattern))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                htmlul += m.Groups[1];

            }
            getlistNumberPage(htmlul, idcate, urlcate);
            //System.IO.File.WriteAllText(@"D:\Workspace\rescatepage1.html", htmlul);

        }




        public void getlistNumberPage(string htmlul, long idcate, string urlcate)
        {
            List<Category> cate = db.Categories.ToList();

            //lay trang cuoi cung cua page -> lay dc tat ca so page
            string pattern = @".*?<li class=nexts><a href=trang-(.*?)><\/a>.*?<\/li>";
            string numberlast = "";



            foreach (Match m in Regex.Matches(htmlul, pattern))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                numberlast += m.Groups[1]; //lay dc string '23'

            }
            int number = Int32.Parse(numberlast); //long value = page (23)
            var category = db.Categories.Find(idcate);
            int lastPageCate = 0;
            if (category.lastPage != null)
            {
                lastPageCate = (int)category.lastPage;
            }
            else lastPageCate = 0;




            int countAllPageManga = 0;


            //for loop take count manga of all page

            #region moi category lay ALL trang truyen (ALL*12)
            //for (int i = 1; i < number; i++)
            //{

            //    string newurlpage = urlcate + "trang-" + i;

            //    countAllPageManga += count1page(newurlpage);
            //}
            #endregion
            #region moi category lay 21 trang truyen (21*12)
            if (number > 21)
            {
                for (int i = 1; i < 21; i++)
                {

                    string newurlpage = urlcate + "trang-" + i;

                    countAllPageManga += count1page(newurlpage);
                }
            }
            else
            {
                for (int i = 1; i <= number; i++)
                {

                    string newurlpage = urlcate + "trang-" + i;

                    countAllPageManga += count1page(newurlpage);
                }
            }
            #endregion

            int countMangaSQL = (db.mangas.Where(d => d.category_id == idcate).ToList()).Count; //240
            if (countAllPageManga > countMangaSQL) //check if == true -> has new manga ;
            {
                #region moi category lay 21 trang truyen (21*12)
                if(countMangaSQL != 0) //chua co truyen
                {
                    if (number > 21)
                    {
                        for (int i = 1; i < 21; i++)
                        {
                            //url trang  = url + (trang-i) -> ra trang tiep theo cua the loai
                            //+= nay dung de xem result in notepad -> sau chi can luu vao csdl


                            //urlcate = https://sstruyen.com/danh-sach/truyen1/trang-1 ,id1
                            string newurlpage = urlcate + "trang-" + i;

                            gettableContent(newurlpage, idcate);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= number; i++)
                        {
                            //url trang  = url + (trang-i) -> ra trang tiep theo cua the loai
                            //+= nay dung de xem result in notepad -> sau chi can luu vao csdl


                            //urlcate = https://sstruyen.com/danh-sach/truyen1/trang-1 ,id1
                            string newurlpage = urlcate + "trang-" + i;

                            gettableContent(newurlpage, idcate);
                        }
                    }



                }
                else //cap nhat them truyen
                {

                }

                #endregion
                #region moi category lay tat ca trang truyen (All*12)
                //if (number != lastPageCate) //co trang moi
                //{
                //    if (lastPageCate == 0 ||countMangaSQL == 0 ) //chua co truyen
                //    {
                //        category.lastPage = number;
                //        db.SaveChanges();
                //        for (int i = 1; i <= number; i++)
                //        {
                //            string newurlpage1 = urlcate + "trang-" + i;
                //            gettableContent(newurlpage1, idcate);
                //        }
                //    }
                //    else
                //    {
                //        int newPage = number - lastPageCate; //22-21 23 - 21 = 2
                //        if (newPage == 1) //chi co 1 page moi
                //        {
                //            category.lastPage = number;
                //            db.SaveChanges();
                //            string newurlpage = urlcate + "trang-" + newPage;
                //            gettableContent(newurlpage, idcate);
                //        }
                //        else
                //        {
                //            for (int i = number - newPage; i <= number; i++)
                //            {
                //                string newurlpage = urlcate + "trang-" + i;
                //                gettableContent(newurlpage, idcate);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    for (int i = 1; i <= number; i++)
                //    {
                //        string newurlpage = urlcate + "trang-" + i;
                //        gettableContent(newurlpage, idcate);
                //    }
                //}
                #endregion
            }
            else
            {
                return;
            }
        }

        //getcontent page 
        public void gettableContent(string url, long idcate)
        {
            string htmlcontent = xnethtml(url); //lay lai htmlcontent

            //get <tr> in table
            string pattern = @"<div class=""table-list pc""><table>(.*?)<\/table><\/div>";
            string listtablecontent = "";
            foreach (Match m in Regex.Matches(htmlcontent, pattern))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                listtablecontent += m.Groups[1]; //getTable (12 manga)
            }
            //System.IO.File.WriteAllText(@"D:\Works\rescatetablecontent.html", pattern);


            //dua vao tr lay ra tung td
            string patterntd = @"<tr.*?>(.*?)<\/tr>";
            string listoftd = "";
            foreach (Match m in Regex.Matches(listtablecontent, patterntd))
            {
                //list td
                listoftd += m.Groups[1];
            }
            //System.IO.File.WriteAllText(@"D:\Works\rescatetabletd.html", listoftd);

            //lay url + name tung truyen
            //string pattern2 = @"<td class=""image .*?""><a href=""(.*?)"" title=""(.*?)"".*?<\/td>";
            string pattern2 = @"<td class=""image.*?""><a href=""(.*?)"" title=""(.*?)""><img class="".*?"" src=""(.*?)"" alt.*?<p>Tác giả:.*? title=""(.*?)"".*?<\/td>.*?title="".*?"">Chương (.*?)<\/a><\/td>";
            var manga = new manga();
            //List<string> listmanga = new List<string>();
            //List<string> listurlmanga = new List<string>();
            string urlr = "";
            //get 1 td
            foreach (Match m in Regex.Matches(listoftd, pattern2))
            {

                if ((m.Groups[1].Value).Length < 255 && (m.Groups[3].Value).Length < 229 && m.Groups[2].Value.Length < 255)
                {

                    urlr = m.Groups[1].Value;
                    var check = db.mangas.FirstOrDefault(d => d.url == urlr);
                    if (check == null)
                    {

                        manga.name = m.Groups[2].Value;
                        manga.url = m.Groups[1].Value;
                        manga.image = "https://sstruyen.com" + m.Groups[3].Value;
                        manga.author = m.Groups[4].Value;
                        manga.chap = m.Groups[5].Value;
                        manga.category_id = idcate;
                        db.mangas.Add(manga);
                        db.SaveChanges();
                    }
                }

            }

        }



        //custom
        public int count1page(string url)
        {
            string htmlcontent = xnethtml(url); //lay lai htmlcontent

            //get <tr> in table
            string pattern = @"<div class=""table-list pc""><table>(.*?)<\/table><\/div>";
            string listtablecontent = "";
            foreach (Match m in Regex.Matches(htmlcontent, pattern))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                listtablecontent += m.Groups[1]; //getTable (12 manga)
            }
            //System.IO.File.WriteAllText(@"D:\Works\rescatetablecontent.html", pattern);


            //dua vao tr lay ra tung td
            string patterntd = @"<tr.*?>(.*?)<\/tr>";
            string listoftd = "";
            foreach (Match m in Regex.Matches(listtablecontent, patterntd))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                listoftd += m.Groups[1];
            }
            //System.IO.File.WriteAllText(@"D:\Works\rescatetabletd.html", listoftd);

            //lay url + name tung truyen
            //string pattern2 = @"<td class=""image .*?""><a href=""(.*?)"" title=""(.*?)"".*?<\/td>";
            string pattern2 = @"<td class=""image.*?""><a href=""(.*?)"" title=""(.*?)""><img class="".*?"" src=""(.*?)"" alt.*?<p>Tác giả:.*? title=""(.*?)"".*?<\/td>.*?title="".*?"">Chương (.*?)<\/a><\/td>";
            var manga = new manga();
            //List<string> listmanga = new List<string>();
            //List<string> listurlmanga = new List<string>();
            string urlr = "";
            int countMangaweb = 0;
            foreach (Match m in Regex.Matches(listoftd, pattern2))
            {

                if ((m.Groups[1].Value).Length < 255 && (m.Groups[3].Value).Length < 229 && m.Groups[2].Value.Length < 255)
                {
                    countMangaweb++;
                }

            }
            return countMangaweb;
        }






        #endregion

        #region Chap

        public string xnethtml(string html)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            http.Cookies = new CookieDictionary();
            string html1 = http.Get(html).ToString();
            return html1;
        }

        public void gethtml(string html, long idmanga, string urlchap)
        {
            //https://regex101.com/r/yv0641/1
            string pattern = @".*?title="""">(.*?)<\/a>.*?<div class=""content container1""><\/br><p>(.*?)<iframe.*?><\/iframe>(.*?)<iframe.*?<\/iframe>(.*?)<\/p>";
            var chap = new Chap();

            string urlr = "";
            foreach (Match m in Regex.Matches(html, pattern))
            {
                string word = m.Groups[2].Value.Replace("<br>", "\n");
                string word2 = m.Groups[3].Value.Replace("<br>", "\n");
                string word3 = m.Groups[4].Value.Replace("<br>", "\n");
                //word.Replace("<br>","\n");
                //word2.Replace("<br>", "\n");
                //word3.Replace("<br>", "\n");
                string wordall = word + word2 + word3;

                if ((m.Groups[1].Value).Length < 255)
                {
                    //+= nay dung de xem result in notepad->sau chi can luu vao csdl
                    string name = m.Groups[1].Value;

                    var check = db.Chaps.FirstOrDefault(d => d.url == urlchap && d.manga_id == idmanga);
                    if (check == null)
                    {
                        chap.name = m.Groups[1].Value;
                        chap.word = wordall;
                        chap.manga_id = idmanga;
                        chap.url = urlchap;
                        db.Chaps.Add(chap);
                        db.SaveChanges();

                    }


                }


            }

        }
        #endregion
    }
}