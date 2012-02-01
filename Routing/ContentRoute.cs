using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using L24CM.Models;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace L24CM.Routing
{
    public class ContentRoute : ExtendedRoute
    {
        public ContentRoute(string url, IRouteHandler rh)
            : base(url, rh) { }
        public ContentRoute(string url, RouteValueDictionary defaults, IRouteHandler rh)
            : base(url, defaults, rh) { }
        public ContentRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler rh)
            : base(url, defaults, constraints, rh) { }
        public ContentRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler rh)
            : base(url, defaults, constraints, dataTokens, rh) { }

        List<string> significantParams = new List<string>();
        public List<string> SignificantParams
        {
            get { return significantParams; }
            set { significantParams = value; }
        }

        public override RouteData GetRouteData(System.Web.HttpContextBase httpContext)
        {
            RouteData rd = base.GetRouteData(httpContext);

            L24CMEntities ctx = new L24CMEntities();

            string path = rd.Values["path"] as string;
            string action = rd.Values["action"] as string;
            bool isDiverted = (rd.Values["originalAction"] != null);
            RequestDataSpecification rds = new RequestDataSpecification(rd, httpContext.Request);
            ContentItem content = ContentRepository.Instance.GetContent(this.SignificantParams, rds);
            
            if (content == null && action != "create")
                return null;
            else
            {
                if (content != null)
                {
                    httpContext.Items["_L24Content"] = content;
                }
                string mode = httpContext.Request.QueryString["-mode"];
                if (mode != null) mode = mode.ToLower();
                if (!isDiverted && mode != "view" && Roles.IsUserInRole(User.EditorRole))
                {
                    rd.Values["originalAction"] = rd.Values["action"];
                    rd.Values["action"] = "DualWindow";
                }

                return rd;
            }
        }
    }
}
