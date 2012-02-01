using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace L24CM
{
    public class XhrFileUploader : FileUploader
    {
        public override int Size
        {
            get { return int.Parse(Request.Headers["Content-Length"]); }
        }

        public override string Name
        {
            get { return Request.QueryString["qqfile"]; }
        }

        public override bool Save(string path)
        {
            Stream input = Request.InputStream;
            FileStream output = File.OpenWrite(path);
            int actualSize = input.CopyTo(output);
            input.Close();
            output.Close();
            return (actualSize == this.Size);
        }
    }   
}
