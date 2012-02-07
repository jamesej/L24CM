using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.IO;
using System.Web.UI;

namespace L24CM.Controllers
{
    public class DynamicController : Controller
    {
        public ActionResult DatesJs()
        {
            return View("L24CMDatesJs");
        }
    }
}
