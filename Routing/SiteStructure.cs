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
    public class StructureException : Exception
    {
        public StructureException() : base() { }
        public StructureException(string msg) : base(msg) { }
        public StructureException(string msg, Exception inner) : base(msg, inner) { }
    }

    public class MissingControllerException : StructureException
    {
        public MissingControllerException() : base() { }
        public MissingControllerException(string msg) : base(msg) { }
        public MissingControllerException(string msg, Exception inner) : base(msg, inner) { }
    }

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

        public ControllerInfo this[string controllerName]
        {
            get
            {
                ControllerInfo controllerInfo = GetController(controllerName);
                if (controllerInfo == null) throw new MissingControllerException("Can't find controller " + controllerName);
                return controllerInfo;
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

        private ControllerInfo GetController(string name)
        {
            return SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name.ToLower() == name.ToLower());
        }

        public bool HasController(string name)
        {
            return GetController(name) != null;
        }

        public void AddController(string url, RouteValueDictionary defaults)
        {
            if (url.Contains("{controller}"))
                AddPatternToAll(url, defaults);
            else if (defaults.ContainsKey("controller"))
            {
                Type controllerType = AllControllers.FirstOrDefault(t => t.Name.UpTo("Controller") == (string)defaults["controller"]);
                if (controllerType == null)
                    throw new MissingControllerException("Attempt to add route to missing controller " + (string)defaults["controller"]);
                ControllerInfo existing = GetController((string)defaults["controller"]);
                if (existing == null)
                    Controllers.Add(new ControllerInfo(controllerType, url, defaults));
                else
                    existing.TryAddPattern(url, defaults);
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
                controller.TryAddPattern(url, defaults);
            Controllers.AddRange(newControllers);
        }

        public string GetUrl(RouteValueDictionary rvs)
        {
            if (!rvs.ContainsKey("controller")) throw new ArgumentException("Route values for GetUrl missing controller entry");
            ControllerInfo cInfo = GetController((string)rvs["controller"]);
            if (cInfo == null) throw new MissingControllerException("Can't find controller " + (string)rvs["controller"]);

            UrlPattern patt = cInfo.UrlPatterns.FirstOrDefault(up => up.Matches(rvs));
            if (patt == null) throw new StructureException("No matching pattern found on controller " + cInfo.Controller.Name);
            return patt.BuildUrl(rvs);
        }
        public string GetUrl(ContentAddress ca)
        {
            RouteValueDictionary rvs = new RouteValueDictionary();
            rvs.Add("controller", ca.Controller);
            rvs.Add("action", ca.Action);
            ControllerInfo cInfo = GetController((string)rvs["controller"]); ;
            if (cInfo == null) throw new MissingControllerException("Can't find controller " + ca.Controller);

            for (int i = 0; i < Math.Min(cInfo.SignificantRouteKeys.Count, ca.Subindexes.Count); i++)
                rvs.Add(cInfo.SignificantRouteKeys[i], ca.Subindexes[i]);

            UrlPattern patt = cInfo.UrlPatterns.FirstOrDefault(up => up.Matches(rvs));
            if (patt == null) throw new StructureException("No matching pattern found on controller " + ca.Controller);
            return patt.BuildUrl(rvs);
        }
    }
}
