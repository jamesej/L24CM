using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;

namespace L24CM.Routing
{
    public static class RouteX
    {
        public static void MapExtendedRoute(this RouteCollection routes, string name, string url, object defaults)
        {
            routes.Add(name, new ExtendedRoute(url, new RouteValueDictionary(defaults), new MvcRouteHandler()));
        }
    }
}
