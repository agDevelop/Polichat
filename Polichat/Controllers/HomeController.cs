using Polichat.API;
using Polichat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Polichat.Controllers
{
    public class HomeController : Controller
    {
        DBContext db = new DBContext();


        public User LoggedUser = null;

        public ActionResult Index()
        {
            HttpCookie login = Request.Cookies["login"];

            HttpCookie sign = Request.Cookies["sign"];

            if (login != null && sign != null)
            {
                if (sign.Value == CryptoProvider.GetMD5Hash(login.Value + "poly"))
                {
                    return RedirectToAction("Chat", "Home");
                }
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Login()
        {
            HttpCookie login = Request.Cookies["login"];

            HttpCookie sign = Request.Cookies["sign"];

            if (login != null && sign != null)
            {
                if (sign.Value == CryptoProvider.GetMD5Hash(login.Value + "poly"))
                {
                    return RedirectToAction("Chat", "Home");
                }
            }

            return View(db.Users);
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            try
            {
                User userToLogin = db.Users.FirstOrDefault(usr => usr.Login == user.Login);

                if (userToLogin.Password == CryptoProvider.GetMD5Hash(user.Password))
                {
                    HttpCookie login = new HttpCookie("login", user.Login);

                    HttpCookie sign = new HttpCookie("sign", CryptoProvider.GetMD5Hash(user.Login + "poly"));

                    Response.Cookies.Add(login);

                    Response.Cookies.Add(sign);

                    LoggedUser = userToLogin;

                    return RedirectToAction("Chat", "Home");
                }
            }
            catch (Exception)
            {
                return View();
            }

            return View();
        }


        [HttpPost]
        public ActionResult Register(User user)
        {
            //User userToLogin = db.Users.FirstOrDefault(usr => usr.Login == user.Login);

            if (db.Users.Count(usr => usr.Login.Equals(user.Login)) == 0)
            {
                string md5HashPas = CryptoProvider.GetMD5Hash(user.Password);

                HttpCookie login = new HttpCookie("login", user.Login);
                HttpCookie sign = new HttpCookie("sign", CryptoProvider.GetMD5Hash(user.Login + "poly"));

                Response.Cookies.Add(login);
                Response.Cookies.Add(sign);

                user.Password = md5HashPas;

                db.Users.Add(user);

                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            HttpCookie login = Request.Cookies["login"];

            HttpCookie sign = Request.Cookies["sign"];

            if (login != null && sign != null)
            {
                if (sign.Value == CryptoProvider.GetMD5Hash(login.Value + "poly"))
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpCookie login = new HttpCookie("login", string.Empty);

            HttpCookie sign = new HttpCookie("sign", string.Empty);

            login.Expires = DateTime.Now.AddDays(-1);

            sign.Expires = DateTime.Now.AddDays(-1);

            Response.Cookies.Add(login);

            Response.Cookies.Add(sign);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Chat()
        {
            HttpCookie login = Request.Cookies["login"];

            HttpCookie sign = Request.Cookies["sign"];

            if (login != null && sign != null)
            {
                if (sign.Value == CryptoProvider.GetMD5Hash(login.Value + "poly"))
                {

                    List<Message> msgsList = db.Messages.ToList<Message>();

                    msgsList.Reverse();

                    ViewBag.Title = login.Value;

                    return View(msgsList);
                }
            }

            return RedirectToAction("Login", "Home");
        }
    
    

        [HttpPost]
        public ActionResult Chat(Message msg)
        {
            HttpCookie login = Request.Cookies["login"];

            HttpCookie sign = Request.Cookies["sign"];

            if (login != null && sign != null)
            {
                if (sign.Value == CryptoProvider.GetMD5Hash(login.Value + "poly"))
                {
                    LoggedUser = db.Users.FirstOrDefault(usr => usr.Login == login.Value);

                    msg.UserName = LoggedUser.Login;
                    msg.Added = DateTime.Now;

                    db.Messages.Add(msg);

                    db.SaveChanges();

                    return RedirectToAction("Chat", "Home");
                }
            }

            return RedirectToAction("Login", "Home");
        }
    }
}