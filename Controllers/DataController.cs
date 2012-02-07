using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using L24CM.Models;
using System.IO;
using L24CM.Attributes;

namespace L24CM.Controllers
{
    public abstract class DataController<T> : ExtendedController where T: class, new()
    {
        //protected MappedLoader<T> Loader { get; set; }
        //public RequestDataSpecification DataSpec { get; set; }
        private T model = null;
        public T Model
        {
            get { return model; }
            set { model = value; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Model == null)
                Model = new T();

            RegisterScript("jquery", "/L24CM/Embedded/Scripts/jquery.js", PrimaryInclude);
            //RegisterScript("jquery", "http://code.jquery.com/jquery-1.7.min.js", PrimaryInclude);
            //DataSpec = new RequestDataSpecification(filterContext);
            //T model = new T();
            //Loader = new MappedLoader<T>(model);
            //Loader.Loaders = GetLoaders();

            base.OnActionExecuting(filterContext);
        }

        //protected abstract LoaderDictionary GetLoaders();

        //[NonAction]
        //public void Load(ActionExecutingContext filterContext, string loadPath)
        //{
        //    if (loadPath != null)
        //        DataSpec.LoadPath = loadPath;
        //    Loader.Load(DataSpec);
        //    filterContext.ActionParameters["model"] = Loader.Loadee;
        //    // This allows templates to have access to the appropriate FilteredCount values for paging purposes.
        //    // This is a little bit 'by magic' but the alternative is forcing data and content classes to inherit
        //    // from something that allows this value to be propagated to the template through the Model which
        //    // would be unpleasantly restrictive
        //    filterContext.HttpContext.Items["_L24DataSpec"] = DataSpec;
        //}

        //[LoadData]
        //public ActionResult DisplayPartial(T model)
        //{
        //    ViewPage<T> vp = new ViewPage<T>();
        //    vp.ViewData.Model = model;
        //    var helper = new HtmlHelper<T>(new ViewContext(), new ViewPage());
        //    MvcHtmlString html;
        //    string template = HttpContext.Request.QueryString["template"];
        //    if (string.IsNullOrEmpty(template))
        //        html = helper.Display(DataSpec.Path);
        //    else
        //        html = helper.Display(DataSpec.Path, template);
        //    return Content(html.ToString());
        //}
    }
}
