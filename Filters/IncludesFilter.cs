using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using L24CM.Controllers;
using System.Diagnostics;
using L24CM.Utility;

namespace L24CM.Filters
{
    public class IncludesFilter : MemoryStream
    {
        StreamWriter sw;
        ExtendedController controller;

        public IncludesFilter(Stream s, ExtendedController controller)
        {
            sw = new StreamWriter(s);
            this.controller = controller;
        }

        private HtmlNode MakeNode(HtmlDocument doc, string tag, string mainAttr, string mainValue, Dictionary<string, string> createAttributes)
        {
            HtmlNode newNode = doc.CreateElement(tag);
            if (mainValue.StartsWith("javascript:"))
                newNode.AppendChild(doc.CreateTextNode(mainValue.After("javascript:")));
            else
                newNode.Attributes.Add(doc.CreateAttribute(mainAttr, mainValue));
            foreach (KeyValuePair<string, string> kvp in createAttributes)
                newNode.Attributes.Add(doc.CreateAttribute(kvp.Key, kvp.Value));
            return newNode;
        }

        public void UpdateIncludes(HtmlDocument doc, string tag, string attr, List<IncludeEntry> includes, Dictionary<string, string> createAttributes)
        {
            if (includes == null || includes.Count == 0)
                return;

            List<HtmlNode> existingIncludes = doc.DocumentNode
                .Descendants(tag)
                .Where(n => n.GetAttributeValue(attr, "") != "")
                .ToList();
            List<string> newIncludes = CreateIncludeList(
                existingIncludes.Select(n => n.GetAttributeValue(attr, "")).ToList(),
                includes);
            int pos = 0;
            foreach (HtmlNode existingIncl in existingIncludes)
            {
                while (newIncludes[pos] != existingIncl.GetAttributeValue(attr, ""))
                {
                    HtmlNode newNode = MakeNode(doc, tag, attr, newIncludes[pos], createAttributes);
                    existingIncl.ParentNode.InsertBefore(newNode, existingIncl);
                    pos++;
                }
                pos++;
            }
            HtmlNode incl = existingIncludes.LastOrDefault();
            if (incl != null)
                while (pos < newIncludes.Count)
                {
                    HtmlNode newNode = MakeNode(doc, tag, attr, newIncludes[pos], createAttributes);
                    incl.ParentNode.InsertAfter(newNode, incl);
                    pos++;
                }
            else
            {
                HtmlNode head = doc.DocumentNode.Element("html").Element("head");
                if (head != null)
                {
                    pos = newIncludes.Count - 1;
                    while (pos >= 0)
                    {
                        HtmlNode newNode = MakeNode(doc, tag, attr, newIncludes[pos], createAttributes);
                        head.PrependChild(newNode);
                        pos--;
                    }
                }
            }

        }

        public void InsertHtmls(HtmlDocument doc, List<IncludeEntry> htmls)
        {
            HtmlNode body = doc.DocumentNode.Element("html").Element("body");
            foreach (IncludeEntry html in htmls)
            {
                HtmlNode insHtml = HtmlNode.CreateNode("<div>" + html.Include + "</div>");
                if (insHtml.ChildNodes.Count == 1)
                    insHtml = insHtml.FirstChild;
                insHtml.Attributes.Add("id", html.Id);
                body.AppendChild(insHtml);
            }
        }

        public string InsertIncludes(string markup,
            List<IncludeEntry> scripts, List<IncludeEntry> csses, List<IncludeEntry> htmls)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(new StringReader(markup));

            if (csses != null)
                UpdateIncludes(doc, "link", "href", csses,
                    new Dictionary<string, string> { { "rel", "stylesheet" }, { "type", "text/css" } });

            if (scripts != null)
                UpdateIncludes(doc, "script", "src", scripts,
                    new Dictionary<string, string> { { "type", "text/javascript" } });

            if (htmls != null)
                InsertHtmls(doc, htmls);

            
            return doc.DocumentNode.OuterHtml;
        }

