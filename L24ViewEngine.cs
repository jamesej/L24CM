using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.IO;

namespace L24CM
{
    /// <summary>
    /// Adds in search paths to Areas/L24CM/Views/Shared as defaults
    /// </summary>
    public class L24ViewEngine : WebFormViewEngine
    {
        public L24ViewEngine()
        {
            var addLocations = new[]
            {
                "~/Areas/L24CM/Views/Shared/{0}.aspx",
                "~/Areas/L24CM/Views/Shared/{0}.ascx"
            };

            ViewLocationFormats = ViewLocationFormats.Concat(addLocations).ToArray();
            PartialViewLocationFormats = PartialViewLocationFormats.Concat(addLocations).ToArray();

            AreaViewLocationFormats = AreaViewLocationFormats.Concat(addLocations).ToArray();
            AreaPartialViewLocationFormats = AreaPartialViewLocationFormats.Concat(addLocations).ToArray();
        }
    }
}
