using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.ViewModels;
using E_Shopper.Models;


namespace E_Shopper.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountModel _Model;
        public AccountController()
        {
            _Model = new AccountModel();
        }

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            AccountViewModel userinfo = _Model.GetUserByUsername(username);


            if (userinfo == null)
            {
                ViewBag.ErrorMessage = "沒有該使用者";
                return View();
            }
            bool validateAccount = _Model.ValidatePassword(userinfo, password);


            if (validateAccount != true)
            {
                ViewBag.ErrorMessage = "密碼錯誤";
                return View();
            }

            Session["UserID"] = userinfo.VCHUSERID;

            return RedirectToAction("Index", "Home");


        }


        public ActionResult Register()
        {
            return View();
        }

        // 用戶註冊
        [HttpPost]
        public ActionResult Register(string username, string email, string role, string password, string confirmPassword)
        {
            try
            {
                if (password != confirmPassword)
                {
                    ModelState.AddModelError("", "兩次密碼不一致");
                    ViewBag.Message = "兩次密碼不一致";
                    ViewBag.MessageType = "danger";

                    return View(); // 密碼返回錯誤時註冊畫面


                }

                string passwordHash = AccountModel.HashPassword(password); // 加密密碼

                AccountViewModel user = new AccountViewModel
                {

                    VCHUSERNAME = username,
                    VCHPASSWORD = passwordHash,
                    VCHEMAIL = email,
                    VCHROLE = role

                };

                _Model.RegisterUser(user);

            }

            catch(Exception)
            {

            }
            return RedirectToAction("Login"); // 註冊成功後跳轉回登入畫面

        }
    }
}