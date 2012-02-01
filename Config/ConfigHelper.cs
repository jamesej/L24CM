using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace L24CM.Config
{
    public static class ConfigHelper
    {
        public static string GetViewPath(string name)
        {
            L24CMSection l24CMSection = ConfigurationManager.GetSection("l24CM/basic") as L24CMSection;
            string baseL24CMPath = l24CMSection.L24CMAreaBaseUrl.Value;
            return "~" + baseL24CMPath + "Views/Shared/" + name;
        }
    }
}
