using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Collections;
using System.Net;
using System.IO;

namespace L24CM.Utility
{
    public static class WebX
    {
        public const string dummyPageKey = "_L24DummyPage";

        public static string GetWebResourceUrl(string resourceId)
        {
            IDictionary items = HttpContext.Current.Items;
            Page page = null;
            if (!items.Contains(dummyPageKey))
                items[dummyPageKey] = new Page();

            page = (Page)items[dummyPageKey];

            return page.ClientScript.GetWebResourceUrl(typeof(WebX), resourceId);
        }

        public static void SetBasicAuth(this WebRequest req, string user, string pass)
        {
            string authInfo = user + ":" + pass;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }

        public static HttpWebRequest GetPostRequest(string url, string body)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = body.Length;
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            stOut.Write(body);
            stOut.Close();
            return req;
        }
    }
}
