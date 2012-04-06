using System.Web.Mvc;

namespace L24CM
{
    public class L24CMAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "L24CM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("l24embedded",
                "L24CM/Embedded/{*innerUrl}",
                new { controller = "Embedded", action = "Index" });
            // Get dynamically generated content
            context.MapRoute("l24dynamic",
                "L24CM/Dynamic/{action}/{*urlTail}",
                new { controller = "Dynamic" });
            context.MapRoute("l24default",
                "L24CM/{controller}/{action}",
                new { controller = "Ajax", action = "Index" }
            );
        }
    }
}
