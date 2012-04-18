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
            Stream input = null;
            FileStream output = null;
            try
            {
                input = Request.InputStream;
                output = File.OpenWrite(path);
                input.CopyTo(output);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (input != null) input.Close();
                if (output != null) output.Close();
            }
            return true;
        }
    }   
}
