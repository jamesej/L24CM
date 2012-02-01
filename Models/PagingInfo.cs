﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using L24CM.Utility;

namespace L24CM.Models
{
    public class PagingInfo
    {
        private string dataPath;
        private RequestDataSpecification rds;
        private PathDataSpecification pathData;
        private string urlBase;

        public int? FullCount
        {
            get { return pathData.FilteredCount; }
        }

        public int? PageCount
        {
            get
            {
                if (FullCount.HasValue && pathData.PageLength.HasValue)
                    return (int)Math.Ceiling((double)FullCount.Value / pathData.PageLength.Value);
                else
                    return null;
            }
        }

        public int Page
        {
            get { return pathData.Page ?? 0; }
        }

        public int FirstItemIndex
        {
            get { return Page * pathData.PageLength.Value; }
        }

        public int PageNumbersWindow { get; set; }

        public PagingInfo(string dataPath, int pageNumbersWindow)
        {
            this.dataPath = dataPath;
            this.PageNumbersWindow = pageNumbersWindow;
            HttpContext httpc = HttpContext.Current;
            rds = httpc.Items["_L24DataSpec"] as RequestDataSpecification;
            pathData = rds[dataPath];
            urlBase = httpc.Request.RawUrl.UpTo("?");
        }

        public string GetPageLink(int page)
        {
            if (PageCount.HasValue && page >= PageCount.Value) page = PageCount.Value - 1;
            Dictionary<string, string> currentArgs = rds.ToArgs();
            currentArgs[dataPath + PathDataSpecification.PageSuffix] = page.ToString();
            return string.Format("{0}?{1}", urlBase, currentArgs.Select(kvp => kvp.Key + "=" + kvp.Value).Join("&"));
        }

        public string GetOffsetPageLink(int offset)
        {
            int page = Page + offset;
            if (page < 0) page = 0;
            if (PageCount.HasValue && page >= PageCount.Value) page = PageCount.Value - 1;
            return GetPageLink(page);
        }

        public IEnumerable<KeyValuePair<int,string>> GetPageNumberLinks()
        {
            if (!PageCount.HasValue)
                yield break;

            int startPage = Page - (int)Math.Floor(PageNumbersWindow / 2.0);
            if (startPage < 0) startPage = 0;
            if (startPage + PageNumbersWindow >= PageCount.Value) startPage = PageCount.Value - PageNumbersWindow;
            if (startPage < 0) startPage = 0;
            int endPage = startPage + PageNumbersWindow - 1;
            if (endPage >= PageCount.Value) endPage = PageCount.Value - 1;
            for (int page = startPage; page <= endPage; page++)
                yield return new KeyValuePair<int, string>(page, GetPageLink(page));
        }
    }
}
