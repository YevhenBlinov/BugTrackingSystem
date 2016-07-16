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
using BugTrackingSystem.Service.Services;

namespace BugTrackingSystem.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthenticateAttribute : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {

            if (SkipAuthorization(filterContext.ActionDescriptor))
            {
                return;
            }

            var cookieValue = filterContext.HttpContext.Request.Cookies.Get("auth");

            if (filterContext.HttpContext.Session["Email"] != null && filterContext.HttpContext.Session["Roles"] != null)
            {
                var user = filterContext.HttpContext.Session["Email"];
                var roles = (string[])filterContext.HttpContext.Session["Roles"];
                filterContext.Principal = new GenericPrincipal(new GenericIdentity(user.ToString()), roles);
            }

            else 
            if (cookieValue != null && !string.IsNullOrEmpty(cookieValue.Value))
            {

                var user = FormsAuthentication.Decrypt(cookieValue.Value);
                var userService = (UserService)DependencyResolver.Current.GetService(typeof(IUserService));
                var userFromDB = userService.GetUserByEmail(user.Name);

                filterContext.HttpContext.Session["Email"] = user.Name;
                filterContext.HttpContext.Session["FirstName"] = userFromDB.FirstName;
                filterContext.HttpContext.Session["LastName"] = userFromDB.LastName;
                filterContext.HttpContext.Session["Photo"] = userFromDB.Photo;
                filterContext.HttpContext.Session["Role"] = user.UserData;
                filterContext.HttpContext.Session["Roles"] = new string[] { userFromDB.Role.ToString() };

                if (user != null && !user.Expired)
                {
                    filterContext.Principal = new GenericPrincipal(new GenericIdentity(user.Name), user.UserData.Split(','));
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