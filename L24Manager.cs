using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;
using System.Web.Security;
using System.Security.Principal;
using L24CM.Membership;
using System.Threading;

namespace L24CM
{
    public class L24Manager
    {
        static readonly L24Manager current = new L24Manager();
        public static L24Manager Current { get { return current; } }

        static L24Manager() { }

        public static void Init(RouteCollection routes, Type typeInAssemblyWithControllers)
        {
            // L24ViewEngine is the standard WebForms engine with added search paths for views in Areas/L24CM/Views/Shared when
            // searching from any location
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new L24ViewEngine());

            // Put assembly containing site controllers into cache
            HttpContext.Current.Application["_L24ControllerAssembly"] = typeInAssemblyWithControllers.Assembly;

            // Find embedded files in L24CM dll
            routes.Add("l24embedded", new Route("L24CM/Embedded/{*innerUrl}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(
                    new { controller = "Embedded", action = "Index" })
            });
            // Get dynamically generated content
            routes.Add("l24dynamic", new Route("L24CM/Dynamic/{action}/{*urlTail}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(
                    new { controller = "Dynamic" })
            });
            // Find urls added to site by L24CM
            routes.Add("l24default", new Route("L24CM/{controller}/{action}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(
                    new { controller = "Ajax", action = "Index" })
            });
        }

        private static string mediaPath = "/Content/Media";
        public static string MediaPath
        {
            get { return mediaPath; }
            set { mediaPath = value; }
        }

    }
}
