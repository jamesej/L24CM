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

    public class MyWebFormViewEngine : WebFormViewEngine
    {
        public MyWebFormViewEngine()
            : base()
        {
            MasterLocationFormats = new[] {
            "~/Views/%1/{1}/{0}.master",
            "~/Views/%1/Shared/{0}.master"
            };

            AreaMasterLocationFormats = new[] {
            "~/Areas/{2}/Views/%1/{1}/{0}.master",
            "~/Areas/{2}/Views/%1/Shared/{0}.master",
            };

            ViewLocationFormats = new[] {
            "~/Views/%1/{1}/{0}.aspx",
            "~/Views/%1/{1}/{0}.ascx",
            "~/Views/%1/Shared/{0}.aspx",
            "~/Views/%1/Shared/{0}.ascx"
            };

            AreaViewLocationFormats = new[] {
            "~/Areas/{2}/Views/%1/{1}/{0}.aspx",
            "~/Areas/{2}/Views/%1/{1}/{0}.ascx",
            "~/Areas/{2}/Views/%1/Shared/{0}.aspx",
            "~/Areas/{2}/Views/%1/Shared/{0}.ascx",
            };

            PartialViewLocationFormats = ViewLocationFormats;
            AreaPartialViewLocationFormats = AreaViewLocationFormats;

            var addLocations = new[]
            {
                "~/Views/%1/{0}.aspx",
                "~/Views/%1/{0}.ascx"
            };


            ViewLocationFormats = ViewLocationFormats.Concat(addLocations).ToArray();
            PartialViewLocationFormats = PartialViewLocationFormats.Concat(addLocations).ToArray();

            AreaViewLocationFormats = AreaViewLocationFormats.Concat(addLocations).ToArray();
            AreaPartialViewLocationFormats = AreaPartialViewLocationFormats.Concat(addLocations).ToArray();
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            var nameSpace = controllerContext.Controller.GetType().Namespace;
            return base.CreatePartialView(controllerContext, partialPath.Replace("%1", nameSpace));
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            var nameSpace = controllerContext.Controller.GetType().Namespace;
            return base.CreateView(controllerContext, viewPath.Replace("%1", nameSpace), masterPath.Replace("%1", nameSpace));
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            var nameSpace = controllerContext.Controller.GetType().Namespace;
            return base.FileExists(controllerContext, virtualPath.Replace("%1", nameSpace));
        }
    }

}
