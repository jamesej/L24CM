using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace L24CM
{
    public class L24View : IView
    {
        #region IView Members

        string viewBody = null;
        public L24View(string viewBody)
        {
            this.viewBody = viewBody;
        }

        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            ViewDataDictionary data = viewContext.ViewData;
            HttpServerUtilityBase server = viewContext.HttpContext.Server;
            foreach (var kvp in data)
            {
                this.viewBody = this.viewBody.Replace("${" + kvp.Key + "}", server.HtmlEncode(kvp.Value == null ? "" : kvp.Value.ToString()));
            }
            writer.Write(this.viewBody);
        }

        #endregion
    }
}
