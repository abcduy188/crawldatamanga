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
    public class ChapController : Controller
    {
        private crawlDbContext db = new crawlDbContext();
        // GET: Chap
        public ActionResult Index()
        {
            string url = "https://sstruyen.com";


            //var cate = db.Categories.Find(11664);
            //string urlcate = url + cate.url;
            //string html1 = xnethtml(urlcate);
            //getPage(html1, cate.id, urlcate);
            List<manga> man = db.mangas.ToList();

            foreach (var item in man)
            {
                int number = Int32.Parse(item.chap); 
                for(int i =1;i<= number; i++)
                {
                    string urlcate = url + item.url +"chuong-" + i; //https://sstruyen.com/thoi-thanh-xuan-tuoi-dep-nhat/chuong-1/ ,id1
                    string html1 = xnethtml(urlcate);// html of mange 1
                    gethtml(html1,item.id);
                }

            }
            

            return View();
        }

        public string xnethtml(string html)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            http.Cookies = new CookieDictionary();
            string html1 = http.Get(html).ToString();
            System.IO.File.WriteAllText(@"D:\Works\chap.html", html1);
            return html1;
        }

        public void gethtml(string html,long idmanga)
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
                    urlr = m.Groups[1].Value;
                    var check = db.Chaps.FirstOrDefault(d => d.id == idmanga);
                    if (check == null)
                    {

                        chap.name = m.Groups[1].Value;
                        chap.word = wordall;
                        chap.manga_id = idmanga;
                        db.Chaps.Add(chap);
                        db.SaveChanges();
                    }
                }


            }

        }
    }
}