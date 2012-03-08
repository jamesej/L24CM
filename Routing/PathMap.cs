using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Reflection;
using L24CM.Controllers;
using System.Web.Mvc;
using System.Web;

namespace L24CM.Routing
{
    public class ControllerPathPattern
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string PathPattern { get; set; }
    }

    public class PathMap
    {
        private class ControllerAction
        {
            public string Controller { get; set; }
            public List<string> Actions { get; set; }
            public List<string> SignificantRouteKeys { get; set; }
        }

        public static PathMap Instance
        {
            get
            {
                PathMap map = HttpContext.Current.Application["_L24PathMap"] as PathMap;
                if (map == null)
                {
                    map = new PathMap();
                    HttpContext.Current.Application["_L24PathMap"] = map;
                }
                return map;
            }
        }

        public List<ControllerPathPattern> ControllerPathPatterns { get; set; }

        public PathMap()
        {
            Assembly assemblyWithControllers = L24Manager.ControllerAssembly;

            List<ControllerAction> controllers = assemblyWithControllers.GetTypes()
                .Where(t => IsSubclassOfRawGeneric(typeof(ContentController<,>), t))
                .Select(t => new ControllerAction
                {
                    Controller = t.Name,
                    Actions = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                               .Where(act => act.ReturnType == typeof(ActionResult))
                               .Select(mi => mi.Name)
                               .ToList(),
                    SignificantRouteKeys = t.GetProperty("SignificantRouteKeys", BindingFlags.Static | BindingFlags.FlattenHierarchy).GetValue(null, null) as List<string>
                })
                .Where(c => c.Actions.Count > 0)
                .ToList();

            ControllerPathPatterns = RouteTable.Routes
                .OfType<ExtendedRoute>()
                .SelectMany(r => ExpandUrl(r, controllers))
                .ToList();

        }

        private List<ControllerPathPattern> ExpandUrl(ExtendedRoute r, List<ControllerAction> controllers)
        {
            List<ControllerPathPattern> cpps = new List<ControllerPathPattern>();

            if (r.Url.Contains("{controller}") && r.Url.Contains("{action}"))
            {
                cpps = controllers
                    .SelectMany(c => c.Actions.Select(a => new { c.Controller, Action = a, c.SignificantRouteKeys }))
                    .Select(ca => new ControllerPathPattern
                                    {
                                        PathPattern = BuildPathPattern(r.Url, ca.Controller, ca.Action, ca.SignificantRouteKeys),
                                        Controller = ca.Controller,
                                        Action = ca.Action
                                    })
                    .ToList();
            }
            else if (r.Url.Contains("{controller}"))
            {
                cpps = controllers
                    .Select(c => new ControllerPathPattern
                                    {
                                        PathPattern = BuildPathPattern(r.Url, c.Controller, "", c.SignificantRouteKeys),
                                        Controller = c.Controller,
                                        Action = r.Defaults["action"].ToString()
                                    })
                    .ToList();
            }
            else if (r.Url.Contains("{action}"))
            {
                ControllerAction controllerAction = controllers
                    .First(c => c.Controller == r.Defaults["controller"].ToString());
                cpps = controllerAction
                    .Actions
                    .Select(a => new ControllerPathPattern
                                    {
                                        PathPattern = BuildPathPattern(r.Url, "", a, controllerAction.SignificantRouteKeys),
                                        Controller = r.Defaults["controller"].ToString(),
                                        Action = a
                                    })
                    .ToList();
            }
            else
            {
                ControllerAction controllerAction = controllers
                    .First(c => c.Controller == r.Defaults["controller"].ToString());
                cpps.Add(new ControllerPathPattern
                            {
                                PathPattern = BuildPathPattern(r.Url, "", "", controllerAction.SignificantRouteKeys),
                                Controller = r.Defaults["controller"].ToString(),
                                Action = r.Defaults["action"].ToString()
                            });
            }

            return cpps;
        }

        private string BuildPathPattern(string routeUrl, string controller, string action, List<string> significantRouteKeys)
        {
            string pathPattern = routeUrl.Replace("{controller}", controller).Replace("{action}", action);
            for (int i = 0; i < significantRouteKeys.Count; i++)
                pathPattern = pathPattern.Replace("{" + significantRouteKeys[i] + "}", "$" + i.ToString());
            return pathPattern;
        }

        private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
