using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using L24CM.Routing;
using L24CM.Controllers;

namespace L24CM.Collation
{
    public class DefaultCollatorFactory : ICollatorFactory
    {
        public ICollator Create(RouteData rd)
        {
            ControllerInfo cInfo = SiteStructure.Current[rd.Values["controller"] as string];
            IContentController cc = Activator.CreateInstance(cInfo.Controller) as IContentController;
            return cc.GetCollator();
        }
    }
}
