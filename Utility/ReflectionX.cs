using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace L24CM.Utility
{
    public static class ReflectionX
    {
        public static object GetPropertyValueByPath(object o, string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            string[] pathEls = path.Split('.');
            Type propType = o.GetType();
            PropertyInfo propInfo = null;
            object val = o;
            foreach (string pathEl in pathEls)
            {
                propInfo = propType.GetProperty(pathEl.UpTo("["));
                val = propInfo.GetValue(val, null);
                if (pathEl.Contains("["))
                {
                    object[] indexes = new object[] { int.Parse(pathEl.After("[").UpTo("]")) };
                    propInfo = propInfo.PropertyType.GetProperty("Item");
                    val = propInfo.GetValue(val, indexes);
                }
                propType = propInfo.PropertyType;
            }
            return val;
        }
    }
}
