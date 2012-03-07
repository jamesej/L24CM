using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

using L24CM.Utility;
using L24CM.Controllers;
using L24CM.Models;

namespace L24CM.Routing
{
    public class ControllerInfo
    {
        public Type Controller { get; set; }
        public Type Content { get; set; }
        public string Name { get; set; }
        public string MainRouteName { get; set; }
        public string Url { get; set; }
        public List<UrlPattern> UrlPatterns { get; set; }
        public List<string> Actions { get; set; }
        public string DefaultAction { get; set; }
        public List<string> SignificantRouteKeys { get; set; }
        public Dictionary<string, string> Defaults { get; set; }

        public List<string> RouteKeys
        {
            get
            {
                return this.Url.Split('/')
                    .Where(s => s.StartsWith("{") && s.EndsWith("}") && s != "{action}")
                    .Select(s => s.Substring(1, s.Length - 2))
                    .ToList();
            }
        }

        private List<string> displayActions = null;
        private List<string> displayPatterns = null;
        public List<string> DisplayPatterns
        {
            get
            {
                if (displayPatterns == null)
                {
                    displayPatterns = UrlPatterns.SelectMany(up => up.DisplayPatterns(this.Name, this.Actions)).ToList();
                    displayActions = UrlPatterns.SelectMany(up => up.DisplayActions(this.Actions)).ToList();
                }
                return displayPatterns;
            }
        }

        List<ContentItem> instances = null;
        public List<ContentItem> Instances
        {
            get
            {
                if (instances == null)
                {
                    instances = ContentRepository.Instance.GetTemplateInstances(this.Name);
                }
                return instances;
            }
        }
        public void InvalidateInstances()
        {
            instances = null;
        }

        public ControllerInfo()
        {
            UrlPatterns = new List<UrlPattern>();
        }
        public ControllerInfo(string routeName, Type controllerType, string url, RouteValueDictionary defaults)
        {
            this.Controller = controllerType;
            this.Name = this.Controller.Name.UpTo("Controller");
            this.Url = url.Replace("{controller}", this.Name);
            this.Actions = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.ReturnType == typeof(ActionResult))
                .Select(m => m.GetCustomAttributes(typeof(ActionNameAttribute), false)
                                    .OfType<ActionNameAttribute>()
                                    .Select(ana => ana.Name)
                                    .FirstOrDefault() ?? m.Name)
                .Distinct()
                .ToList();
            this.UrlPatterns = new List<UrlPattern> { new UrlPattern { Url = url, Defaults = defaults } };
            this.Defaults = defaults.ToDictionary(rv => rv.Key, rv => (string)rv.Value);
            if (defaults.ContainsKey("action"))
                this.DefaultAction = (string)defaults["action"];
            IContentController cc = Activator.CreateInstance(this.Controller) as IContentController;
            this.SignificantRouteKeys = cc.SignificantRouteKeys;
            this.Content = cc.ContentType;
            this.MainRouteName = routeName;
        }

        public string GetPatternAction(int patternIdx)
        {
            return displayActions[patternIdx];
        }

        public bool TryAddPattern(string url, RouteValueDictionary defaults)
        {
            bool matches = true;
            if (defaults.ContainsKey("controller") && !url.Contains("{controller}"))
                matches = (this.Name == (string)defaults["controller"]);
            if (defaults.ContainsKey("action") && !url.Contains("{action}"))
                matches = matches && this.Actions.Contains((string)defaults["action"]);
            if (matches)
                this.UrlPatterns.Add(new UrlPattern { Url = url, Defaults = defaults });
            return matches;
        }

        public bool CreateInstance(string action, string[] fieldNames, string[] fieldValues)
        {
            BaseContent blankContent = Activator.CreateInstance(this.Content) as BaseContent;
            ContentItem newItem = new ContentItem(blankContent, this.SignificantRouteKeys,
                new RequestDataSpecification(this.MainRouteName, this.Name, action, fieldNames, fieldValues));

            ContentItem itemOnPath = ContentRepository.Instance.AddContentItem(newItem);
            bool created = (itemOnPath == newItem);
            if (created)
                InvalidateInstances();
            return created;
        }

        public void DeleteInstances(string[] urls)
        {
            ContentRepository.Instance.DeleteByUrls(urls);
            InvalidateInstances();
        }
    }
}
