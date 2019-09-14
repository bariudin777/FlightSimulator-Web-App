using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Exercise3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // missions 1 & 4
            routes.MapRoute(
                name: "Redirect",
                url: "display/{arg1}/{arg2}",
                defaults: new { controller = "Display", action = "Redirect" }
            );

            // used in missions 1-3
            routes.MapRoute(
                 name: "GetFlightData",
                 url: "getFlightData",
                 defaults: new { controller = "Display", action = "GetFlightData" }
            );

            // mission 2
            routes.MapRoute(
                name: "DisplayInIntervals",
                url: "display/{ip}/{port}/{frequency}",
                defaults: new { controller = "Display", action = "DisplayInIntervals" }
            );

            // mission 3
            routes.MapRoute(
                name: "SaveAndDisplay",
                url: "save/{ip}/{port}/{frequency}/{duration}/{fileName}",
                defaults: new { controller = "Display", action = "SaveAndDisplay" }
            );

            // used in mission 3
            routes.MapRoute(
               name: "Save",
               url: "save",
               defaults: new { controller = "Display", action = "SaveData" }
           );

            // used in mission 4
            routes.MapRoute(
                name: "GetPoint",
                url: "getPoint",
                defaults: new { controller = "Display", action = "GetPoint" }
                );

            // default view
            routes.MapRoute(
                name: "default",
                url: "display/{id}",
                defaults: new { controller = "Display", action = "Default", id = UrlParameter.Optional }
            );
        }
    }
}
