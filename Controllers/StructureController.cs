using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using L24CM.Models;

namespace L24CM.Controllers
{
    public class StructureController : DataController<SiteStructure>
    {
        public ActionResult Index()
        {
            UrlHelper urls = new UrlHelper((System.Web.HttpContext.Current.Handler as MvcHandler).RequestContext);

            string url = urls.Action("*1", "Ajax");
            url = urls.Action("Index", "Ajax");
            return View("L24CMStructure");
        }

    }
}
