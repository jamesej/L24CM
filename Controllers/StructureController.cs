using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using L24CM.Routing;
using L24CM.Models;
using L24CM.Search;

namespace L24CM.Controllers
{
    public class StructureController : DataController<SiteStructure>
    {
        public ActionResult Index()
        {
            return View("L24CMStructure", SiteStructure.Current);
        }

        [HttpGet, OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Instances(string name)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);

            if (cInfo == null)
                return new HttpStatusCodeResult(500, "No such template: " + name);

            var instances = new
            {
                actions = cInfo.Actions,
                routeKeys = cInfo.RouteKeys,
                pattern = cInfo.Url.StartsWith("/") ? cInfo.Url : "/" + cInfo.Url,
                defaults = cInfo.Defaults,
                insts = cInfo.Instances.Select(ci => ci.Path)
            };

            return Json(instances, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddInstance(string name, string pattern, string[] fieldNames, string[] fieldValues)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);

            if (cInfo == null)
                return new HttpStatusCodeResult(500, "No such template: " + name);

            int patternIdx = int.Parse(pattern);
            if (fieldNames == null)
                fieldNames = new string[0];
            if (fieldValues == null)
                fieldValues = new string[0];
            bool created = cInfo.CreateInstance(cInfo.GetPatternAction(patternIdx), fieldNames, fieldValues);

            if (created)
                return Content("OK");
            else
                return Content("Already Exists");
        }

        [HttpPost]
        public ActionResult DeleteInstances(string name, string[] urls)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);
            cInfo.DeleteInstances(urls);
            return Content("OK");
        }

        [HttpPost]
        public ActionResult BuildIndex()
        {
            SearchManager.BuildIndex();
            return Content("OK");
        }

    }
}
