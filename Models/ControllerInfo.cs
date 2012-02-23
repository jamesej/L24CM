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
                    .Where(s => s.StartsWith("{") && s.EndsWith("}") && s != "{action}")
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

        public void Create(string[] fieldNames, string[] fieldValues)
        {
            IContentController cc = Activator.CreateInstance(this.Controller) as IContentController;
            ContentItem newItem = new ContentItem(null, cc.SignificantRouteKeys,
                new RequestDataSpecification(this.Name, "", fieldNames, fieldValues));
        }
    }
}
