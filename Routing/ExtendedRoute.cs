using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;

namespace L24CM.Routing
{
    public class ExtendedRoute : Route
    {
        public ExtendedRoute(string url, IRouteHandler rh)
            : base(url, rh) { }
        public ExtendedRoute(string url, RouteValueDictionary defaults, IRouteHandler rh)
            : base(url, defaults, rh) { }
        public ExtendedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler rh)
            : base(url, defaults, constraints, rh) { }
        public ExtendedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler rh)
            : base(url, defaults, constraints, dataTokens, rh) { }

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
