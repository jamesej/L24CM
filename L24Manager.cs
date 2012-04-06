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
using System.Reflection;

namespace L24CM
{
    public class L24Manager
    {
        static readonly L24Manager current = new L24Manager();
        public static L24Manager Current { get { return current; } }

        public static Assembly ControllerAssembly
        {
            get { return HttpContext.Current.Application["_L24ControllerAssembly"] as Assembly; }
            set { HttpContext.Current.Application["_L24ControllerAssembly"] = value; }
        }

        static L24Manager() { }

        public static void Init(RouteCollection routes, Type typeInAssemblyWithControllers)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new L24ViewEngine());

            // Put assembly containing site controllers into cache
            L24Manager.ControllerAssembly = typeInAssemblyWithControllers.Assembly;
        }

        private static string mediaPath = "/Content/Media";
        public static string MediaPath
        {
            get { return mediaPath; }
            set { mediaPath = value; }
        }

    }
}
