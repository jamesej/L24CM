using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;
using L24CM.Models;

namespace L24CM.Routing
{
    public class ExtendedRoute : Route
    {
        public ExtendedRoute(string url, IRouteHandler rh)
            : base(url, rh)
        { SaveStructure(url, new RouteValueDictionary()); }
        public ExtendedRoute(string url, RouteValueDictionary defaults, IRouteHandler rh)
            : base(url, defaults, rh)
        { SaveStructure(url, defaults); }
        public ExtendedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler rh)
            : base(url, defaults, constraints, rh)
        { SaveStructure(url, defaults); }
        public ExtendedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler rh)
            : base(url, defaults, constraints, dataTokens, rh)
        { SaveStructure(url, defaults); }

        private void SaveStructure(string url, RouteValueDictionary defaults)
        {
            SiteStructure.Current.AddController(url, defaults);
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData rd = base.GetRouteData(httpContext);
            if (rd == null) return null;

            string path = httpContext.Request.Path;
            if (!string.IsNullOrEmpty(path) && path[0] == '/') path = path.Substring(1);
            rd.Values.Add("path", path);

            string action = httpContext.Request.QueryString["-action"]
                            ?? httpContext.Request.QueryString["-action"];
            if (action != null)
            {
                rd.Values["originalAction"] = rd.Values["action"];
                rd.Values["action"] = action;
            }

            return rd;
        }
    }
}
