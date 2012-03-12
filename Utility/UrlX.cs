using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using L24CM.Models;
using L24CM.Routing;

namespace L24CM.Utility
{
    public static class UrlX
    {
        static public string ConvertUrlsToAbsolute(string s)
        {
            StringBuilder res = new StringBuilder();

            int pos = 0;
            string section;
            string[] seps;
            while (true)
            {
                section = s.GetHead(ref pos, new string[] { "href=\"", "href='", "src=\"", "src='", "background=\"", "background='", "location.replace(\"", "location.replace('", "url: '", "url: \"", ":url('", ":url(\"" }, true);
                res.Append(section);
                if (pos == -1) break;
                if (pos < s.Length && s[pos] == '\"')
                    continue;
                seps = new string[] { StringX.Right(section, 1) }; // Ensure we are scanning for the single or double quote appropriately
                res.Append(UrlToAbsolute(s.GetHead(ref pos, seps, false)));
                res.Append(seps[0]);
            }

            return res.ToString();
        }

        static public string UrlToAbsolute(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("/") || url.StartsWith("javascript:"))
                return url;

            StringBuilder res = new StringBuilder();
            Uri thisUri = HttpContext.Current.Request.Url;
            res.Append(thisUri.Scheme);
            res.Append("://");
            res.Append(thisUri.Host);
            for (int i = 0; i < thisUri.Segments.Length - 1; i++)
            {
                if (thisUri.Segments[i].EndsWith(".aspx") || thisUri.Segments[i].EndsWith(".aspx/"))
                    break;
                res.Append(thisUri.Segments[i]);
            }
            res.Append(url);

            return res.ToString();
        }

        static public string Action(this UrlHelper urls, ContentAddress addr)
        {
            return SiteStructure.Current.GetUrl(addr);
        }
    }
}
