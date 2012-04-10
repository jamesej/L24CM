using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Models;

namespace L24CM.Collation
{
    public interface ICollator
    {
        ContentItem GetContent(ContentAddress primaryAddress);
        void SetContent<T>(ContentItem contentItem, T content) where T : BaseContent;
    }
}
