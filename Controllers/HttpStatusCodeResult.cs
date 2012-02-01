using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace L24CM.Controllers
{
    public class HttpStatusCodeResult : ActionResult
    {
        private readonly int code;
        public HttpStatusCodeResult(int code)
        {
            this.code = code;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = code;
        }
    }
}
