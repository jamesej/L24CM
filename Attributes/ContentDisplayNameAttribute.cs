using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using L24CM.Models;

namespace L24CM.Attributes
{
    public class ContentDisplayNameAttribute : DisplayNameAttribute
    {
        string contentField = null;
        Type targetType = null;

        public override string DisplayName
        {
            get
            {
                L24CMEntities ctx = new L24CMEntities();
                ContentItem item = ctx.ContentItemSet.FirstOrDefault(ci => ci.Path == targetType.FullName);
                return item[contentField] as string;
            }

        }

        public ContentDisplayNameAttribute(string contentField, Type targetType)
        {
            this.contentField = contentField;
            this.targetType = targetType;
        }
    }
}