        public override void Close()
        {
            byte[] allBytes = this.ToArray();
            string s = System.Text.Encoding.UTF8.GetString(allBytes);

            List<IncludeEntry> csses = controller.HttpContext.Items[ExtendedController.CssItem] as List<IncludeEntry>;
            List<IncludeEntry> scripts = controller.HttpContext.Items[ExtendedController.ScriptsItem] as List<IncludeEntry>;
            List<IncludeEntry> htmls = controller.HttpContext.Items[ExtendedController.HtmlsItem] as List<IncludeEntry>;

            if (!s.Contains("</html>"))
            {
                s = string.Format("<html><head></head><body>{0}</body></html>", s);
            }

            if ((csses != null && csses.Count > 0)
                || (scripts != null && scripts.Count > 0)
                || (htmls != null && htmls.Count > 0))
                s = InsertIncludes(s, scripts, csses, htmls);

            sw.Write(s);
            sw.Flush();
            sw.Close();
            base.Close();
        }

        public List<string> CreateIncludeList(List<string> existing, List<IncludeEntry> requested)
        {
            var primaries = requested
                .Where(ie => ie.Dependencies == null)
                .Select(ie => ie.Include)
                .Concat(existing);
            // creates a list of lists where in each list each item depends on the ones before it
            List<List<string>> depLists = requested
                .Where(ie => ie.Dependencies != null)
                .Select(ie => primaries.Concat(ie.Dependencies).Concat(ie.Include).ToList())
                .ToList();

            // distinct items in lists
            List<string> items = depLists.SelectMany(list => list).Distinct().ToList();

            // build graph of ordering information
            ArrayGraph<string, bool> orderingGraph = new ArrayGraph<string, bool>(items) { Unidirectional = true };

            for (int i = 0; i < depLists.Count; i++)
                for (int j = 1; j < depLists[i].Count; j++)
                    for (int k = j - 1; k >= 0; k--)
                        orderingGraph[depLists[i][k], depLists[i][j]] = true;

            // use it to order includes
            List<string> result = items
                .PartialOrderBy(s => s, orderingGraph)
                .ToList();

            return result;
        }

        //public List<string> CreateIncludeList(List<string> existing, List<IncludeEntry> requested)
        //{
        //    List<string> res = new List<string>();
        //    int startPos = 0;
        //    int pos;
        //    int inclPos;

        //    requested = requested.OrderBy(incl => incl.Dependencies == null
        //        ? 0
        //        : (incl.Dependencies.Count == 0 ? int.MaxValue : incl.Dependencies.Count))
        //        .ToList();


        //    // inserts requesteds plus dependencies in appropriate
        //    // order into res
        //    foreach (IncludeEntry incl in requested)
        //    {
        //        pos = startPos;
        //        inclPos = res.IndexOf(incl.Include);
        //        if (inclPos < 0) inclPos = int.MaxValue;
        //        if (incl.Dependencies != null && incl.Dependencies.Count == 0) // prioritise include with empty list of dependencies
        //        {
        //            if (inclPos == int.MaxValue)
        //                res.Insert(startPos++, incl.Include);
        //        }
        //        else
        //        {
        //            if (incl.Dependencies != null)
        //                foreach (string dep in incl.Dependencies)
        //                {
        //                    int depPos = res.IndexOf(dep);
        //                    if (depPos < 0) // dependency not in output list
        //                    {
        //                        res.Insert(pos, dep); // insert at next available pos
        //                        pos++;
        //                    }
        //                    else if (depPos >= pos) // dependency in list
        //                    {
        //                        pos = depPos + 1;
        //                        if (pos >= inclPos)
        //                            throw new Exception(string.Format("While positioning '{0}' dependency '{1}' found after it in list",
        //                                incl.Include, dep, res[pos - 1]));
        //                    }
        //                    else
        //                        throw new Exception(string.Format("While positioning '{0}' dependency '{1}' can't come before '{2}'",
        //                            incl.Include, dep, res[pos - 1]));
        //                }

        //            if (inclPos == int.MaxValue)
        //                res.Add(incl.Include);
        //        }
        //    }

        //    pos = startPos;
        //    foreach (string incl in existing)
        //    {
        //        inclPos = res.IndexOf(incl);
        //        if (inclPos < 0)
        //        {
        //            res.Insert(pos, incl);
        //            pos++;
        //        }
        //        else if (inclPos >= pos)
        //            pos = inclPos + 1;
        //        else
        //            throw new Exception(string.Format("Can't position {0} before {1} as includes in page are other way round",
        //                    res[pos - 1], incl));
        //    }


        //    if (res.Count > 0)
        //        Debug.WriteLine(string.Format("{0}\n<{1}>", this.controller.Request.Url.ToString(),
        //            res.Join(", ")));

        //    return res;
        //}
    }
}
