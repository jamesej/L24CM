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
            return View("L24CMStructure", SiteStructure.Current);
        }

    }
}
