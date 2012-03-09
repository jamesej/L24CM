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

namespace L24CM.Search
{
    public class SearchManager
    {
        public static void BuildIndex()
        {
            //state the file location of the index
            string indexFileLocation = HttpContext.Current.Server.MapPath("/App_Data/Lucene");
            Lucene.Net.Store.Directory dir =
                Lucene.Net.Store.FSDirectory.GetDirectory(indexFileLocation, true);

            //create an analyzer to process the text
            Lucene.Net.Analysis.Analyzer analyzer = new
            Lucene.Net.Analysis.Standard.StandardAnalyzer();

            //create the index writer with the directory and analyzer defined.
            Lucene.Net.Index.IndexWriter indexWriter = new
            Lucene.Net.Index.IndexWriter(dir, analyzer,
                /*true to create a new index*/ true);

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

                indexedProps.Select(ip =>
                    new Field(ip.PropPath,
                        jo.SelectToken(ip.PropPath).ToString(),
                        Mode2Store(ip.IndexAttr.IndexMode),
                        Mode2Index(ip.IndexAttr.IndexMode),
                        Mode2TermVector(ip.IndexAttr.IndexMode)))
                    .Do(f => doc.Add(f));

            }



            //Lucene.Net.Documents.Field fldContent =
            //  new Lucene.Net.Documents.Field("content",
            //  "The quick brown fox jumps over the lazy dog",
            //  Lucene.Net.Documents.Field.Store.YES,


            //Lucene.Net.Documents.Field.Index.TOKENIZED,
            //Lucene.Net.Documents.Field.TermVector.YES);

            //doc.Add(fldContent);

            ////write the document to the index
            //indexWriter.AddDocument(doc);

            ////optimize and close the writer
            //indexWriter.Optimize();
            //indexWriter.Close();
        }

        private static Field.Index Mode2Index(IndexAttribute.Mode mode)
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

        private static Field.TermVector Mode2TermVector(IndexAttribute.Mode mode)
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

        private static Field.Store Mode2Store(IndexAttribute.Mode mode)
        {
            return Field.Store.NO;
        }
    }
}
