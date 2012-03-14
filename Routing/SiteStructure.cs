using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

using L24CM.Utility;
using L24CM.Models;

namespace L24CM.Routing
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

                Assembly siteAssembly = L24Manager.ControllerAssembly;
                allControllers = siteAssembly
                    .GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType.Name.StartsWith("ContentController"))
                    .ToList();

                return allControllers;
            }
        }

        public void AddController(string routeName, string url, RouteValueDictionary defaults)
        {
            if (url.Contains("{controller}"))
                AddPatternToAll(routeName, url, defaults);
            else if (defaults.ContainsKey("controller"))
            {
                Type controllerType = AllControllers.FirstOrDefault(t => t.Name.UpTo("Controller") == (string)defaults["controller"]);
                if (controllerType == null)
                    throw new Exception("Attempt to add route to missing controller " + (string)defaults["controller"]);
                Controllers.Add(new ControllerInfo(routeName, controllerType, url, defaults));
            }
            else
                throw new Exception("No controller specified");
        }

        public void AddPatternToAll(string routeName, string url, RouteValueDictionary defaults)
        {
            List<ControllerInfo> newControllers =
                AllControllers
                    .Where(c => !Controllers.Any(ci => ci.Controller.FullName == c.FullName))
                    .Select(c => new ControllerInfo(routeName, c, url, defaults))
                    .ToList();
            foreach (ControllerInfo controller in Controllers)
                controller.TryAddPattern(url, defaults);
            Controllers.AddRange(newControllers);
        }

        public string GetUrl(RouteValueDictionary rvs)
        {
            if (!rvs.ContainsKey("controller")) return null;
            ControllerInfo cInfo = Controllers.FirstOrDefault(ci => ci.Name.ToLower() == ((string)rvs["controller"]).ToLower());
            if (cInfo == null) return null;

            UrlPattern patt = cInfo.UrlPatterns.FirstOrDefault(up => up.Matches(rvs));
            if (patt == null) return null;
            return patt.BuildUrl(rvs);
        }
        public string GetUrl(ContentAddress ca)
        {
            RouteValueDictionary rvs = new RouteValueDictionary();
            rvs.Add("controller", ca.Controller);
            rvs.Add("action", ca.Action);
            ControllerInfo cInfo = Controllers.FirstOrDefault(ci => ci.Name.ToLower() == ((string)rvs["controller"]).ToLower());
            if (cInfo == null) return null;

            for (int i = 0; i < Math.Min(cInfo.SignificantRouteKeys.Count, ca.Subindexes.Count); i++)
                rvs.Add(cInfo.SignificantRouteKeys[i], ca.Subindexes[i]);

            UrlPattern patt = cInfo.UrlPatterns.FirstOrDefault(up => up.Matches(rvs));
            if (patt == null) return null;
            return patt.BuildUrl(rvs);
        }
    }
}
