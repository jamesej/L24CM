using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using L24CM;
using L24CM.Models;

namespace L24CM.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index(string dir)
        {
            FileUploader uploader = null;

            if (Request.QueryString["qqfile"] != null)
                uploader = new XhrFileUploader();
            else if (Request.Files["qqfile"] != null)
                uploader = new FormFileUploader();

            string uploadBasePath = Server.MapPath(dir);

            bool success = false;
            if (uploader != null)
                success = uploader.Save(uploadBasePath + "\\" + uploader.Name);

            return Content("{ 'success' : true }");
        }

    }
}
