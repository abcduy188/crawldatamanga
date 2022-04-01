using crawldataweb.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using xNet;

namespace crawldataweb.Common
{
    public class ScheduleManga : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {
           await Mangad();

        }
        private crawlDbContext db = new crawlDbContext();
        public async Task Mangad()
        {

            string url = "https://sstruyen.com/danh-sach/";
            List<Category> cate = db.Categories.ToList();
            await Task.Run(() =>
            {
                foreach (var item in cate)
                {


                   
                    string urlcate = url + item.url; //https://sstruyen.com/danh-sach/truyen1 ,id1
                    string html1 = xnethtmlManga(urlcate);// html of mange 1

                    getPage(html1, item.id, urlcate);

                }
            });


        }


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
                htmlul += m.Groups[1];
            }
            getlistNumberPage(htmlul, idcate, urlcate);

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
            if (number > 10)
            {
                for (int i = 1; i < 10; i++)
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
                if (countMangaSQL == 0) //chua co truyen
                {
                    if (number > 10)
                    {
                        for (int i = 1; i < 10; i++)
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
                    //kho
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
            string htmlcontent = xnethtmlManga(url); //lay lai htmlcontent

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
                        if (m.Groups[5].Value != null)
                        {
                            manga.chap = Int32.Parse(m.Groups[5].Value);
                        }

                        else
                        {
                            manga.chap = 0;
                        }

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
            string htmlcontent = xnethtmlManga(url); //lay lai htmlcontent

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
    }
}