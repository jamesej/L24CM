using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Web.Mvc;
using System.Web;

namespace L24CM.Models
{
    public static class DataExtensions
    {
        public static IEnumerable<TElement> ProcessBySpecification<TElement>(this IEnumerable<TElement> source, PathDataSpecification spec)
        {
            return source.ProcessBySpecification(spec, null);
        }
        public static IEnumerable<TElement> ProcessBySpecification<TElement>(this IEnumerable<TElement> source, PathDataSpecification spec, int? pageLength)
        {
            IEnumerable<TElement> result = source;
            if (!string.IsNullOrEmpty(spec.FilterExpression))
                result = result.AsQueryable().Where(spec.FilterExpression).AsEnumerable();
            bool doPaging = (pageLength.HasValue || (spec.Page.HasValue && spec.PageLength.HasValue));
            if (doPaging)
                spec.FilteredCount = result.Count();
            if (!string.IsNullOrEmpty(spec.SortKey))
                result = result.AsQueryable().OrderBy(spec.SortKey).AsEnumerable();
            if (doPaging)
            {
                spec.Page = spec.Page ?? 0;
                spec.PageLength = pageLength ?? spec.PageLength.Value;
                result = result.Skip(spec.PageLength.Value * spec.Page.Value).Take(spec.PageLength.Value);
            }

            return result;
        }
        public static IQueryable<TElement> ProcessBySpecification<TElement>(this IQueryable<TElement> source, PathDataSpecification spec)
        {
            return source.ProcessBySpecification(spec, null);
        }
        public static IQueryable<TElement> ProcessBySpecification<TElement>(this IQueryable<TElement> source, PathDataSpecification spec, int? pageLength)
        {
            IQueryable<TElement> result = source;
            if (!string.IsNullOrEmpty(spec.FilterExpression))
                result = result.Where(spec.FilterExpression);
            bool doPaging = (pageLength.HasValue || (spec.Page.HasValue && spec.PageLength.HasValue));
            if (doPaging)
                spec.FilteredCount = result.Count();
            if (!string.IsNullOrEmpty(spec.SortKey))
                result = result.OrderBy(spec.SortKey);
            if (doPaging)
            {
                spec.Page = spec.Page ?? 0;
                spec.PageLength = pageLength ?? spec.PageLength.Value;
                result = result.Skip(spec.PageLength.Value * spec.Page.Value).Take(spec.PageLength.Value);
            }

            return result;
        }

        public static int? FilteredCount(this ViewDataDictionary viewData)
        {
            return viewData.FilteredCount(viewData.TemplateInfo.HtmlFieldPrefix);
        }
        public static int? FilteredCount(this ViewDataDictionary viewData, string dataPath)
        {
            RequestDataSpecification rds = HttpContext.Current.Items["_L24DataSpec"] as RequestDataSpecification;
            return rds[dataPath].FilteredCount;
        }
    }
}
