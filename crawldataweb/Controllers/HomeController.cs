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
    }
}