using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using L24CM.Models;
using L24CM.Utility;
using System.Reflection;
using System.IO;
using System.Web.UI;
using System.Web.Script.Serialization;
using L24CM.Membership;
using System.Text;
using System.Web.Routing;
using L24CM.Attributes;
using System.Web.Security;
using L24CM.Config;
using System.Configuration;

namespace L24CM.Controllers
{
    public abstract class ContentController<TContent> : ContentController<ContentModel<TContent>, TContent>
        where TContent: BaseContent, new()
    {
    }
    public abstract class ContentController<TModel, TContent> : DataController<TModel>
        where TModel : ContentModel<TContent>, new()
        where TContent : BaseContent, new()
    {
        abstract public List<string> SignificantRouteKeys { get; }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            
            this.RegisterScript("jquery", "/L24CM/Embedded/Scripts/jquery.js", ExtendedController.PrimaryInclude);
            this.RegisterScript("/L24CM/Embedded/Scripts/L24Main.js");
            this.RegisterCss("/L24CM/Embedded/Content/L24Main.css");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Model == null)
            {
                Model = new TModel();
                Model.Controller = this;
                Model.SignificantRouteKeys = this.SignificantRouteKeys;
            }

            if (Model.Content == null
                && !(filterContext.ActionDescriptor.ActionName == "Create" && Roles.IsUserInRole(L24CM.Models.User.AdminRole)))
            {
                filterContext.Result = new HttpStatusCodeResult(404);
                return;
            }

            if (Roles.IsUserInRole(L24CM.Models.User.EditorRole)
                && !filterContext.IsChildAction               // not called as part of another request
                && RouteData.Values["originalAction"] == null // this is not a divert
                && Request.RequestType == "GET"
                && (Request.QueryString["-mode"] ?? "").ToLower() != "view")
            {
                ViewData["Path"] = "/" + (string)RouteData.Values["path"];
                filterContext.Result = View(ConfigHelper.GetViewPath("L24CMDual.aspx"));
                return;
            }

            base.OnActionExecuting(filterContext);

            //Loader.Loaders.Add("Content", rds => 
            //    {
            //        ContentItem contentItem = filterContext.HttpContext.Items["_L24Content"] as ContentItem;
            //        if (contentItem == null) return null;
            //        return contentItem.GetContent<TContent>();
            //    });
        }

        //protected override LoaderDictionary GetLoaders()
        //{
        //    return new LoaderDictionary();
        //}

        //
        // GET: /Content/

        [HttpGet]
        [Authorize(Roles=Models.User.EditorRole)]
        public ActionResult Edit()
        {
            return View(ConfigHelper.GetViewPath("L24CMEditor.aspx"), Model.Content);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles=Models.User.EditorRole)]
        public ActionResult Edit(string path, TContent update)
        {
            JavaScriptSerializer jsSer = new JavaScriptSerializer();
            Model.ContentItem.Content = jsSer.Serialize(update);
            ContentRepository.Instance.Save();  // Model.ContentItem originated from ContentRepository.

            return View(ConfigHelper.GetViewPath("L24CMEditor.aspx"), update);
        }

        [HttpGet]
        [Authorize(Roles=Models.User.AdminRole)]
        public ActionResult Create()
        {
            return View(ConfigHelper.GetViewPath("L24CMEditor.aspx"), new TContent());
        }


        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles=Models.User.AdminRole)]
        public ActionResult Create(string path, TContent update)
        {
            L24CMEntities ctx = new L24CMEntities();
            ContentItem content = ctx.ContentItemSet.FirstOrDefault(c => c.Path == path);
            if (content == null)
            {
                RequestDataSpecification rds = new RequestDataSpecification(RouteData, Request);
                content = new ContentItem(update, this.SignificantRouteKeys, rds);
                ctx.AddToContentItemSet(content);
            }

            ctx.SaveChanges();

            return View(ConfigHelper.GetViewPath("L24CMEditor.aspx"), update);
        }

        [HttpGet]
        public ActionResult DualWindow(string path)
        {
            ViewData["Path"] = "/" + path;
            return View(ConfigHelper.GetViewPath("L24CMDual.aspx"));
        }
    }
}
