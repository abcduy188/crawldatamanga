using crawldataweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using xNet;

namespace crawldataweb.Common
{
    public class CrawlChap
    {
        private crawlDbContext db = new crawlDbContext();
        #region Chap

        public string xnethtml(string html)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            string html1 = http.Get(html).ToString();
            return html1;
        }

        public void gethtml(string html, long idmanga, string urlchap, int i)
        {
            //https://regex101.com/r/yv0641/1
            string pattern = @".*?title="""">(.*?)<\/a>.*?<div class=""content container1"">(.*?)<\/div>";
         
            var chap = new Chap();

            string urlr = "";
            foreach (Match m in Regex.Matches(html, pattern))
            {
                string word = m.Groups[2].Value;
                string wordall = word;

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
                        chap.chapNumber = i;
                        db.Chaps.Add(chap);
                        db.SaveChanges();

                    }


                }


            }

        }
        #endregion
    }
}