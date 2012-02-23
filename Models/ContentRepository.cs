using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Controllers;
using L24CM.Utility;
using System.Web;
using System.Collections;
using System.Linq.Dynamic;

namespace L24CM.Models
{
    public class ContentRepository
    {
        public static ContentRepository Instance { get; set; }

        static ContentRepository()
        {
            Instance = new ContentRepository();
        }

        L24CMEntities Ctx
        {
            get
            {
                IDictionary items = HttpContext.Current.Items;
                if (items["_L24CTX"] == null)
                    items["_L24CTX"] = new L24CMEntities();
                return items["_L24CTX"] as L24CMEntities;
            }
        }

        public virtual ContentItem GetContent(List<string> significantRouteKeys, RequestDataSpecification rds)
        {
            List<string> routeValues = rds.GetRouteValues(significantRouteKeys);
            var query = Ctx.ContentItemSet.Where(c => c.Controller == rds.Controller && c.Action == rds.Action);
            for (int i = 0; i < routeValues.Count; i++)
                query = query.Where(string.Format("Subindex{0} = @0", i), new object[] { routeValues[i] });
            ContentItem contentItem = query.FirstOrDefault();
            return contentItem;
        }

        public virtual void CreateContent<TController>(string action, string[] routeKeys, string[] routeValues)
        {
            Type ctrType = typeof(TController);
            Type contentType = ctrType.GetGenericArguments().Where(t => typeof(BaseContent).IsAssignableFrom(t)).First();

            ContentItem contentItem = new ContentItem
            {
                Action = action,
                Content = null,
                Controller = ctrType.Name.UpTo("Controller"),
                Type = contentType.FullName
            };
            
                    
        }

        public virtual void Save()
        {
            Ctx.SaveChanges();
        }

        public virtual IQueryable<string> Paths()
        {
            return Ctx.ContentItemSet.Select(ci => ci.Path).OrderBy(p => p);
        }
    }
}
