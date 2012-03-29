using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using L24CM.Routing;
using L24CM.Models;
using L24CM.Search;
using L24CM.Attributes;
using L24CM.Utility;
using System.Web.Routing;

namespace L24CM.Controllers
{
    public class StructureController : DataController<SiteStructure>
    {
        [Authorize(Roles=Models.User.EditorRole)]
        public ActionResult Index()
        {
            return View("L24CMStructure", SiteStructure.Current);
        }

        [HttpGet, NoCache, Authorize(Roles = Models.User.EditorRole)]
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

        [HttpPost, Authorize(Roles = Models.User.EditorRole)]
        public ActionResult AddInstance(string name, string pattern, string[] fieldNames, string[] fieldValues)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);

            if (cInfo == null)
                return new HttpStatusCodeResult(500, "No such template: " + name);

            try
            {
                int patternIdx = int.Parse(pattern);
                if (fieldNames == null)
                    fieldNames = new string[0];
                if (fieldValues == null)
                    fieldValues = new string[0];
                RouteValueDictionary rvs = new RouteValueDictionary();
                rvs.Add("controller", name);
                rvs.Add("action", cInfo.GetPatternAction(patternIdx));
                for (int i = 0; i < fieldNames.Length; i++)
                    rvs.Add(fieldNames[i], fieldValues[i]);
                foreach (var kvp in cInfo.GetPatternByDisplayIdx(patternIdx).Defaults)
                    if (!rvs.ContainsKey(kvp.Key))
                        rvs.Add(kvp.Key, kvp.Value);
                bool created = cInfo.CreateInstance(rvs);

                if (created)
                    return Content("OK");
                else
                    return new HttpStatusCodeResult(500, "Already exists");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = Models.User.EditorRole)]
        public ActionResult DeleteInstances(string name, string[] urls)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);
            cInfo.DeleteInstances(urls);
            return Content("OK");
        }

        [HttpPost, Authorize(Roles = Models.User.EditorRole)]
        public ActionResult RenameInstances(string name, string[] urls, string from, string to)
        {
            ControllerInfo cInfo = SiteStructure.Current.Controllers.FirstOrDefault(ci => ci.Name == name);
            cInfo.RenameInstances(urls, from, to);
            return Content("OK");
        }

        [HttpPost, Authorize(Roles = Models.User.EditorRole)]
        public ActionResult BuildIndex()
        {
            SearchManager.Instance.BuildIndex();

            ContentRepository.Instance.All().Do(ci => ci.SetKeys());
            ContentRepository.Instance.Save();

            return Content("OK");
        }

    }
}
