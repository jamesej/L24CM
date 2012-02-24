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

        public virtual ContentItem AddContentItem(ContentItem item)
        {
            ContentItem existing = Ctx.ContentItemSet.FirstOrDefault(ci => ci.Path == item.Path);
            if (existing != null)
                return existing;
            else
            {
                Ctx.AddToContentItemSet(item);
                Ctx.SaveChanges();
                return item;
            }
        }

        public virtual List<ContentItem> GetTemplateInstances(string controller)
        {
            var insts = Ctx.ContentItemSet
                .Where(ci => ci.Controller == controller)
                .Select(ci => new { ci.Path, ci.Action, ci.Subindex0, ci.Subindex1, ci.Subindex2, ci.Subindex3, ci.Subindex4, ci.Subindex5 });
            List<ContentItem> items = new List<ContentItem>();
            foreach (var inst in insts)
                items.Add(new ContentItem
                {
                    Path = inst.Path,
                    Controller = controller,
                    Action = inst.Action,
                    Subindex0 = inst.Subindex0,
                    Subindex1 = inst.Subindex1,
                    Subindex2 = inst.Subindex2,
                    Subindex3 = inst.Subindex3,
                    Subindex4 = inst.Subindex4,
                    Subindex5 = inst.Subindex5
                });
            return items;
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
