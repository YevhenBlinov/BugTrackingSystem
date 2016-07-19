using System;
using System.Diagnostics.Contracts;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Claims;
using System.Web;
using BugTrackingSystem.Data.Model;
using BugTrackingSystem.Service.Models;
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthenticateAttribute : FilterAttribute, IAuthenticationFilter
    {
        public bool IsUserExists(AuthenticationContext filterContext)
        {
            bool f = false;
            UserViewModel userFromDB;
            var cookieValue = filterContext.HttpContext.Request.Cookies.Get("auth");
            if (cookieValue != null)
            {
                var user = FormsAuthentication.Decrypt(cookieValue.Value);
                var userService = (UserService)DependencyResolver.Current.GetService(typeof(IUserService));
                try
                {
                    userFromDB = userService.GetUserByEmail(user.Name);
                }
                catch (Exception)
                {
                    userFromDB = new UserViewModel();
                }
                f = userService.IsEmailValid(userFromDB.Email);
                if (f == false)
                {
                    var auth = new HttpCookie("auth")
                    {
                        Expires = DateTime.Now.AddDays(-1),
                        Value = null
                    };
                    filterContext.HttpContext.Response.Cookies.Add(auth);
                    filterContext.HttpContext.Session.Abandon();
                }
            }
            
            return f;
        }
        public void OnAuthentication(AuthenticationContext filterContext)
        {

            if (SkipAuthorization(filterContext.ActionDescriptor))
            {
                return;
            }

            if (IsUserExists(filterContext))
            {
                var cookieValue = filterContext.HttpContext.Request.Cookies.Get("auth");

                if (filterContext.HttpContext.Session["Email"] != null &&
                    filterContext.HttpContext.Session["Roles"] != null)
                {
                    var user = filterContext.HttpContext.Session["Email"];
                    var roles = (string[])filterContext.HttpContext.Session["Roles"];
                    filterContext.Principal = new GenericPrincipal(new GenericIdentity(user.ToString()), roles);
                }

                else if (cookieValue != null && !string.IsNullOrEmpty(cookieValue.Value))
                {

                    var user = FormsAuthentication.Decrypt(cookieValue.Value);
                    var userService = (UserService)DependencyResolver.Current.GetService(typeof(IUserService));
                    var userFromDB = userService.GetUserByEmail(user.Name);

                    filterContext.HttpContext.Session["Email"] = user.Name;
                    filterContext.HttpContext.Session["FirstName"] = userFromDB.FirstName;
                    filterContext.HttpContext.Session["LastName"] = userFromDB.LastName;
                    filterContext.HttpContext.Session["Photo"] = userFromDB.Photo;
                    filterContext.HttpContext.Session["Role"] = userFromDB.Role;
                    filterContext.HttpContext.Session["Roles"] = new string[] { userFromDB.Role.ToString() };
                    filterContext.HttpContext.Session["UserId"] = userFromDB.UserId;

                    if (user != null && !user.Expired)
                    {
                        filterContext.Principal = new GenericPrincipal(new GenericIdentity(user.Name),
                            user.UserData.Split(','));
                    }
                }
            }
            else
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Login",
                                action = "Login",
                                returnUrl = filterContext.HttpContext.Request.Url.LocalPath
                            }));
                
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            if (SkipAuthorization(filterContext.ActionDescriptor) || filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            filterContext.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Login",
                            action = "Login",
                            returnUrl = filterContext.HttpContext.Request.Url.LocalPath
                        }));
        }

        private static bool SkipAuthorization(ActionDescriptor actionDescriptor)
        {
            Contract.Assert(actionDescriptor != null);

            return actionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                         || actionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
        }
    }
}