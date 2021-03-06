﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Net.Mime;

namespace L24CM.Controllers
{
    public class HttpStatusCodeResult : ActionResult
    {
        private readonly int code;
        private readonly string description;
        public HttpStatusCodeResult(int code, string description)
        {
            this.code = code;
            this.description = description;
        }
        public HttpStatusCodeResult(int code)
            : this(code, "")
        { }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = code;
            context.HttpContext.Response.StatusDescription = description;
            if (500 <= code && code < 600)
            {
                context.HttpContext.Response.ContentType = MediaTypeNames.Text.Plain;
                context.HttpContext.Response.Write(description);
            }
        }
    }
}
