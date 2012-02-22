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
        public List<string> UrlPatterns { get; set; }

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
    }

    public class SiteStructure
    {
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
                    .Where(t => t.BaseType.Name == "ContentController")
                    .ToList();

                return allControllers;
            }
        }

        public void AddPatternToAll(string url)
        {
            List<ControllerInfo> newControllers =
                AllControllers
                    .Where(c => !Controllers.Any(ci => ci.Controller.FullName == c.FullName))
                    .Select(c => new ControllerInfo
                        {
                            Controller = c,
                            Name = c.Name.UpTo("Controller"),
                            UrlPatterns = new List<string> { url }
                        })
                    .ToList();
            foreach (ControllerInfo controller in Controllers)
                controller.UrlPatterns.Add(url);
            Controllers.AddRange(newControllers);
        }
    }
}
