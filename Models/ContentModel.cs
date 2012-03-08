using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

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
            get { return contentItem; }
        }

        private T content = null;
        public T Content
        {
            get
            {
                if (contentItem == null)
                {
                    contentItem = ContentRepository.Instance.GetContent(SignificantRouteKeys, ReqDataSpec);
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
