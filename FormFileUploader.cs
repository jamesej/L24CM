using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace L24CM
{
    public class FormFileUploader : FileUploader
    {
        public override int Size
        {
            get { return Request.Files["qqfile"].ContentLength; }
        }

        public override string Name
        {
            get { return Request.Files["qqfile"].FileName; }
        }

        public override bool Save(string path)
        {
            try
            {
                Request.Files["qqfile"].SaveAs(path);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
