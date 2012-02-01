using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace L24CM
{
    public abstract class FileUploader
    {
        protected HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }

        public abstract int Size { get; }
        public abstract string Name { get; }
        public abstract bool Save(string path);
    }
}
