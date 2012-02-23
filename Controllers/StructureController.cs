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

        [HttpGet]
        public ActionResult Instances(string name)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);

            if (cInfo == null)
                return new HttpStatusCodeResult(500, "No such template: " + name);

            var instances = new
            {
                actions = cInfo.Actions,
                routeKeys = cInfo.RouteKeys
            };

            return Json(instances, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddInstance(string name, string[] fieldNames, string[] fieldValues)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);

            if (cInfo == null)
                return new HttpStatusCodeResult(500, "No such template: " + name);

            cInfo.Create(fieldNames, fieldValues);

            return Content("OK");
        }

    }
}
