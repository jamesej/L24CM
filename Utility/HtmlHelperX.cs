using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using L24CM.Controllers;
using System.Web.Routing;

namespace L24CM.Utility
{
    public static class HtmlHelperX
    {
        public static readonly int MaxDropDownItems = 80;

        #region Include Management

        public static string RegisterScript(this HtmlHelper html, string script)
        {
            ExtendedController clr = html.ViewContext.Controller as ExtendedController;
            clr.RegisterScript(script);
            return "";
        }
        public static string RegisterScript(this HtmlHelper html, string id, string script, List<string> dependencies)
        {
            ExtendedController clr = html.ViewContext.Controller as ExtendedController;
            clr.RegisterScript(id, script, dependencies);
            return "";
        }

        public static string RegisterCss(this HtmlHelper html, string css)
        {
            ExtendedController clr = html.ViewContext.Controller as ExtendedController;
            clr.RegisterCss(css);
            return "";
        }

        public static string RegisterHtmlBlock(this HtmlHelper html, string id, MvcHtmlString htmlBlock)
        {
            return html.RegisterHtmlBlock(id, htmlBlock.ToString());
        }
        public static string RegisterHtmlBlock(this HtmlHelper html, string id, string htmlBlock)
        {
            ExtendedController clr = html.ViewContext.Controller as ExtendedController;
            clr.RegisterHtml(id, htmlBlock);
            return "";
        }

        #endregion

        private static void RegisterControlsScript(HtmlHelper html)
        {
            html.RegisterScript("_l24controls", "/L24CM/Embedded/Scripts/L24Controls.js",
                new List<string> { "/L24CM/Embedded/Scripts/jquery.js", "/L24CM/Embedded/Scripts/jquery-ui.js" });
        }

        public static MvcHtmlString StyledSelect(this HtmlHelper html, IEnumerable<SelectListItem> items, object attributes, string rightImageUrl, int rightShadowWidth, string initValue )
        {
            StringBuilder sb = new StringBuilder();
            RouteValueDictionary aDict = new RouteValueDictionary(attributes);
            aDict["class"] = (aDict["class"] ?? "") + " l24-styled-dd";
            aDict["style"] = "position: relative; display: inline-block; padding: 0px;" + (aDict["style"] ?? "");
            sb.AppendFormat("<span {0}><select style='margin: 0px; width: 100%; height: 100%; position: relative; z-index: 10; border: none; opacity: 0; -khtml-appearance: none; -webkit-appearance: none; filter: alpha(opacity=0); zoom: 1;'>",
                aDict.Select(kvp => kvp.Key + "='" + html.AttributeEncode(kvp.Value) + "'").Join(" "));
            items.Do(sli => sb.AppendFormat("<option value='{0}'{1}>{2}</option>",
                html.Encode(sli.Value),
                sli.Selected ? " selected" : "",
                html.Encode(sli.Text)));
            sb.AppendFormat("</select><span style='position: absolute; top: 0px; left: 0px; right: {0}px; bottom: 0px; z-index: 1; background: url({1}) no-repeat right; padding-left: 5px;'>{2}</span></span>",
                -rightShadowWidth,
                rightImageUrl,
                initValue ?? "&nbsp;");
            MvcHtmlString s = MvcHtmlString.Create(sb.ToString());
            RegisterControlsScript(html);
            return s;
        }

        public static MvcHtmlString BimodalAutocomplete(this HtmlHelper html, string url, int count, Func<IEnumerable<SelectListItem>> getFullList, string name )
        {
            return BimodalAutocomplete(html, url, count, getFullList, name, null);
        }
        public static MvcHtmlString BimodalAutocomplete(this HtmlHelper html, string url, int count, Func<IEnumerable<SelectListItem>> getFullList, string name, string value)
        {
            StringBuilder sb = new StringBuilder();

            string source = "'" + url + "'";

            if (count < MaxDropDownItems)
            {
                sb.AppendFormat("<div id=\"{0}\" class=\"l24-bimodal-autocomplete-container\">", name + "-container");
                source = "[" + getFullList().Select(sli => "{ label: \"" + sli.Text + "\", id: \"" + sli.Value + "\" }").Join(", ") + "]";
            }

            sb.AppendFormat("<input id=\"{0}\" name=\"{1}\" type=\"text\" class=\"l24-bimodal-autocomplete\" {2} />",
                name, name, value == null ? "" : "value=\"" + value + "\"");

            if (count < MaxDropDownItems)
            {
                sb.AppendFormat("<div id=\"{0}-button\" class=\"l24-bimodal-autocomplete-button\"><img src=\"/L24CM/Embedded/Content/Images/dropdownarrow.png\"/></div></div>",
                    name);
            }

            sb.Append("<script type='text/javascript'>if (typeof _autocompleteSources == 'undefined') _autocompleteSources = {}; _autocompleteSources['" + name + "'] = " + source + ";</script>");

            RegisterControlsScript(html);
            html.RegisterCss("/L24CM/Embedded/Content/jquery-ui.css");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
