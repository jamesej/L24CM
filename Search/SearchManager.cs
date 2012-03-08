using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using L24CM.Models;
using Lucene.Net.Documents;
using L24CM.Attributes;

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
                    .Select(pi => pi.GetCustomAttributes(typeof(IndexAttribute), false).FirstOrDefault())
                    .Where(ia => ia != null)
                    .ToList();

                // type has no indexing
                if (indexedProps.Count == 0)
                    continue;

                Document doc = new Document();
                

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
    }
}
