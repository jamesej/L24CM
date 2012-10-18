using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using L24CM.Routing;
using L24CM.Collation;

namespace L24CM.Models
{
    [Bind(Exclude = "Controller,SignificantRouteKeys,ContentItem,Content")]
    public class ContentModel<T> where T: BaseContent, new()
    {
        protected RequestDataSpecification reqDataSpec = null;
        protected RequestDataSpecification ReqDataSpec
        {
            get
            {
                if (reqDataSpec == null)
                {
                    if (Controller == null)
                        throw new Exception("Attempting to get RequestDataSpecification before setting Controller in ContentModel");
                    reqDataSpec = new RequestDataSpecification(Controller.RouteData, Controller.Request);
                }
                return reqDataSpec;
            }
        }

        public Controller Controller { get; set; }

        List<string> significantRouteKeys = null;
        public List<string> SignificantRouteKeys
        {
            get { return significantRouteKeys; }
            set { significantRouteKeys = value; }
        }

        ContentItem contentItem = null;
        public ContentItem ContentItem
        {
            get
            {
                if (contentItem == null)
                {
                    ContentAddress ca = new ContentAddress(ReqDataSpec, SignificantRouteKeys);
                    contentItem = ContentRoute.GetContentForAddress(ca);
                    if (contentItem == null)
                        contentItem = CollatorBuilder.Factory.Create(Controller.RouteData).GetContent(ca);
                }
                return contentItem;
            }
        }

        private T content = null;
        public T Content
        {
            get
            {
                if (content != null)
                    return content;

                if (ContentItem == null)
                    return null;

                content = ContentItem.GetContent<T>();
                content.ContentItem = ContentItem;
                return content;
            }
            set
            {
                content = value;
                if (content.ContentItem == null)
                    content.ContentItem = ContentItem;
            }
        }
    }
}
