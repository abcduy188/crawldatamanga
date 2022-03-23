﻿using System;
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
            long number = Int64.Parse(numberlast); //long value = page (23)

            var pagesess = new pageSession();
            pagesess.page = number;
            Session.Add("pagesess", pagesess);
            if(number > 21)
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
            foreach (Match m in Regex.Matches(listoftd, pattern2))
            {

                if ((m.Groups[1].Value).Length < 255 && (m.Groups[3].Value).Length < 229   && m.Groups[2].Value.Length < 255 )
                {
                    //+= nay dung de xem result in notepad->sau chi can luu vao csdl
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
            //string pattern = @"<tr><td class=""image.*?><a href=""(.*?)"" title=""(.*?)"".*? src=""(.*?)"" .*? href=""\/tac-gia.*?title=.*?>(.*?)<\/a>.*?<\/td><\/tr>";
            //< tr >< td class="image.*?><a href="(.*?)" title="(.*?)".*? src="(.*?)" .*? href="\/tac-gia.*?title=.*?>(.*?)<\/a>.*?<\/td><\/tr>
            //System.IO.File.WriteAllText(@"D:\Workspace\rescatetabletd.html", listtd);


            //done: DA lay dc name + url tat ca cac truyen
            //continue: lay chap tung truyen + show content
        }
    }
}