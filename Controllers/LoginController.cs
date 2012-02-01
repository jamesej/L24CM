using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using L24CM.FormClasses;
using System.Web.Security;
using L24CM.Membership;
using System.Web.Routing;
using L24CM.Config;
using L24CM.Models;

namespace L24CM.Controllers
{
    public class LoginController : Controller
    {
        LightweightMembershipProvider membership = null;

        protected override void Initialize(RequestContext requestContext)
        {
            if (membership == null) { membership = System.Web.Security.Membership.Provider as LightweightMembershipProvider; }

            base.Initialize(requestContext);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(ConfigHelper.GetViewPath("L24CMLogin.aspx"));
        }

        [HttpPost]
        public ActionResult Index(LoginForm login, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
                {
                    ModelState.AddModelError("", "Please supply both a user name and password");
                }
                else
                {
                    IUser user = L24SecurityManager.Current.LoginUser(login.UserName, login.Password);
                    if (user != null)
                    {
                        //membership.ChangePassword("admin", "init", "adminlocal");
                        if (!String.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(ConfigHelper.GetViewPath("L24CMLogin.aspx"), login);
        }
    }
}
