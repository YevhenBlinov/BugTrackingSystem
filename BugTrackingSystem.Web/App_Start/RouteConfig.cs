using System.Web.Mvc;
using System.Web.Routing;

namespace BugTrackingSystem.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default2",
                url: "Login",
                defaults: new { controller = "Login", action = "Login" }
            );

            routes.MapRoute(
                name: "DefaultError",
                url: "Error/{id}",
                defaults: new { controller = "Shared", action = "Error", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}
