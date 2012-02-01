using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace L24CM
{
    public static class HtmlExtensions
    {
        public static L24HtmlHelper C2(this HtmlHelper helper, L24Model model)
        {
            return new L24HtmlHelper(model);
        }
    }

    public class L24HtmlHelper
    {
        public L24Model Model { get; private set; }

        public L24HtmlHelper(L24Model model)
        {
            this.Model = model;
        }

        //public MvcHtmlString Text(string path)
        //{
        //    Mvc2MsEntities ctx = new Mvc2MsEntities();
        //    string fullPath = Model.GetFullPath(path);
        //    Content content = ctx.ContentSet.FirstOrDefault(c => c.path == fullPath);
        //    string text = "<missing text>";
        //    if (content != null)
        //        text = content.content;
        //    string html = "<span class='c2_" + path + "'>" + text + "</span>";
        //    return MvcHtmlString.Create(text);
        //}
    }
}
