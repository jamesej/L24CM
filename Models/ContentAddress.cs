using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Models
{
    public class ContentAddress
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public List<string> Subindexes { get; set; }

        public ContentAddress()
        { }
        public ContentAddress(RequestDataSpecification rds, List<string> significantRouteKeys)
        {
            Controller = rds.Controller;
            Action = rds.Action;
            Subindexes = rds.GetRouteValues(significantRouteKeys);
        }

        public ContentAddress Clone()
        {
            ContentAddress ca = new ContentAddress();
            ca.Controller = this.Controller;
            ca.Action = this.Action;
            ca.Subindexes = this.Subindexes.ToList();
            return ca;
        }

        public ContentAddress Redirect(string redirectDescriptor)
        {
            ContentAddress ca = Clone();
            ca.Subindexes = ca.Subindexes.Select(si => (string)null).ToList();
            return ca;
        }
    }
}
