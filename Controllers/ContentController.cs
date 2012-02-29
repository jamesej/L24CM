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
using System.Collections;

namespace L24CM.Controllers
{
    public interface IContentController
    {
        List<string> SignificantRouteKeys { get; }
        Type ContentType { get; }
    }
    
    public class ContentController<TContent> : ContentController<ContentModel<TContent>, TContent>
        where TContent: BaseContent, new()
    {
        public ContentController()
            : base()
        { }
    }
    public class ContentController<TModel, TContent> : DataController<TModel>, IContentController
        where TModel : ContentModel<TContent>, new()
        where TContent : BaseContent, new()
    {
        public virtual List<string> SignificantRouteKeys
        {
            get { return new List<string>(); }
        }

        public Type ContentType
        {
            get { return typeof(TContent); }
        }

        public ContentController()
        {
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            
            this.RegisterScript("jquery", "/L24CM/Embedded/Scripts/jquery.js", ExtendedController.PrimaryInclude);
            this.RegisterScript("/L24CM/Embedded/Scripts/L24Main.js");
            //this.RegisterCss("/L24CM/Embedded/Content/L24Main.css");
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
                ViewData["FileManagerRoot"] = ConfigHelper.FileManagerRoot;
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
        public ActionResult Edit(string path, TContent update, string _l24action)
        {
            if (_l24action != null)
            {
                IList list;
                Type itemType;
                switch (_l24action.UpTo("-"))
                {
                    case "add":
                        list = ReflectionX.GetPropertyValueByPath(update, _l24action.After("-")) as IList;
                        itemType = list.GetType().GetGenericArguments()[0];
                        if (list != null)
                            list.Add(Activator.CreateInstance(itemType));
                        break;
                    case "del":
                        list = ReflectionX.GetPropertyValueByPath(update, _l24action.After("-").UpToLast("[")) as IList;
                        itemType = list.GetType().GetGenericArguments()[0];
                        if (list != null)
                            list.RemoveAt(int.Parse(_l24action.LastAfter("[").UpTo("]")));
                        break;

                }
            }
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
            RequestDataSpecification rds = new RequestDataSpecification(RouteData, Request);
            ContentItem item = new ContentItem(update, this.SignificantRouteKeys, rds);
            item = ContentRepository.Instance.AddContentItem(item);

            return View(ConfigHelper.GetViewPath("L24CMEditor.aspx"), item.Content);
        }

        [HttpGet]
        public ActionResult DualWindow(string path)
        {
            ViewData["Path"] = "/" + path;
            return View(ConfigHelper.GetViewPath("L24CMDual.aspx"));
        }
    }
}
