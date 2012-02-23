using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

using L24CM.Utility;

namespace L24CM.Models
{
    public class SiteStructure
    {
        static readonly SiteStructure current = new SiteStructure();
        public static SiteStructure Current { get { return current; } }

        static SiteStructure() { }

        List<ControllerInfo> controllers = new List<ControllerInfo>();
        public List<ControllerInfo> Controllers
        {
            get
            {
                return controllers;
            }
        }

        List<Type> allControllers = null;
        public List<Type> AllControllers
        {
            get
            {
                if (allControllers != null)
                    return allControllers;

                Assembly siteAssembly = HttpContext.Current.Application["_L24ControllerAssembly"] as Assembly;
                allControllers = siteAssembly
                    .GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType.Name.StartsWith("ContentController"))
                    .ToList();

                return allControllers;
            }
        }

        public void AddController(string url, RouteValueDictionary defaults)
        {
            if (url.Contains("{controller}"))
                AddPatternToAll(url, defaults);
            else if (defaults.ContainsKey("controller"))
            {
                Type controllerType = AllControllers.FirstOrDefault(t => t.Name.UpTo("Controller") == (string)defaults["controller"]);
                if (controllerType == null)
                    throw new Exception("Attempt to add route to missing controller " + (string)defaults["controller"]);
                Controllers.Add(new ControllerInfo(controllerType, url, defaults));
            }
            else
                throw new Exception("No controller specified");
        }

        public void AddPatternToAll(string url, RouteValueDictionary defaults)
        {
            List<ControllerInfo> newControllers =
                AllControllers
                    .Where(c => !Controllers.Any(ci => ci.Controller.FullName == c.FullName))
                    .Select(c => new ControllerInfo(c, url, defaults))
                    .ToList();
            foreach (ControllerInfo controller in Controllers)
                controller.UrlPatterns.Add(url);
            Controllers.AddRange(newControllers);
        }
    }
}
