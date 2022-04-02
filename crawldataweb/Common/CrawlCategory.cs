using crawldataweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace crawldataweb.Common
{
    public class CrawlCategory
    {
        private crawlDbContext db = new crawlDbContext();
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
                    for (int i = newCount1; i < 4; i++) //2
                    {
                        urlr = listurlcate[i];
                        var check = db.Categories.FirstOrDefault(d => d.url == urlr);
                        if (check == null)
                        {
                            cate.name = listcate[i];
                            cate.url = listurlcate[i];
                            cate.status = true;
                            db.Categories.Add(cate);
                            db.SaveChanges();
                        }
                    }
                    //create function to with name domain + href to redirect categorycontroller
                }
            }

        }
        #endregion
    }
}