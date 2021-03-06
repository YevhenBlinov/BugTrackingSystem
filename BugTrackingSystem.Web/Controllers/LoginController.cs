﻿using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Web.Filters;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Caching;
using System.Web.UI;
using BugTrackingSystem.Service.Models;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        public LoginController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: Login
        [AllowAnonymous]
        
        public ActionResult Login()
        {
            if (Session["Email"] != null)
                return RedirectToAction("Dashboard", "Home");
            return View();
        }

        [CustomAuthorize]
        [Route("Logout")]
        public ActionResult Logout()
        {
            if (Request.Cookies["auth"] != null && Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var auth = new HttpCookie("auth")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                var aspnet = new HttpCookie("ASP.NET_SessionId")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                Response.Cookies.Add(auth);
                Response.Cookies.Add(aspnet);
            }
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            string headerToken = "";
            if (ModelState.IsValid)
            {
                UserViewModel user = new UserViewModel();
                try
                {
                    user = _userService.GetUserByEmail(model.Email);
                }
                catch (Exception)
                {
                }
                //TODO Check in DB for existing and valid user
                if (_userService.IsUserExists(model.Email, model.Password))
                {
                    int expiresDays = 1;
                    if (model.RememberMe)
                        expiresDays = 365;
                    Session["FirstName"] = user.FirstName;
                    Session["LastName"] = user.LastName;
                    Session["Photo"] = user.Photo;
                    Session["Email"] = user.Email;
                    Session["Role"] = user.Role;
                    Session["Roles"] = new string[] { user.Role.ToString() };
                    Session["UserId"] = user.UserId;
                    //HttpCookie myCookie = new HttpCookie("auth");
                    //myCookie["FirstName"] = user.FirstName;
                    //myCookie["LastName"] = user.LastName;
                    //myCookie["Photo"] = user.Photo;
                    //myCookie["Email"] = user.Email;
                    //myCookie.Expires = DateTime.Now.AddDays(expiresDays);
                    //Response.Cookies.Add(myCookie);

                    var userToken = new FormsAuthenticationTicket(1, model.Email, DateTime.Now,
                        DateTime.Now.AddDays(expiresDays), false, user.Role.ToString()/*, FormsAuthentication.FormsCookiePath*/);

                    headerToken = FormsAuthentication.Encrypt(userToken);
                }

                if (!string.IsNullOrEmpty(headerToken))
                {
                    Response.Cookies.Add(new HttpCookie("auth", headerToken));

                    return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Dashboard","Home") : RedirectToAction(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Sorry, your email or password are incorrect. Please try again.");
                }
            }

            return View(model);
        }

        [Route("ForgotPassword")]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [Route("ResetPassword")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string email)
        {
            var isEmailValid = _userService.IsEmailValid(email);

            if (isEmailValid)
            {
                _userService.SendResetPasswordEmailToUser(email);
                return RedirectToActionPermanent("Login", "Login");
            }
            else
            {
                ModelState.AddModelError("", "The email adress you entered is not registered");
                return View();
            }
            
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string resetPassword)
        {
            int userId = _userService.GetUserIdByCryptEmail(resetPassword);
            ViewBag.UserId = userId;
            return View();
        }

        [AllowAnonymous]
        public void ResetPasswordConfirm(int userId, string password)
        {
            _userService.ChangeUserPassword(userId, password);
        }
    }
}