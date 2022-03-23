using crawldataweb.Common;
using crawldataweb.Models;
using crawldataweb.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace crawldataweb.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private crawlDbContext db = new crawlDbContext();
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Remove("DUY");
            return RedirectToAction("Index", "Login");
        }
        public ActionResult Login(LoginModel model)
        {
            string abc = Encryptor.MD5Hash(model.PassWord);
            if (ModelState.IsValid)
            {
                var result = db.Users.SingleOrDefault(d => d.email == model.Email);
                if (result == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại!");
                }
                else
                {
                   
                        if (result.status == false)
                        {
                            ModelState.AddModelError("", "Tài khoản đang bị khóa!");
                        }
                        else
                        {
                            if (result.password == Encryptor.MD5Hash(model.PassWord))
                            {
                                var userSession = new UserLogin();
                                userSession.Email = result.email;
                                userSession.UserID = result.id;
                                userSession.Name = result.name;
                               
                                    Session.Add("DUY", userSession);
                            Session.Timeout = 120;
                                
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Mật khẩu sai!");
                            }
                        }
                    

                }
            }
            return View("Index");
        }
    }
}