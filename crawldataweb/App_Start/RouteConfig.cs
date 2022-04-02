using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace crawldataweb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
            name: "search",
            url: "tim-kiem",
            defaults: new { controller = "Home", action = "Search", id = UrlParameter.Optional },
           namespaces: new[] { "crawldataweb.Controllers" }
        );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
            name: "detail",
            url: "{url}/chuong-{id}-{idm}",
            defaults: new { controller = "Home", action = "Detail", id = UrlParameter.Optional },
           namespaces: new[] { "crawldataweb.Controllers" }
        );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
            name: "Manga",
            url: "{url}/{id}",
            defaults: new { controller = "Home", action = "Manga", id = UrlParameter.Optional },
           namespaces: new[] { "crawldataweb.Controllers" }
        );
            routes.MapRoute(
             name: "Category",
             url: "the-loai/{url}/{id}",
             defaults: new { controller = "Home", action = "Category", id = UrlParameter.Optional },
            namespaces: new[] { "crawldataweb.Controllers" }
         );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "crawldataweb.Controllers" }
            );
        }
    }
}
