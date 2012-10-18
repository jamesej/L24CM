using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using L24CM.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace L24CM.Models
{
    public partial class ContentItem
    {
        private Dictionary<string, object> objectContent = null;

        public JObject JObjectContent { get; set; }

        public ContentAddress ContentAddress
        {
            get
            {
                return new ContentAddress { Controller = this.Controller, Action = this.Action, Subindexes = this.GetSubindexes() };
            }
            set
            {
                this.Controller = value.Controller;
                this.Action = value.Action;
                this.SetSubindexes(value.Subindexes);
                SetKeys();
            }
        }

        public void SetKeys()
        {
            string contentAddress = ContentAddress.ToString();
            this.VersionKey = ((Version.HasValue ? Version.Value.ToString() : "")
                + contentAddress).GetMd5Sum();
            this.AddressKey = contentAddress.GetMd5Sum();
        }

        public ContentItem() { }
        public ContentItem(BaseContent content, List<string> significantRouteKeys, RequestDataSpecification rds)
        {
            this.Type = content.GetType().FullName;
            this.Path = rds.Path;
            this.Controller = rds.Controller;
            this.Action = rds.Action;
            List<string> subindexes = rds.GetRouteValues(significantRouteKeys);
            SetSubindexes(subindexes);

            if (content == null)
                this.Content = "{}";
            else
            {
                this.Content = JsonConvert.SerializeObject(content);
            }
        }

        public void SetSubindexes(List<string> subindexes)
        {
            int c = subindexes.Count;
            if (c > 0) this.Subindex0 = subindexes[0];
            if (c > 1) this.Subindex1 = subindexes[1];
            if (c > 2) this.Subindex2 = subindexes[2];
            if (c > 3) this.Subindex3 = subindexes[3];
            if (c > 4) this.Subindex4 = subindexes[4];
            if (c > 5) this.Subindex5 = subindexes[5];
            SetKeys();
        }

        public List<string> GetSubindexes()
        {
            List<string> sis = new List<string>();

            bool started = false;
            if (this.Subindex5 != null || started) { sis.Insert(0, this.Subindex5); started = true; }
            if (this.Subindex4 != null || started) { sis.Insert(0, this.Subindex4); started = true; }
            if (this.Subindex3 != null || started) { sis.Insert(0, this.Subindex3); started = true; }
            if (this.Subindex2 != null || started) { sis.Insert(0, this.Subindex2); started = true; }
            if (this.Subindex1 != null || started) { sis.Insert(0, this.Subindex1); started = true; }
            if (this.Subindex0 != null || started) { sis.Insert(0, this.Subindex0); started = true; }

            return sis;
        }

        public PropertyInfo GetSummaryProperty(Type t)
        {
            Type summaryType = typeof(Summary);
            return t.GetProperties().FirstOrDefault(pi => summaryType.IsAssignableFrom(pi.PropertyType));
        }

        public T GetContent<T>() where T: new()
        {
            T contentObject = default(T);
            if (string.IsNullOrEmpty(this.Content))
                contentObject = new T();
            else if (this.JObjectContent != null)
                contentObject = JObjectContent.ToObject<T>();
            else
                contentObject = JsonConvert.DeserializeObject<T>(this.Content);

            // Set the summary value if record field has data and a summary property exists on the type
            if (!string.IsNullOrEmpty(this.Summary))
            {
                PropertyInfo summaryPI = GetSummaryProperty(typeof(T));
                if (summaryPI != null)
                    summaryPI.SetValue(contentObject, JsonConvert.DeserializeObject(this.Summary, summaryPI.PropertyType), null);
            }

            return contentObject;
        }

        public void SetContent<T>()
        {
            if (this.JObjectContent == null)
                throw new Exception("Setting content from uninitialised JObjectContent");
            SetContent<T>(this.JObjectContent);
        }
        public void SetContent<T>(JObject jObjectContent)
        {
            // ContentItem property is not for serialization
            jObjectContent.Remove("ContentItem");

            this.JObjectContent = jObjectContent;
            PropertyInfo summaryPI = GetSummaryProperty(typeof(T));
            if (summaryPI != null)
            {
                JToken summaryProp = jObjectContent[summaryPI.Name];
                this.Summary = summaryProp.ToString();
                JObject contentWithoutSummary = jObjectContent.DeepClone() as JObject;
                contentWithoutSummary.Remove(summaryPI.Name);
                this.Content = contentWithoutSummary.ToString();
            }
            else
                this.Content = jObjectContent.ToString();
        }

        public object GetContent()
        {
            JavaScriptSerializer jsSer = new JavaScriptSerializer();
            Type t = System.Type.GetType(this.Type);
            objectContent = jsSer.DeserializeObject(this.Content) as Dictionary<string, object>;
            // add summary data
            if (!string.IsNullOrEmpty(this.Summary))
                (jsSer.DeserializeObject(this.Summary) as Dictionary<string, object>).Do(kvp => objectContent.Add(kvp.Key, kvp.Value));
            return null;
        }

        public ContentItem Clone()
        {
            ContentItem newItem = new ContentItem
            {
                Action = this.Action,
                Content = this.Content,
                Controller = this.Controller,
                Path = this.Path,
                Type = this.Type,
                Version = this.Version
            };
            newItem.SetSubindexes(this.GetSubindexes());
            newItem.SetKeys();
            return newItem;
        }

        public object this[string propertyPath]
        {
            get
            {
                if (objectContent == null)
                    GetContent();

                object curr = objectContent;
                try
                {
                    string[] pathElements = propertyPath.Split('.');
                    foreach (string pathEl in pathElements)
                    {
                        curr = (curr as Dictionary<string, object>)[pathEl];
                    }
                }
                catch
                {
                    throw new ArgumentException("Failed to get property path at " + propertyPath);
                }
                return curr;
            }
        }
    }
}
