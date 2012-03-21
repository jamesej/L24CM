using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Attributes;
using Newtonsoft.Json.Linq;

namespace L24CM.Models
{
    public class ContentCollator
    {
        public virtual ContentItem GetContent(List<string> significantRouteKeys, RequestDataSpecification rds)
        {
            return GetContent(new ContentAddress(rds, significantRouteKeys));

        }
        public ContentItem GetContent(ContentAddress primaryAddress)
        {
            ContentItem contentItem = ContentRepository.Instance.GetContentItem(primaryAddress);

            if (contentItem != null)
            {
                Type contentType = L24Manager.ControllerAssembly.GetType(contentItem.Type);
                contentItem = GetContent(primaryAddress, contentItem, contentType);
            }
            return contentItem;
        }
        public virtual ContentItem GetContent(ContentAddress primaryAddress, ContentItem primaryItem, Type contentType)
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
                    ContentItem refdItem = items.First(ci => ci.ContentAddress == refdAddress);

                    foreach (string path in rpsAttribute.Path.Split('/'))
                        UpdateJObjectForPathSource(primaryItem.JObjectContent, path, refdItem);
                }
            }

            return null;
        }

        protected virtual void UpdateJObjectForPathSource(JObject jo, string path, ContentItem item)
        {
                JObject redirectJo = JObject.Parse(item.Content);
                JProperty primaryProperty = jo.Property(path);
                JProperty redirectProperty = redirectJo.Property(path);
                if (redirectProperty != null)
                {
                    if (primaryProperty == null)
                        jo.Add(redirectProperty);
                    else
                        primaryProperty.Replace(redirectProperty);
                }
        }
    }
}
