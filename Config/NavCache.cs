using System;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L24CM.Models;
using L24CM.Routing;

namespace L24CM.Config
{
    public class PathNode
    {
        public string Name;
        public PathNode[] Children;
    }

    public class NavCache
    {
        public static NavCache Instance
        {
            get
            {
                NavCache cache = HttpContext.Current.Cache["_L24NavCache"] as NavCache;
                if (cache == null)
                {
                    cache = new NavCache();
                    HttpContext.Current.Cache.Add("_L24NavCache", cache,
                        null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
                        CacheItemPriority.Normal, null);
                }
                return cache;
            }
        }

        public NavCache()
        {
            PathMap pathMap = PathMap.Instance;
            PathNode root = new PathNode { Name = "" };
        }

        public PathNode Root { get; private set; }
    }
}
