using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using L24CM.Models;
using L24CM.Utility;
using System.Reflection;
using System.IO;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Web.Security;
using L24CM.Membership;
using System.Text;
using System.Web.Routing;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Diagnostics;
using L24CM.Filters;

namespace L24CM.Controllers
{
    public class IncludeEntry
    {
        /// <summary>
        /// in precedence order, later items mutually depend on earlier
        /// </summary>
        public List<string> Dependencies = new List<string>();

        public string Include { get; set; }

        string id = null;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
    }

    public class ExtendedController : Controller
    {
        public const string CssItem = "_L24Css";
        public const string ScriptsItem = "_L24Scripts";
        public const string HtmlsItem = "_L24Htmls";
        public static readonly List<string> PrimaryInclude = null;

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                filterContext.HttpContext.Response.Filter = new IncludesFilter(filterContext.HttpContext.Response.Filter, this);
            }
        }

        protected virtual void RegisterInclude(string key, string incl, string id, List<string> deps)
        {
            if (HttpContext.Items[key] == null)
                HttpContext.Items[key] = new List<IncludeEntry>();
            List<IncludeEntry> incls = (HttpContext.Items[key] as List<IncludeEntry>);
            if (!incls.Any(ie => ie.Id == id))
                incls.Add(new IncludeEntry { Include = incl, Id = id, Dependencies = deps });
        }

        /// <summary>
        /// Register a script for once-only inclusion on the page
        /// </summary>
        /// <param name="script">url of script, or prefix with 'javascript:' for javascript code to include</param>
        public void RegisterScript(string script)
        {
            RegisterInclude(ScriptsItem, script, script, null);
        }
        /// <summary>
        /// Register a script for once-only inclusion on the page
        /// </summary>
        /// <param name="script">url of script, or prefix with 'javascript:' for javascript code to include</param>
        /// <param name="id"></param>
        /// <param name="script"></param>
        public void RegisterScript(string id, string script)
        {
            RegisterInclude(ScriptsItem, script, id, null);
        }
        public void RegisterScript(string id, string script, List<string> dependencies)
        {
            RegisterInclude(ScriptsItem, script, id, dependencies);
        }

        public void RegisterCss(string css)
        {
            RegisterInclude(CssItem, css, css, null);
        }
        public void RegisterCss(string id, string css)
        {
            RegisterInclude(CssItem, css, id, null);
        }
        public void RegisterCss(string id, string css, List<string> dependencies)
        {
            RegisterInclude(CssItem, css, id, dependencies);
        }

        public void RegisterHtml(string id, string html)
        {
            RegisterInclude(HtmlsItem, html, id, null);
        }

    }
}
