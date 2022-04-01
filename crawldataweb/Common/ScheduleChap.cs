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
    public class ScheduleChap : IJob
    {
        private crawlDbContext db = new crawlDbContext();
        public async void Execute(IJobExecutionContext context)
        {
            await Chap();

        }
        public async Task Chap()
        {
            string url = "https://sstruyen.com";
            List<manga> man = db.mangas.ToList();

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


        #region Chap
        private void runcode(string url, string urlitem, int i, long iditem)
        {
            string urlcate = url + urlitem + "chuong-" + i; //https://sstruyen.com/thoi-thanh-xuan-tuoi-dep-nhat/chuong-1/ ,id1
            string html1 = xnethtml(urlcate);// html of mange 1
            string urlchap = "chuong-" + i;
            gethtml(html1, iditem, urlchap, i);
        }

        public string xnethtml(string html)
        {
            xNet.HttpRequest http = new xNet.HttpRequest();
            string html1 = http.Get(html).ToString();
            return html1;
        }

        public void gethtml(string html, long idmanga, string urlchap, int i)
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