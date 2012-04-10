using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Attributes;
using Newtonsoft.Json.Linq;
using L24CM.Models;
using L24CM.Utility;

namespace L24CM.Collation
{
    public class DefaultContentCollator : ICollator
    {
        public virtual ContentItem GetContent(ContentAddress primaryAddress)
        {
            ContentItem contentItem = ContentRepository.Instance.GetContentItem(primaryAddress);

            if (contentItem != null)
            {
                Type contentType = L24Manager.ControllerAssembly.GetType(contentItem.Type);
                contentItem = GetContent(primaryAddress, contentItem, contentType);
            }
            return contentItem;
        }
        protected virtual ContentItem GetContent(ContentAddress primaryAddress, ContentItem primaryItem, Type contentType)
        {
            var rpsAttributes = contentType
                .GetCustomAttributes(typeof(RedirectPropertySourceAttribute), false)
                .Cast<RedirectPropertySourceAttribute>()
                .ToList();
            List<ContentAddress> addresses = new List<ContentAddress>();
            if (primaryItem == null)
                addresses.Add(primaryAddress);
            addresses.AddRange(rpsAttributes
                    .Select(a => primaryAddress.Redirect(a.SourceDescriptor))
                    .Distinct());
            List<ContentItem> items = ContentRepository.Instance.GetContentItems(addresses);
            if (primaryItem == null)
                primaryItem = items.First(ci => ci.ContentAddress == primaryAddress);
            if (rpsAttributes.Any())
            {
                primaryItem.JObjectContent = JObject.Parse(primaryItem.Content);
                foreach (var rpsAttribute in rpsAttributes)
                {
                    ContentAddress refdAddress = primaryAddress.Redirect(rpsAttribute.SourceDescriptor);
                    ContentItem refdItem = items.FirstOrDefault(ci => ci.ContentAddress == refdAddress);
                    if (refdItem != null)
                        foreach (string path in rpsAttribute.Paths)
                            UpdateJObjectForPathSource(primaryItem.JObjectContent, path, refdItem);
                }
            }

            return primaryItem;
        }

        public virtual void SetContent<T>(ContentItem contentItem, T content) where T : BaseContent
        {
            var rpsAttributes = typeof(T)
                .GetCustomAttributes(typeof(RedirectPropertySourceAttribute), false)
                .Cast<RedirectPropertySourceAttribute>()
                .Where(rpsa => !rpsa.ReadOnly)
                .ToList();
            List<ContentAddress> addresses = rpsAttributes
                    .Select(a => contentItem.ContentAddress.Redirect(a.SourceDescriptor))
                    .Distinct()
                    .ToList();
            List<ContentItem> items = ContentRepository.Instance.GetContentItems(addresses);
            
            contentItem.JObjectContent = JObject.FromObject(content);

            if (rpsAttributes.Any())
            {
                foreach (var rpsAttribute in rpsAttributes)
                {
                    ContentAddress refdAddress = contentItem.ContentAddress.Redirect(rpsAttribute.SourceDescriptor);
                    ContentItem refdItem = items.FirstOrDefault(ci => ci.ContentAddress == refdAddress);
                    if (refdItem != null)
                        foreach (string path in rpsAttribute.Paths)
                            UpdateItemForPathSource(refdItem, path, contentItem.JObjectContent);
                }
            }

            contentItem.Content = contentItem.JObjectContent.ToString();

            ContentRepository.Instance.Save();
        }

        protected virtual void UpdateJObjectForPathSource(JObject jo, string path, ContentItem item)
        {
            string[] paths = GetPaths(path);
            JObject redirectJo = JObject.Parse(item.Content);
            JProperty primaryProperty = SelectProperty(jo, paths[0]) as JProperty;
            JProperty redirectProperty = SelectProperty(redirectJo, paths[1]) as JProperty;
            if (redirectProperty != null)
            {
                if (primaryProperty == null)
                    AddAtPath(jo, paths[0], redirectProperty);
                else
                    primaryProperty.Replace(redirectProperty);
            }
        }

        protected virtual void UpdateItemForPathSource(ContentItem item, string path, JObject jo)
        {
            string[] paths = GetPaths(path);
            item.JObjectContent = JObject.Parse(item.Content);
            JProperty primaryProperty = SelectProperty(jo, paths[0]) as JProperty;
            JProperty redirectProperty = SelectProperty(item.JObjectContent, paths[1]) as JProperty;
            if (primaryProperty != null)
            {
                if (redirectProperty == null)
                    AddAtPath(item.JObjectContent, paths[1], primaryProperty);
                else
                    redirectProperty.Replace(primaryProperty);
            }
            item.Content = item.JObjectContent.ToString();
        }
        
        protected virtual string[] GetPaths(string path)
        {
            if (path.Contains(">"))
                return path.Split('>').Select(s => s.Trim()).ToArray(); // primary path > redirect path
            else
                return new string[] { path, path };
        }

        protected virtual JProperty SelectProperty(JObject jo, string path)
        {
            string leafParent = path.Contains(".") ? path.UpToLast(".") : "";
            string leafProperty = path.Contains(".") ? path.LastAfter(".") : path;
            JObject parent = jo;
            if (!string.IsNullOrEmpty(leafParent))
                parent = jo.SelectToken(leafParent) as JObject;
            return parent == null ? null : parent.Property(leafProperty);
        }

        protected virtual void AddAtPath(JObject jo, string path, JProperty property)
        {
            JObject leafParent = jo;
            if (path.Contains("."))
            {
                string[] leafParents = path.UpToLast(".").Split('.');
                foreach (string propName in leafParents)
                {
                    JToken child = leafParent[propName];
                    if (child is JValue)
                    {
                        leafParent.Remove(propName);
                        child = null;
                    }
                    if (child == null)
                    {
                        child = new JObject();
                        leafParent.Add(propName, child);
                    }
                    leafParent = child as JObject;
                }
            }
            leafParent.Add(property);
        }
    }
}
