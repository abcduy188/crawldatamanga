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
using crawldataweb.Common;

namespace crawldataweb.Controllers
{
    public class CategoryController : Controller
    {
        //page the loai tung truyen
        private crawlDbContext db = new crawlDbContext();
        public ActionResult Index()
        {
            string url = "https://sstruyen.com/danh-sach/";

           
            //var cate = db.Categories.Find(11664);
            //string urlcate = url + cate.url;
            //string html1 = xnethtml(urlcate);
            //getPage(html1, cate.id, urlcate);
            List<Category> cate = db.Categories.ToList();
            foreach (var item in cate)
            {   
                string urlcate = url + item.url; //https://sstruyen.com/danh-sach/truyen1 ,id1
                string html1 = xnethtml(urlcate);// html of mange 1
                getPage(html1, item.id, urlcate);
            }


            return View();

        }
        public string xnethtml(string html)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            http.Cookies = new CookieDictionary();
            string html1 = http.Get(html).ToString();
            //System.IO.File.WriteAllText(@"D:\Workspace\resCategory.html", html1);
            return html1;
        }





        //get page category :
        public void getPage(string html,long idcate,string urlcate)
        {

            string pattern = @"<div class=""pagination pc""><ul>(.*?)<\/ul><\/div>";
            string htmlul = ""; //li pages
            //lay html cua page
            foreach (Match m in Regex.Matches(html, pattern))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                htmlul += m.Groups[1];

            }
            getlistNumberPage(htmlul, idcate,urlcate);
            //System.IO.File.WriteAllText(@"D:\Workspace\rescatepage1.html", htmlul);

        }

        public void getlistNumberPage(string htmlul, long idcate,string urlcate)
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
            long number = Int64.Parse(numberlast); //long value = page (23)

            var pagesess = new pageSession();
            pagesess.page = number;
            Session.Add("pagesess", pagesess);

            for (int i = 1; i <= number; i++)
            {
                //url trang  = url + (trang-i) -> ra trang tiep theo cua the loai
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl


                //urlcate = https://sstruyen.com/danh-sach/truyen1/trang-1 ,id1
                string newurlpage = urlcate + "trang-" + i;

                gettableContent(newurlpage, idcate);
            }
            //System.IO.File.WriteAllText(@"D:\Workspace\reslistnumbercate.html", listnumber);

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
                listtablecontent += m.Groups[1];
            }
            //System.IO.File.WriteAllText(@"D:\Workspace\rescatetablecontent.html", listtablecontent);


            //dua vao tr lay ra tung td
            string patterntd = @"<tr.*?>(.*?)<\/tr>";
            string listoftd = "";
            foreach (Match m in Regex.Matches(listtablecontent, patterntd))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                listoftd += m.Groups[1];
            }
            //System.IO.File.WriteAllText(@"D:\Workspace\rescatetabletd.html", listtd);

            //lay url + name tung truyen
            string pattern2 = @"<td class=""image .*?""><a href=""(.*?)"" title=""(.*?)"".*?<\/td>";
            var manga = new manga();
            string urlr = "";
            foreach (Match m in Regex.Matches(listoftd, pattern2))
            {
                //+= nay dung de xem result in notepad -> sau chi can luu vao csdl
                urlr = m.Groups[1].Value;
                var check = db.mangas.FirstOrDefault(d => d.url == urlr);
                if (check != null)
                {
                    manga.name = m.Groups[2].Value;
                    manga.url = m.Groups[1].Value;
                    manga.category_id = idcate;
                    db.mangas.Add(manga);
                    db.SaveChanges();
                }    
               
            }
            //System.IO.File.WriteAllText(@"D:\Workspace\rescatetabletd.html", listtd);


            //done: DA lay dc name + url tat ca cac truyen
            //continue: lay chap tung truyen + show content
        }
    }
}