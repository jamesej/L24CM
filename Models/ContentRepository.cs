using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Controllers;
using L24CM.Utility;
using System.Web;
using System.Collections;
using System.Linq.Dynamic;
using L24CM.Attributes;
using Newtonsoft.Json.Linq;

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

        public virtual ContentItem GetContentItem(List<string> significantRouteKeys, RequestDataSpecification rds)
        {
            return GetContentItem(new ContentAddress(rds, significantRouteKeys));
        }
        public virtual ContentItem GetContentItem(ContentAddress ca)
        {
            // TODO: Versioning
            string addressKey = ca.ToString().GetMd5Sum();
            ContentItem contentItem = Ctx.ContentItemSet.FirstOrDefault(c => c.AddressKey == addressKey);

            return contentItem;
        }

        public virtual List<ContentItem> GetContentItems(List<ContentAddress> cas)
        {
            if (cas == null || cas.Count == 0)
                return new List<ContentItem>();

            List<ContentItem> items = Ctx.ContentItemSet
                .WhereIn(c => c.AddressKey, cas.Select(ca => ca.ToString().GetMd5Sum()))
                .ToList();
            return items;
        }

        //public virtual ContentItem GetContent(List<string> significantRouteKeys, RequestDataSpecification rds)
        //{
        //    ContentAddress primaryAddress = new ContentAddress(rds, significantRouteKeys);
        //    ContentItem contentItem = GetContentItem(primaryAddress);

        //    if (contentItem != null)
        //    {
        //        Type contentType = L24Manager.ControllerAssembly.GetType(contentItem.Type);
        //        var rpsAttributes = contentType
        //            .GetCustomAttributes(typeof(RedirectPropertySourceAttribute), false)
        //            .Cast<RedirectPropertySourceAttribute>()
        //            .ToList();
        //        if (rpsAttributes.Any())
        //        {
        //            contentItem.JObjectContent = JObject.Parse(contentItem.Content);
        //            foreach (var rpsAttribute in rpsAttributes)
        //                foreach (string path in rpsAttribute.Path.Split('/'))
        //                    UpdateJObjectForPathSource(contentItem.JObjectContent, path, rpsAttribute.SourceDescriptor, primaryAddress);
        //        }
        //    }
        //    return contentItem;
        //}

        //protected virtual void UpdateJObjectForPathSource(JObject jo, string path, string sourceDescriptor, ContentAddress address)
        //{
        //    ContentAddress redirectAddress = address.Redirect(sourceDescriptor);
        //    ContentItem redirectContent = GetContentItem(redirectAddress);
        //    if (redirectContent != null)
        //    {
        //        JObject redirectJo = JObject.Parse(redirectContent.Content);
        //        JProperty primaryProperty = jo.Property(path);
        //        JProperty redirectProperty = redirectJo.Property(path);
        //        if (redirectProperty != null)
        //        {
        //            if (primaryProperty == null)
        //                jo.Add(redirectProperty);
        //            else
        //                primaryProperty.Replace(redirectProperty);
        //        }
        //    }
        //}

        public virtual ContentItem AddContentItem(ContentItem item)
        {
            item.SetKeys();
            ContentItem existing = Ctx.ContentItemSet.FirstOrDefault(ci => ci.AddressKey == item.AddressKey);
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

        public virtual void Delete(ContentItem item)
        {
            Ctx.DeleteObject(item);
        }

        public virtual void DeleteByUrls(string[] urls)
        {
            Ctx.ContentItemSet
                .WhereIn(ci => ci.Path, urls)
                .AsEnumerable()
                .Do(ci => Ctx.DeleteObject(ci));
            Ctx.SaveChanges();
        }

        public virtual ContentItem[] GetByUrls(string[] urls)
        {
            return Ctx.ContentItemSet
                        .WhereIn(ci => ci.Path, urls)
                        .ToArray();
        }

        public virtual void Save()
        {
            Ctx.SaveChanges();
        }

        public virtual IQueryable<ContentItem> All()
        {
            return Ctx.ContentItemSet;
        }

        public virtual IEnumerable<AddressedContent<T>> GetContent<T>(IEnumerable<ContentAddress> addresses) where T : BaseContent, new()
        {
            var query = Ctx.ContentItemSet
                .WhereIn(ci => ci.AddressKey, addresses.Select(ca => ca.ToString().GetMd5Sum()))
                .AsEnumerable()
                .Select(ci => new AddressedContent<T> { Address = ci.ContentAddress, Content = ci.GetContent<T>() });
            return query;
        }
    }
}
