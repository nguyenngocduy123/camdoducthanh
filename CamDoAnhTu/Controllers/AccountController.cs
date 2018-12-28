using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CamDoAnhTu.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CamDoAnhTu.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            string Username = model.UserName;
            string password = model.PassWord;

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                User us = ctx.Users.Where(u => u.UserName == Username && u.PassWord == password).FirstOrDefault();

                if (us != null)
                {
                    //Session["IsLogin"] = 1;
                    //Session["CurUser"] = us;
                    HttpCookie userInfo = new HttpCookie("userInfo");
                    userInfo["username"] = us.UserName;
                    userInfo["password"] = us.PassWord;
                    userInfo["permisson"] = us.Permission.ToString();
                    userInfo.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(userInfo);
                    if (model.Remember)
                    {
                        Response.Cookies["Usersname"].Value = Username;
                        Response.Cookies["Usersname"].Expires = DateTime.Now.AddDays(7);
                    }
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ViewBag.Error = "Usersname hoặc password không đúng";
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session["IsLogin"] = 0;
            Session["CurUser"] = null;

            //Response.Cookies["Username"].Expires = DateTime.Now.AddDays(-1);
            HttpCookie reqCookies = Request.Cookies["userInfo"];
            reqCookies.Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login", "Account");
        }

        const int WrongPass = 1;
        const int SamePass = 2;
        const int PwdChanged = 1;
        const int InfoChanged = 2;

        [HttpPost]
        public ActionResult UpdatePWD(ChangePasswordModel model, string uid, string pwd, string newPwd)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                User usr = ctx.Users.FirstOrDefault(u => u.UserName == model.UserName);
                if (usr.PassWord != pwd)
                {
                    return Json(new { Error = WrongPass });
                }

                else
                {
                    usr.PassWord = newPwd;
                    ctx.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();

                    return Json(new { Success = PwdChanged });
                }

            }
            //return RedirectToAction("Profile", "Account");
        }
        public ActionResult ChangePassWord(ChangePasswordModel model)
        {
            User us = new User();

            HttpCookie reqCookies = Request.Cookies["userInfo"];
            if (reqCookies != null)
            {
                us.UserName = reqCookies["username"].ToString();
                us.PassWord = reqCookies["password"].ToString();
                us.Permission = Int32.Parse(reqCookies["permisson"].ToString());
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            model.UserName = us.UserName;

            return View(model);
        }
    }
}