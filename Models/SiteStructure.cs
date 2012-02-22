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
    public class ControllerInfo
    {
        public Type Controller { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> UrlPatterns { get; set; }
        public List<string> Actions { get; set; }

        public List<string> RouteKeys
        {
            get
            {
                return this.Url.Split('/')
                    .Where(s => s.StartsWith("{") && s.EndsWith("}"))
                    .Select(s => s.Substring(1, s.Length - 2))
                    .ToList();
            }
        }

        List<List<string>> instances = null;
        public List<List<string>> Instances
        {
            get
            {
                return null;
            }
        }

        public ControllerInfo()
        {
            UrlPatterns = new List<string>();
        }
        public ControllerInfo(Type controllerType, string url, RouteValueDictionary defaults)
        {
            this.Controller = controllerType;
            this.Name = this.Controller.Name.UpTo("Controller");
            this.Url = url.Replace("{controller}", this.Name);
            this.Actions = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.ReturnType == typeof(ActionResult))
                .Select(m => defaults.ContainsKey("action") && m.Name == (string)defaults["action"]
                             ? ""
                             : m.Name)
                .ToList();
            this.UrlPatterns = this.Actions
                .Select(u => this.Url.Replace("{action}", u).Replace("//", "/"))
                .Select(u => u.EndsWith("/") ? u.UpToLast("/") : u)
                .ToList();
        }
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
