using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Models
{
    public class MediaFileLink
    {
        public string Url { get; set; }
        public BbText Content { get; set; }

        public MediaFileLink()
        {
            Url = "";
        }
    }
}
