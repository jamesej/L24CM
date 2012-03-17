using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using L24CM.Utility;

namespace L24CM.Routing
{
    public class UrlPattern
    {
        public string Url { get; set; }
        public RouteValueDictionary Defaults { get; set; }

        public List<string> UrlVariables
        {
            get
            {
                return Url.Split('{').Where(s => s.Contains("}")).Select(s => s.UpTo("}")).ToList();
            }
        }

        public bool Matches(RouteValueDictionary rvs)
        {
            Dictionary<string, object> adjustedRvs = rvs.ToDictionary(rv => rv.Key, rv => rv.Value); // clone
            if (adjustedRvs.ContainsKey("originalAction"))
            {
                adjustedRvs["action"] = adjustedRvs["originalAction"];
                adjustedRvs.Remove("originalAction");
            }
            return adjustedRvs.All(kvp => kvp.Key == "controller"
                                  || !Defaults.ContainsKey(kvp.Key)
                                  || Defaults.Contains(kvp))
                   && UrlVariables.All(uv => adjustedRvs.ContainsKey(uv));
        }

        public string BuildUrl(RouteValueDictionary rvs)
        {
            string url = Url;
            foreach (KeyValuePair<string, object> kvp in rvs)
            {
                if (Defaults.Contains(kvp))
                    url = url.Replace("{" + kvp.Key + "}", "").Replace("//", "/");
                else
                    url = url.Replace("{" + kvp.Key + "}", (string)kvp.Value);
            }
            if (!url.StartsWith("/"))
                url = "/" + url;
            if (url.EndsWith("/"))
                url = url.UpToLast("/");
            return url;
        }

        private string SubstAction(string pat, string action)
        {
            if (Defaults.ContainsKey("action") && (string)Defaults["action"] == action)
            {
                pat = pat.Replace("{action}", "").Replace("//", "/");
                return pat.EndsWith("/") ? pat.UpToLast("/") : pat;
            }
            else
                return pat.Replace("{action}", action);
        }

        public List<string> DisplayPatterns(string controller, List<string> actions)
        {
            string pat = this.Url.Replace("{controller}", controller);
            if (pat.Contains("{action}"))
                return actions.Select(a => SubstAction(pat, a)).ToList();
            else
                return new List<string> { pat };
        }

        public List<string> DisplayActions(List<string> actions)
        {
            if (this.Url.Contains("{action}"))
                return actions;
            else
                return new List<string> { (string)Defaults["action"] };
        }
    }

}
