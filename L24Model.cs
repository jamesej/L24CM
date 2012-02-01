using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace L24CM
{
    public class L24Model
    {
        private HttpRequest request = null;
        public L24Model()
        {
            request = HttpContext.Current.Request;
        }

        public string GetFullPath(string relative)
        {
            string sitePath = request.Url.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            string fullPath = (sitePath + "/" + relative).ToLower();
            return fullPath;
        }

        public bool CanEdit
        {
            get { return true; }
        }
    }
}
