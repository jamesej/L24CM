using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using L24CM.Models;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using L24CM.Collation;
using L24CM.Utility;

namespace L24CM.Routing
{
    public class RedirectionSpec
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class RedirectRoute : RouteBase
    {
        List<RedirectionSpec> redirections = null;

        const string redirectStatus = "301 Moved Permanently";

        public RedirectRoute(List<RedirectionSpec> redirections) : base()
        {
            this.redirections = redirections;
        }

        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext)
        {
            string to = redirections.FirstSelectOrDefault(r => r.From.ToLower() == httpContext.Request.Url.AbsolutePath.ToLower(), r => r.To);
            if (to == null) return null;

            var response = httpContext.Response;
            response.Status = redirectStatus;
            response.RedirectLocation = to;
            response.End();

            return null;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return null;
        }
    }
}
