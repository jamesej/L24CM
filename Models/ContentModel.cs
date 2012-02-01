using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace L24CM.Models
{
    public class ContentModel<T> where T: BaseContent, new()
    {
        protected RequestDataSpecification reqDataSpec = null;

        Controller controller = null;
        public Controller Controller
        {
            get { return controller; }
            set
            {
                controller = value;
                this.reqDataSpec = new RequestDataSpecification(controller.RouteData, controller.Request);
            }
        }

        List<string> significantRouteKeys = null;
        public List<string> SignificantRouteKeys
        {
            get { return significantRouteKeys; }
            set { significantRouteKeys = value; }
        }

        ContentItem contentItem = null;
        public ContentItem ContentItem
        {
            get { return contentItem; }
        }

        private T content = null;
        public T Content
        {
            get
            {
                if (contentItem == null)
                {
                    contentItem = ContentRepository.Instance.GetContent(SignificantRouteKeys, reqDataSpec);
                    if (contentItem == null)
                        content = null;
                    else
                    {
                        content = contentItem.GetContent<T>();
                        content.ContentItem = contentItem;
                    }
                }
                return content;
            }
            set { content = value; }
        }
    }
}
