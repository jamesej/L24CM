using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace L24CM.Controllers
{
    public class FileManagerController : Controller
    {
        public ActionResult Index()
        {
            return View("L24CMFileManager");
        }

    }
}
