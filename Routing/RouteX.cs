using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using L24CM.Models;

namespace L24CM.Routing
{
    public static class RouteX
    {
        public static void MapExtendedRoute(this RouteCollection routes, string name, string url, object defaults)
        {
            RouteValueDictionary rvs = new RouteValueDictionary(defaults);
            routes.Add(name, new ExtendedRoute(url, rvs, new MvcRouteHandler()));
            SiteStructure.Current.AddController(name, url, rvs);
        }
    }
}
