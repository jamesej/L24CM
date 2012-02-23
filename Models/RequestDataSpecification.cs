using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using L24CM.Utility;
using System.Linq.Expressions;
using System.Web.Routing;

namespace L24CM.Models
{
    /// <summary>
    /// Specification for how to page, sort and filter data for
    /// a particular property path in the Model for the page
    /// based on standard query string arguments in the request
    /// </summary>
    public class PathDataSpecification
    {
        public const string PageSuffix = "_Page";
        public const string PageLengthSuffix = "_PageLength";
        public const string SortKeySuffix = "_SortKey";
        public const string FilterSuffix = "_Filter";

        public static PathDataSpecification Empty
        {
            get
            {
                return new PathDataSpecification
                {
                    PropertyPath = null,
                    FilteredCount = null,
                    FilterExpression = null,
                    Page = null,
                    PageLength = null,
                    SortKey = null
                };
            }
        }

        public string PropertyPath { get; set; }
        public string FilterExpression { get; set; }
        public int? FilteredCount { get; internal set; }
        public string SortKey { get; set; }
        public int? PageLength { get; set; }
        public int? Page { get; set; }

        public Dictionary<string, string> ToArgs()
        {
            var dict = new Dictionary<string, string>();
            if (Page.HasValue)
                dict.Add(PropertyPath + PageSuffix, Page.ToString());
            if (PageLength.HasValue)
                dict.Add(PropertyPath + PageLengthSuffix, PageLength.ToString());
            if (!string.IsNullOrEmpty(SortKey))
                dict.Add(PropertyPath + SortKeySuffix, SortKey);
            if (!string.IsNullOrEmpty(FilterExpression))
                dict.Add(PropertyPath + FilterSuffix, FilterExpression);
            return dict;
        }
    }

    /// <summary>
    /// Specification for which data item to get based on the request,
    /// also how to page, sort and filter various parts of that data
    /// object based on standard query string arguments in the request
    /// </summary>
    public class RequestDataSpecification
    {
        public string Path { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public IDictionary<string, object> RouteData { get; set; }
        public string LoadPath { get; set; }
        public List<PathDataSpecification> PathDataSpecs { get; set; }

        public RequestDataSpecification(ActionExecutingContext filterContext)
            : this(filterContext.RouteData, filterContext.HttpContext.Request)
        {
        }
        public RequestDataSpecification(string controller, string action, string[] routeNames, string[] routeValues)
        {
            UrlHelper urls = new UrlHelper((HttpContext.Current.Handler as MvcHandler).RequestContext);
            Controller = controller;
            Action = action;
            RouteData = Enumerable.Range(0, routeNames.Length)
                .ToDictionary(i => routeNames[i], i => (object)routeValues[i]);

            Path = urls.Action(Action, Controller);
        }
        public RequestDataSpecification(RouteData rd, HttpRequestBase req)
        {
            Path = (rd.Values["path"] as string) ?? req.Path;
            Controller = rd.Values["controller"] as string;
            Action = (rd.Values["originalAction"] as string) ?? (rd.Values["action"] as string);
            RouteData = rd.Values.ToDictionary(v => v.Key, v => v.Value);
            
            if (!string.IsNullOrEmpty(Path) && Path[0] == '/') Path = Path.Substring(1);

            List<KeyValuePair<string, string>> allValues = req.Form.ToKeyValues()
                .Concat(req.QueryString.ToKeyValues())
                .ToList();
            LoadPath = allValues.FirstSelectOrDefault(kvp => kvp.Key == "-loadpath", kvp => kvp.Value) ?? "";
            PathDataSpecs = allValues
                .Where(kvp => kvp.Key != null
                              && (kvp.Key.EndsWith(PathDataSpecification.PageSuffix)
                                  || kvp.Key.EndsWith(PathDataSpecification.PageLengthSuffix)
                                  || kvp.Key.EndsWith(PathDataSpecification.SortKeySuffix)
                                  || kvp.Key.EndsWith(PathDataSpecification.FilterSuffix)))
                .GroupBy(kvp => kvp.Key.UpTo("_"))
                .Select(kvpg => new PathDataSpecification
                {
                    PropertyPath = kvpg.Key,
                    Page = kvpg.FirstSelectOrDefault(kvp => kvp.Key.EndsWith(PathDataSpecification.PageSuffix), kvp => kvp.Value.AsIntOrNull()),
                    PageLength = kvpg.FirstSelectOrDefault(kvp => kvp.Key.EndsWith(PathDataSpecification.PageLengthSuffix), kvp => kvp.Value.AsIntOrNull()),
                    SortKey = kvpg.FirstSelectOrDefault(kvp => kvp.Key.EndsWith(PathDataSpecification.SortKeySuffix), kvp => kvp.Value),
                    FilterExpression = kvpg.FirstSelectOrDefault(kvp => kvp.Key.EndsWith(PathDataSpecification.FilterSuffix), kvp => kvp.Value)
                })
                .ToList();
        }

        public PathDataSpecification this[string path]
        {
            get
            {
                PathDataSpecification spec = this.PathDataSpecs.FirstOrDefault(pds => pds.PropertyPath == path);
                if (spec != null) return spec;
                spec = new PathDataSpecification { PropertyPath = path };
                this.PathDataSpecs.Add(spec);
                return spec;
            }
        }

        public PathDataSpecification DataSpecification
        {
            get
            {
                return this[this.LoadPath];
            }
        }

        public List<string> GetRouteValues(List<string> significantRouteKeys)
        {
            return significantRouteKeys
                .Select(rk => RouteData.ContainsKey(rk) ? RouteData[rk].ToString() : null)
                .ToList();
        }

        public Dictionary<string, string> ToArgs()
        {
            Dictionary<string, string> args =
                PathDataSpecs.SelectMany(pds => pds.ToArgs()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return args;
        }
    }
}
