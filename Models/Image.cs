using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Models
{
    public class Image
    {
        public string Url { get; set; }
        string alt = null;
        public string Alt
        {
            get
            {
                if (alt == null)
                    return "image";
                else
                    return alt;
            }
            set { alt = value; }
        }

        public Image()
        {
            Url = "";
        }
    }
}
