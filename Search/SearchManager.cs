using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using L24CM.Models;
using Lucene.Net.Documents;
using L24CM.Attributes;
using Newtonsoft.Json.Linq;
using L24CM.Utility;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace L24CM.Search
{
    public class SearchManager
    {
        public static SearchManager Instance { get; set; }

        static SearchManager()
        {
            Instance = new SearchManager();
        }

        public virtual string GetIndexFilePath()
        {
            return HttpContext.Current.Server.MapPath("/App_Data/Lucene");
        }

        public void BuildIndex()
        {
            //state the file location of the index
            string indexFileLocation = GetIndexFilePath();
            Directory dir =
                Lucene.Net.Store.FSDirectory.GetDirectory(indexFileLocation, true);

            //create an analyzer to process the text
            Analyzer analyzer = new StandardAnalyzer();

            //create the index writer with the directory and analyzer defined.
            IndexWriter indexWriter = new
            IndexWriter(dir, analyzer, true); /*true to create a new index*/ 

            foreach (ContentItem contentItem in ContentRepository.Instance.All())
            {
                Type type = L24Manager.ControllerAssembly.GetType(contentItem.Type);
                // only index pages
                if (!typeof(PageContent).IsAssignableFrom(type))
                    continue;

                var indexedProps = type.GetProperties()
                    .Select(pi => new { PropPath = pi.Name, IndexAttr = pi.GetCustomAttributes(typeof(IndexAttribute), false).FirstOrDefault() as IndexAttribute })
                    .Where(ip => ip.IndexAttr != null)
                    .ToList();

                // type has no indexing
                if (indexedProps.Count == 0)
                    continue;

                JObject jo = JObject.Parse(contentItem.Content);

                Document doc = new Document();

                indexedProps
                    .Where(ip => ip.IndexAttr.IndexMode != IndexAttribute.Mode.Agglomerate)
                    .Select(ip =>
                    new Field(ip.PropPath,
                        jo.SelectToken(ip.PropPath).ToString(),
                        Mode2Store(ip.IndexAttr.IndexMode),
                        Mode2Index(ip.IndexAttr.IndexMode),
                        Mode2TermVector(ip.IndexAttr.IndexMode)))
                    .Do(f => doc.Add(f));

                var agglomProps = indexedProps.Where(ip => ip.IndexAttr.IndexMode == IndexAttribute.Mode.Agglomerate);
                if (agglomProps.Any())
                {
                    string glom = agglomProps.Select(ip => jo.SelectToken(ip.PropPath).ToString()).Join(" ").Trim();
                    doc.Add(new Field("_GLOM_", glom, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES));
                }

                doc.Add(new Field("_CONTENTADDRESS_", contentItem.ContentAddress.ToString(), Field.Store.YES, Field.Index.NO));

                indexWriter.AddDocument(doc);
            }

            //optimize and close the writer
            indexWriter.Optimize();
            indexWriter.Close();
        }

        public List<ContentAddress> Search(string search, Dictionary<string, object> nonTextualMatches)
        {
            string indexFileLocation = GetIndexFilePath();
            Directory dir =
                Lucene.Net.Store.FSDirectory.GetDirectory(indexFileLocation);
            Analyzer analyzer = new StandardAnalyzer();
            IndexSearcher searcher = new IndexSearcher(dir);
            try
            {
                QueryParser parser = new QueryParser("_GLOM_", analyzer);
                BooleanQuery query = new BooleanQuery();
                if (!string.IsNullOrEmpty(search))
                {
                    Query textQuery = parser.Parse(search);
                    query.Add(textQuery, BooleanClause.Occur.MUST);
                }
                foreach (var match in nonTextualMatches)
                {
                    query.Add(new TermQuery(new Term(match.Key, match.Value.ToString())), BooleanClause.Occur.MUST);
                }
                TopDocs hits = searcher.Search(query, 1000);
                List<ContentAddress> addrs =
                    Enumerable.Range(0, hits.totalHits)
                        .Select(n => ContentAddress.FromString(searcher.Doc(hits.scoreDocs[n].doc).Get("_CONTENTADDRESS_")))
                        .ToList();
                return addrs;
            }
            finally
            {
                searcher.Close();
                analyzer.Close();
                dir.Close();
            }
        }

        private Field.Index Mode2Index(IndexAttribute.Mode mode)
        {
            switch (mode)
            {
                case IndexAttribute.Mode.Textual:
                    return Field.Index.ANALYZED;
                case IndexAttribute.Mode.NonTextual:
                default:
                    return Field.Index.NOT_ANALYZED;
            }
        }

        private Field.TermVector Mode2TermVector(IndexAttribute.Mode mode)
        {
            switch (mode)
            {
                case IndexAttribute.Mode.Textual:
                    return Field.TermVector.YES;
                case IndexAttribute.Mode.NonTextual:
                default:
                    return Field.TermVector.NO;
            }
        }

        private Field.Store Mode2Store(IndexAttribute.Mode mode)
        {
            return Field.Store.NO;
        }
    }
}
