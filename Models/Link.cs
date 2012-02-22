using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace L24CM.Models
{
    public class Link
    {
        public bool IsInternal { get; set; }

        string url = null;
        public string Url
        {
            get
            {
                if (url != null)
                    return url;
                else if (!string.IsNullOrEmpty(Action))
                {
                    var uh = new UrlHelper((HttpContext.Current.Handler as MvcHandler).RequestContext);
                    return uh.Action(Action, Controller);
                }
                else
                    return null;
            }
            set { url = value; }
        }

        public string Action { get; set; }
        public string Controller { get; set; }

        public string Content { get; set; }

        public Link()
        {
            IsInternal = false;
            Content = "";
        }
    }
}
