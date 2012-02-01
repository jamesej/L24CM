using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using L24CM.Utility;

namespace L24CM.Controllers
{
    public class AjaxController : Controller
    {
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult FileTreeFolders(string dir)
        {
            if (string.IsNullOrEmpty(dir)) dir = "/";

            DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath(dir));
            var output = di.GetDirectories()
                            .OrderBy(cdi => cdi.Name)
                            .Select(cdi => new
                                {
                                    data = new { title = cdi.Name },
                                    state = "closed",
                                    attr = new { title = dir + cdi.Name + "/" }
                                }).ToArray();
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult FileTreeFiles(string dir)
        {
            if (string.IsNullOrEmpty(dir)) dir = "/";

            DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath(dir));
            var output = new
            {
                dir = !di.Exists ? null : dir + (dir[dir.Length - 1] == '/' ? "" : "/"),
                dirs = !di.Exists ? null : di.GetDirectories()
                    .OrderBy(cdi => cdi.Name)
                    .Select(cdi => new
                        {
                            name = cdi.Name
                        }).ToArray(),
                files = !di.Exists ? null : di.GetFiles()
                    .OrderBy(cfi => cfi.Name)
                    .Select(cfi => new
                        {
                            name = cfi.Name,
                            ext = cfi.Extension.Substring(1),
                            size = cfi.Length
                        }).OrderBy(inf => inf.name).ToArray()
            };
            return Json(output, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Rename(string path, string newName)
        {
            try
            {
                string filePath = Server.MapPath(path);
                if (filePath.EndsWith("\\")) filePath = filePath.Substring(0, filePath.Length - 1);
                string newFilePath = filePath.UpToLast("\\") + "\\" + newName;
                System.IO.Directory.Move(filePath, newFilePath);
            }
            catch
            {
                return Json("");
            }
            return Json(path.UpToLast("/") + "/" + newName);
        }

        [HttpPost]
        public ActionResult Move(string path, string newDir)
        {
            try
            {
                string filePath = Server.MapPath(path);
                if (filePath.EndsWith("\\")) filePath = filePath.Substring(0, filePath.Length - 1);
                string newDirPath = Server.MapPath(newDir);
                if (newDirPath.EndsWith("\\")) newDirPath = newDirPath.Substring(0, newDirPath.Length - 1);
                string newFilePath = newDirPath + "\\" + filePath.LastAfter("\\");
                System.IO.Directory.Move(filePath, newFilePath);
            }
            catch
            {
                return Json(false);
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult Delete(string path)
        {
            try
            {
                string filePath = Server.MapPath(path);
                if (filePath.EndsWith("\\"))
                {
                    filePath = filePath.Substring(0, filePath.Length - 1);
                    DirectoryInfo d = new DirectoryInfo(filePath);
                    d.DeleteRecursive();
                }
                else
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch
            {
                return Json(false);
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult CreateFolder(string path)
        {
            return Json(true);
        }
    }
}
