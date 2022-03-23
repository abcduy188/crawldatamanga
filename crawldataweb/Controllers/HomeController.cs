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
           
            return View();

        }
        public ActionResult Webindex()
        {
            string html = getbyCate("https://sstruyen.com/");
            ViewBag.MyString = html;
            var listacate = db.Categories.ToList();
            return View(listacate);
        }

        public string dataindex2(string url)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            http.Cookies = new CookieDictionary();
            string html = http.Get(url).ToString();
            System.IO.File.WriteAllText(@"D:\Workspace\res.html", html);

            string pattern = @"<div class=""col-xs-4 .*?><a href=""(.*?)"" title=""(.*?)"">.*?<\/a>.*?<\/div>";
            string s = "";
            
            
            foreach (Match m in Regex.Matches(html, pattern))
            {
                s += s.Count();
            }
            
            return s;
        }
        //get by category :
        public string getbyCate(string url)
        {
            //convert wweb to html
            xNet.HttpRequest http = new xNet.HttpRequest();
            http.Cookies = new CookieDictionary();
            string html = http.Get(url).ToString();



            System.IO.File.WriteAllText(@"D:\res.html", html);

            //lay name and href cua tat ca the loai truyen
            string pattern = @"<ul class=""sub-menu sub-menu-cat"">(.*?)<\/ul>";
            string ul = "";
            foreach (Match m in Regex.Matches(html, pattern))
            {
                ul += m.Groups[1]; 
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
            foreach (Match m in Regex.Matches(url, pattern))
            {
                listcate.Add(m.Groups[2].Value);
                listurlcate.Add(m.Groups[1].Value);
               
                //var check = db.Categories.FirstOrDefault(d => d.url == urlr);
                //if (check == null)
                //{
                //    cate.name = m.Groups[2].Value;
                //    cate.url = m.Groups[1].Value;
                //    db.Categories.Add(cate);
                //    db.SaveChanges();
                //}
            }
            for (int i = 0; i < 3; i++)
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