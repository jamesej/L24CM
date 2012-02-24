using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using L24CM.Utility;

namespace L24CM.Models
{
    public partial class ContentItem
    {
        private Dictionary<string, object> objectContent = null;

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
                JavaScriptSerializer jsSer = new JavaScriptSerializer();
                this.Content = jsSer.Serialize(content);
            }
        }

        public void SetSubindexes(List<string> subindexes)
        {
            int c = subindexes.Count;
            if (c < 1) return;
            this.Subindex0 = subindexes[0];
            if (c < 2) return;
            this.Subindex1 = subindexes[1];
            if (c < 3) return;
            this.Subindex2 = subindexes[2];
            if (c < 4) return;
            this.Subindex3 = subindexes[3];
            if (c < 5) return;
            this.Subindex4 = subindexes[4];
            if (c < 6) return;
            this.Subindex5 = subindexes[5];
        }

        public List<string> GetSubindexes()
        {
            List<string> sis = new List<string>();
            if (this.Subindex0 != null) sis.Add(this.Subindex0);
            if (this.Subindex1 != null) sis.Add(this.Subindex1);
            if (this.Subindex2 != null) sis.Add(this.Subindex2);
            if (this.Subindex3 != null) sis.Add(this.Subindex3);
            if (this.Subindex4 != null) sis.Add(this.Subindex4);
            if (this.Subindex5 != null) sis.Add(this.Subindex5);
            return sis;
        }

        public T GetContent<T>() where T: new()
        {
            JavaScriptSerializer jsSer = new JavaScriptSerializer();
            T contentObject = default(T);
            if (string.IsNullOrEmpty(this.Content))
                contentObject = new T();
            else
            {
                contentObject = jsSer.Deserialize<T>(this.Content);
            }
            return contentObject;
        }

        public object GetContent()
        {
            JavaScriptSerializer jsSer = new JavaScriptSerializer();
            Type t = System.Type.GetType(this.Type);
            objectContent = jsSer.DeserializeObject(this.Content) as Dictionary<string, object>;
            return null;
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
