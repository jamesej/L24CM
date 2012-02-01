﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.IO;
using System.Web.UI;

namespace L24CM.Controllers
{
    public class EmbeddedController : Controller
    {
        public ActionResult Index(string innerUrl)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string location = "L24CM." + innerUrl.Replace("/", ".");
            WebResourceAttribute attribute = assembly.GetCustomAttributes(true)
                .OfType<WebResourceAttribute>()
                .FirstOrDefault(wra => wra.WebResource == location);
            string contentType = (attribute == null ? "text/plain" : attribute.ContentType);

            Stream stream = assembly.GetManifestResourceStream(location);

            return File(stream, contentType);
        }
    }
}
