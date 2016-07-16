using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Service.Services;
using BugTrackingSystem.Web.Filters;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Caching;
using System.Web.UI;

namespace BugTrackingSystem.Web.Controllers
{
    [CustomAuthenticate]
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        //AppCache appCache;
        public LoginController(IUserService userService)
        {
            _userService = userService;
            //appCache = new AppCache();
        }
        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [CustomAuthorize]
        public ActionResult Logout()
        {
            Response.Cookies.Add(new HttpCookie("auth", null));
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [OutputCache(Duration = 120, Location = OutputCacheLocation.Server)]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            string headerToken = "";
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByEmail(model.Email);
                Session["FirstName"] = user.FirstName;
                Session["LastName"] = user.LastName;
                Session["Photo"] = user.Photo;
                //TODO Check in DB for existing and valid user
                if (_userService.IsUserExists(model.Email, model.Password))
                {
                    var userToken = new FormsAuthenticationTicket(1, model.Email, DateTime.Now,
                        DateTime.Now.AddMinutes(100), false, user.Role.ToString());

                    headerToken = FormsAuthentication.Encrypt(userToken);
                }

                if (!string.IsNullOrEmpty(headerToken))
                {
                    Response.Cookies.Add(new HttpCookie("auth", headerToken));

                    return string.IsNullOrEmpty(returnUrl) ? Redirect("/Home/Dashboard") : Redirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Sorry, your email or password are incorrect. Please try again.");
                }

                

                //var noms = System.Runtime.Caching.MemoryCache.Default["user"];
                //if (noms == null)
                //{
                //    noms = user;
                //    System.Runtime.Caching.MemoryCache.Default["user"] = noms;
                //}


                //Session["FirstName"] = user.FirstName.ToString();

                //var cTime = DateTime.Now.AddMinutes(11);
                //var cExp = System.Web.Caching.Cache.NoSlidingExpiration;
                //var cPri = System.Web.Caching.CacheItemPriority.Normal;
                //HttpContext.Cache.Add(cacheKey, user, null, cTime, cExp, cPri, null);
            }

            return View(model);
        }

        //public ActionResult GetUserFromCache()
        //{
        //    var user = this.Session["User"] as User;
        //    return View();
        //}

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }
    }
}